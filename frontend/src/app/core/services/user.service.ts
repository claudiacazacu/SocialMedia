import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { User, UpdateUserDto } from '../models/post.models';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}`;

  getAllUsers(): Observable<User[]> {
    return this.http.get<User[]>(`${this.apiUrl}/users`);
  }

  getUserById(id: string): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/users/${id}`);
  }

  updateUser(id: string, dto: UpdateUserDto): Observable<User> {
    return this.http.put<User>(`${this.apiUrl}/users/${id}`, dto);
  }

  deleteUser(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/users/${id}`);
  }

  getFollowers(userId: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/follow/followers/${userId}`);
  }

  getFollowing(userId: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/follow/following/${userId}`);
  }

  followUser(followingId: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/follow/${followingId}`, {});
  }

  unfollowUser(followingId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/follow/${followingId}`);
  }
}
