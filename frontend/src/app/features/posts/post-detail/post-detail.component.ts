import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { PostService } from '../../../core/services/post.service';
import { AuthService } from '../../../core/services/auth.service';
import { Post, Comment } from '../../../core/models/post.models';

@Component({
  selector: 'app-post-detail',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './post-detail.component.html',
  styleUrls: ['./post-detail.component.css']
})
export class PostDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private postService = inject(PostService);
  private authService = inject(AuthService);
  private fb = inject(FormBuilder);

  post: Post | null = null;
  comments: Comment[] = [];
  isLoading = true;
  errorMessage = '';
  submitError = '';

  currentUsername = this.authService.getCurrentUsername();

  commentForm: FormGroup = this.fb.group({
    content: ['', [Validators.required, Validators.minLength(1)]]
  });

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (!id) {
      this.router.navigate(['/posts']);
      return;
    }
    this.loadPost(id);
    this.loadComments(id);
  }

  loadPost(id: number): void {
    this.postService.getPostById(id).subscribe({
      next: (post) => {
        this.post = post;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Postarea nu a fost găsită.';
        this.isLoading = false;
      }
    });
  }

  loadComments(postId: number): void {
    this.postService.getCommentsForPost(postId).subscribe({
      next: (comments) => this.comments = comments,
      error: () => this.comments = []
    });
  }

  submitComment(): void {
    if (this.commentForm.invalid || !this.post) return;
    this.submitError = '';

    this.postService.createComment({
      content: this.commentForm.value.content,
      postId: this.post.id
    }).subscribe({
      next: (comment) => {
        this.comments = [...this.comments, comment];
        this.commentForm.reset();
      },
      error: () => {
        this.submitError = 'Comentariul nu a putut fi adăugat.';
      }
    });
  }

  deleteComment(commentId: number): void {
    if (!confirm('Ștergi comentariul?')) return;
    this.postService.deleteComment(commentId).subscribe({
      next: () => {
        this.comments = this.comments.filter(c => c.id !== commentId);
      },
      error: () => alert('Nu ai permisiunea să ștergi acest comentariu.')
    });
  }

  goBack(): void {
    this.router.navigate(['/posts']);
  }
}
