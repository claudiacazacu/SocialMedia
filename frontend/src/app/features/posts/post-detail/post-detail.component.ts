import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { PostService } from '../../../core/services/post.service';
import { AuthService } from '../../../core/services/auth.service';
import { Post, Comment, Like } from '../../../core/models/post.models';

@Component({
  selector: 'app-post-detail',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, DatePipe],
  templateUrl: './post-detail.component.html',
  styleUrls: ['./post-detail.component.css']
})
export class PostDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private postService = inject(PostService);
  private authService = inject(AuthService);
  private fb = inject(FormBuilder);
  private cdr = inject(ChangeDetectorRef);

  post: Post | null = null;
  comments: Comment[] = [];
  likes: Like[] = [];
  isLoading = true;
  errorMessage = '';
  submitError = '';

  currentUsername = this.authService.getCurrentUsername();

  commentForm: FormGroup = this.fb.group({
    content: ['', [Validators.required, Validators.minLength(1)]]
  });

  get likesCount(): number { return this.likes.length; }
  get hasLiked(): boolean { return this.likes.some(l => l.username === this.currentUsername); }
  get myLikeId(): number | null {
    return this.likes.find(l => l.username === this.currentUsername)?.id ?? null;
  }

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (!id) { this.router.navigate(['/posts']); return; }
    this.loadPost(id);
    this.loadComments(id);
    this.loadLikes(id);
  }

  loadPost(id: number): void {
    this.postService.getPostById(id).subscribe({
      next: (post) => { this.post = post; this.isLoading = false; this.cdr.detectChanges(); },
      error: () => { this.errorMessage = 'Postarea nu a fost găsită.'; this.isLoading = false; this.cdr.detectChanges(); }
    });
  }

  loadComments(postId: number): void {
    this.postService.getCommentsForPost(postId).subscribe({
      next: (comments) => { this.comments = comments; this.cdr.detectChanges(); },
      error: () => { this.comments = []; this.cdr.detectChanges(); }
    });
  }

  loadLikes(postId: number): void {
    this.postService.getLikesForPost(postId).subscribe({
      next: (likes) => { this.likes = likes; this.cdr.detectChanges(); },
      error: () => { this.likes = []; this.cdr.detectChanges(); }
    });
  }

  toggleLike(): void {
    if (!this.post) return;
    if (this.hasLiked) {
      const likeId = this.myLikeId;
      if (likeId == null) return;
      this.postService.unlikePost(likeId).subscribe({
        next: () => this.loadLikes(this.post!.id),
        error: (err) => { console.error('Unlike error:', err); this.cdr.detectChanges(); }
      });
    } else {
      this.postService.likePost(this.post.id).subscribe({
        next: () => this.loadLikes(this.post!.id),
        error: (err) => { console.error('Like error:', err); this.cdr.detectChanges(); }
      });
    }
  }

  submitComment(): void {
    if (this.commentForm.invalid || !this.post) return;
    this.submitError = '';
    this.postService.createComment({ content: this.commentForm.value.content, postId: this.post.id }).subscribe({
      next: (comment) => { this.comments = [...this.comments, comment]; this.commentForm.reset(); this.cdr.detectChanges(); },
      error: () => { this.submitError = 'Comentariul nu a putut fi adăugat.'; this.cdr.detectChanges(); }
    });
  }

  deleteComment(commentId: number): void {
    if (!confirm('Ștergi comentariul?')) return;
    this.postService.deleteComment(commentId).subscribe({
      next: () => { this.comments = this.comments.filter(c => c.id !== commentId); this.cdr.detectChanges(); },
      error: () => { alert('Nu ai permisiunea să ștergi acest comentariu.'); this.cdr.detectChanges(); }
    });
  }

  goBack(): void { this.router.navigate(['/posts']); }
}
