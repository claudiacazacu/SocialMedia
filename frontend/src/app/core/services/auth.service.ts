import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, tap, catchError, throwError } from 'rxjs';
import { LoginRequest, RegisterRequest, AuthResponse } from '../models/auth.models';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/auth`;

  private loggedInSubject = new BehaviorSubject<boolean>(this.hasToken());
  public isLoggedIn$ = this.loggedInSubject.asObservable();

  register(data: RegisterRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, data).pipe(
      catchError(err => {
        console.error('Register error:', err);
        return throwError(() => err);
      })
    );
  }

  login(data: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, data).pipe(
      tap(response => {
        const token = (response as any)?.token || (response as any)?.data?.token;
        if (token) {
          localStorage.setItem('jwt_token', token);
          this.loggedInSubject.next(true);
        }
      }),
      catchError(err => {
        console.error('Login error:', err);
        return throwError(() => err);
      })
    );
  }

  logout(): void {
    localStorage.removeItem('jwt_token');
    this.loggedInSubject.next(false);
  }

  hasToken(): boolean {
    return !!localStorage.getItem('jwt_token');
  }

  getToken(): string | null {
    return localStorage.getItem('jwt_token');
  }

  private decodeToken(): Record<string, unknown> | null {
    const token = this.getToken();
    if (!token) return null;
    try {
      const payload = token.split('.')[1];
      if (!payload) return null;
      const base64 = payload.replace(/-/g, '+').replace(/_/g, '/');
      const json = decodeURIComponent(
        atob(base64).split('').map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2)).join('')
      );
      return JSON.parse(json);
    } catch (error) {
      console.error('Failed to decode JWT token', error);
      return null;
    }
  }

  getCurrentUsername(): string | null {
    const tokenData = this.decodeToken();
    return (
      tokenData?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] as string
      || tokenData?.['name'] as string
      || tokenData?.['unique_name'] as string
      || null
    );
  }

  getCurrentUserId(): string | null {
    const tokenData = this.decodeToken();
    return (
      tokenData?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] as string
      || tokenData?.['nameid'] as string
      || tokenData?.['sub'] as string
      || null
    );
  }

  isAdmin(): boolean {
    const tokenData = this.decodeToken();
    if (!tokenData) return false;
    const roleClaim = tokenData?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
      || tokenData?.['role']
      || tokenData?.['roles'];
    if (Array.isArray(roleClaim)) return roleClaim.includes('Admin');
    return typeof roleClaim === 'string' && roleClaim === 'Admin';
  }
}
