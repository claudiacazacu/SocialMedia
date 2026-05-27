import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, timeout } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Post, CreatePostDto, UpdatePostDto, Comment, CreateCommentDto, Like, CreateLikeDto } from '../models/post.models';

@Injectable({
  providedIn: 'root'
})
export class PostService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}`;
  private requestTimeout = 10000;

  uploadImage(file: File): Observable<{ imageUrl: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ imageUrl: string }>(`${this.apiUrl}/upload/image`, formData).pipe(
      timeout(this.requestTimeout)
    );
  }

  getAllPosts(): Observable<Post[]> {
    return this.http.get<Post[]>(`${this.apiUrl}/posts`).pipe(
      timeout(this.requestTimeout)
    );
  }

  getPostsByUserId(userId: string): Observable<Post[]> {
    return this.http.get<Post[]>(`${this.apiUrl}/posts/user/${userId}`).pipe(
      timeout(this.requestTimeout)
    );
  }

  getPostById(id: number): Observable<Post> {
    return this.http.get<Post>(`${this.apiUrl}/posts/${id}`).pipe(
      timeout(this.requestTimeout)
    );
  }

  createPost(dto: CreatePostDto): Observable<Post> {
    return this.http.post<Post>(`${this.apiUrl}/posts`, dto).pipe(
      timeout(this.requestTimeout)
    );
  }

  updatePost(id: number, dto: UpdatePostDto): Observable<Post> {
    return this.http.put<Post>(`${this.apiUrl}/posts/${id}`, dto).pipe(
      timeout(this.requestTimeout)
    );
  }

  deletePost(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/posts/${id}`).pipe(
      timeout(this.requestTimeout)
    );
  }

  getCommentsForPost(postId: number): Observable<Comment[]> {
    return this.http.get<Comment[]>(`${this.apiUrl}/comments/post/${postId}`).pipe(
      timeout(this.requestTimeout)
    );
  }

  createComment(dto: CreateCommentDto): Observable<Comment> {
    return this.http.post<Comment>(`${this.apiUrl}/comments`, dto).pipe(
      timeout(this.requestTimeout)
    );
  }

  deleteComment(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/comments/${id}`).pipe(
      timeout(this.requestTimeout)
    );
  }

  getLikesForPost(postId: number): Observable<Like[]> {
    return this.http.get<Like[]>(`${this.apiUrl}/likes/post/${postId}`).pipe(
      timeout(this.requestTimeout)
    );
  }

  likePost(postId: number): Observable<Like> {
    const dto: CreateLikeDto = { postId };
    return this.http.post<Like>(`${this.apiUrl}/likes`, dto).pipe(
      timeout(this.requestTimeout)
    );
  }

  unlikePost(likeId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/likes/${likeId}`).pipe(
      timeout(this.requestTimeout)
    );
  }
}
