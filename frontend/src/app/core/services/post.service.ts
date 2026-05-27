import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Post, CreatePostDto, UpdatePostDto, Comment, CreateCommentDto, Like, CreateLikeDto } from '../models/post.models';

@Injectable({
  providedIn: 'root'
})
export class PostService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}`;

  // Posts
  getAllPosts(): Observable<Post[]> {
    return this.http.get<Post[]>(`${this.apiUrl}/posts`);
  }

  getPostById(id: number): Observable<Post> {
    return this.http.get<Post>(`${this.apiUrl}/posts/${id}`);
  }

  createPost(dto: CreatePostDto): Observable<Post> {
    return this.http.post<Post>(`${this.apiUrl}/posts`, dto);
  }

  updatePost(id: number, dto: UpdatePostDto): Observable<Post> {
    return this.http.put<Post>(`${this.apiUrl}/posts/${id}`, dto);
  }

  deletePost(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/posts/${id}`);
  }

  // Comments
  getCommentsForPost(postId: number): Observable<Comment[]> {
    return this.http.get<Comment[]>(`${this.apiUrl}/comments/post/${postId}`);
  }

  createComment(dto: CreateCommentDto): Observable<Comment> {
    return this.http.post<Comment>(`${this.apiUrl}/comments`, dto);
  }

  deleteComment(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/comments/${id}`);
  }

  // Likes
  getLikesForPost(postId: number): Observable<Like[]> {
    return this.http.get<Like[]>(`${this.apiUrl}/likes/post/${postId}`);
  }

  likePost(postId: number): Observable<Like> {
    const dto: CreateLikeDto = { postId };
    return this.http.post<Like>(`${this.apiUrl}/likes`, dto);
  }

  unlikePost(likeId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/likes/${likeId}`);
  }
}
