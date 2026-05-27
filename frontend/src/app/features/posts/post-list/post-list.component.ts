import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { PostService } from '../../../core/services/post.service';
import { AuthService } from '../../../core/services/auth.service';
import { Post, Like } from '../../../core/models/post.models';

@Component({
  selector: 'app-post-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './post-list.component.html',
  styleUrls: ['./post-list.component.css']
})
export class PostListComponent implements OnInit {
  private postService = inject(PostService);
  private authService = inject(AuthService);
  private router = inject(Router);
  private fb = inject(FormBuilder);

  posts: Post[] = [];
  likesMap: Map<number, Like[]> = new Map();
  isLoading = true;
  errorMessage = '';
  showCreateForm = false;

  currentUsername = this.authService.getCurrentUsername();
  isAdmin = this.authService.isAdmin();

  createForm: FormGroup = this.fb.group({
    descriere: ['', [Validators.required, Validators.minLength(3)]],
    imageUrl: ['', Validators.required]
  });

  ngOnInit(): void {
    this.loadPosts();
  }

  loadPosts(): void {
    this.isLoading = true;
    this.postService.getAllPosts().subscribe({
      next: (posts) => {
        this.posts = posts;
        this.isLoading = false;
        posts.forEach(p => this.loadLikes(p.id));
      },
      error: () => {
        this.errorMessage = 'Nu s-au putut încărca postările.';
        this.isLoading = false;
      }
    });
  }

  loadLikes(postId: number): void {
    this.postService.getLikesForPost(postId).subscribe({
      next: (likes) => this.likesMap.set(postId, likes),
      error: () => this.likesMap.set(postId, [])
    });
  }

  getLikesCount(postId: number): number {
    return this.likesMap.get(postId)?.length ?? 0;
  }

  hasLiked(postId: number): boolean {
    const likes = this.likesMap.get(postId) ?? [];
    return likes.some(l => l.username === this.currentUsername);
  }

  getMyLikeId(postId: number): number | null {
    const likes = this.likesMap.get(postId) ?? [];
    const myLike = likes.find(l => l.username === this.currentUsername);
    return myLike?.id ?? null;
  }

  toggleLike(postId: number): void {
    if (this.hasLiked(postId)) {
      const likeId = this.getMyLikeId(postId);
      if (likeId == null) return;
      this.postService.unlikePost(likeId).subscribe({
        next: () => this.loadLikes(postId),
        error: (err) => console.error('Unlike error:', err)
      });
    } else {
      this.postService.likePost(postId).subscribe({
        next: () => this.loadLikes(postId),
        error: (err) => console.error('Like error:', err)
      });
    }
  }

  submitPost(): void {
    if (this.createForm.invalid) return;
    this.postService.createPost(this.createForm.value).subscribe({
      next: () => {
        this.createForm.reset();
        this.showCreateForm = false;
        this.loadPosts();
      },
      error: (err) => console.error('Create post error:', err)
    });
  }

  deletePost(postId: number): void {
    if (!confirm('Ești sigur că vrei să ștergi această postare?')) return;
    this.postService.deletePost(postId).subscribe({
      next: () => this.loadPosts(),
      error: (err) => console.error('Delete post error:', err)
    });
  }

  goToPost(postId: number): void {
    this.router.navigate(['/posts', postId]);
  }

  goToProfile(): void {
    this.router.navigate(['/profile']);
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
