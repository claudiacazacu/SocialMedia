import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, timeout } from 'rxjs';
import { environment } from '../../../environments/environment';
import { User, UpdateUserDto } from '../models/post.models';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}`;
  private requestTimeout = 10000; // 10 seconds

  getAllUsers(): Observable<User[]> {
    return this.http.get<User[]>(`${this.apiUrl}/users`).pipe(
      timeout(this.requestTimeout)
    );
  }

  getUserById(id: string): Observable<User> {
    console.log('[UserService] Fetching user:', id);
    return this.http.get<User>(`${this.apiUrl}/users/${id}`).pipe(
      timeout(this.requestTimeout)
    );
  }

  updateUser(id: string, dto: UpdateUserDto): Observable<User> {
    return this.http.put<User>(`${this.apiUrl}/users/${id}`, dto).pipe(
      timeout(this.requestTimeout)
    );
  }

  deleteUser(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/users/${id}`).pipe(
      timeout(this.requestTimeout)
    );
  }

  getFollowers(userId: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/follow/followers/${userId}`).pipe(
      timeout(this.requestTimeout)
    );
  }

  getFollowing(userId: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/follow/following/${userId}`).pipe(
      timeout(this.requestTimeout)
    );
  }

  followUser(followingId: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/follow/${followingId}`, {}).pipe(
      timeout(this.requestTimeout)
    );
  }

  unfollowUser(followingId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/follow/${followingId}`).pipe(
      timeout(this.requestTimeout)
    );
  }
}
