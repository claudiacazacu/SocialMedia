import { Component, OnInit, inject, ElementRef, ViewChild, ChangeDetectorRef } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { PostService } from '../../../core/services/post.service';
import { AuthService } from '../../../core/services/auth.service';
import { Post, Like } from '../../../core/models/post.models';

@Component({
  selector: 'app-post-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, DatePipe],
  templateUrl: './post-list.component.html',
  styleUrls: ['./post-list.component.css']
})
export class PostListComponent implements OnInit {
  private postService = inject(PostService);
  private authService = inject(AuthService);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  private cdr = inject(ChangeDetectorRef);

  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;

  posts: Post[] = [];
  likesMap: Map<number, Like[]> = new Map();
  isLoading = true;
  errorMessage = '';
  showCreateForm = false;

  selectedFile: File | null = null;
  previewUrl: string | null = null;
  isUploading = false;
  uploadError = '';

  currentUsername = this.authService.getCurrentUsername();
  isAdmin = this.authService.isAdmin();

  createForm: FormGroup = this.fb.group({
    descriere: ['', [Validators.required, Validators.minLength(3)]]
  });

  ngOnInit(): void {
    this.loadPosts();
  }

  loadPosts(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.postService.getAllPosts().subscribe({
      next: (posts) => {
        this.posts = posts;
        this.isLoading = false;
        this.cdr.detectChanges();
        posts.forEach(p => this.loadLikes(p.id));
      },
      error: (err) => {
        if (err.name === 'TimeoutError') {
          this.errorMessage = 'Timeout: Serverul nu răspunde. Verifică dacă backend-ul este pornit.';
        } else if (err.status === 401) {
          this.errorMessage = 'Nu ești autentificat. Conectează-te din nou.';
        } else {
          this.errorMessage = 'Nu s-au putut încărca postările. Detalii: ' + (err.message || err.statusText);
        }
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  loadLikes(postId: number): void {
    this.postService.getLikesForPost(postId).subscribe({
      next: (likes) => { this.likesMap.set(postId, likes); this.cdr.detectChanges(); },
      error: () => { this.likesMap.set(postId, []); this.cdr.detectChanges(); }
    });
  }

  getLikesCount(postId: number): number {
    return this.likesMap.get(postId)?.length ?? 0;
  }

  hasLiked(postId: number): boolean {
    return (this.likesMap.get(postId) ?? []).some(l => l.username === this.currentUsername);
  }

  getMyLikeId(postId: number): number | null {
    return (this.likesMap.get(postId) ?? []).find(l => l.username === this.currentUsername)?.id ?? null;
  }

  toggleLike(postId: number): void {
    if (this.hasLiked(postId)) {
      const likeId = this.getMyLikeId(postId);
      if (likeId == null) return;
      this.postService.unlikePost(likeId).subscribe({
        next: () => this.loadLikes(postId),
        error: (err) => { console.error('Unlike error:', err); this.cdr.detectChanges(); }
      });
    } else {
      this.postService.likePost(postId).subscribe({
        next: () => this.loadLikes(postId),
        error: (err) => { console.error('Like error:', err); this.cdr.detectChanges(); }
      });
    }
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;
    this.uploadError = '';
    this.selectedFile = file;
    const reader = new FileReader();
    reader.onload = (e) => {
      this.previewUrl = e.target?.result as string;
      this.cdr.detectChanges();
    };
    reader.readAsDataURL(file);
  }

  removeImage(): void {
    this.selectedFile = null;
    this.previewUrl = null;
    if (this.fileInput) this.fileInput.nativeElement.value = '';
    this.cdr.detectChanges();
  }

  submitPost(): void {
    if (this.createForm.invalid || !this.selectedFile) return;
    this.isUploading = true;
    this.uploadError = '';
    this.cdr.detectChanges();

    this.postService.uploadImage(this.selectedFile).subscribe({
      next: (res) => {
        this.postService.createPost({
          descriere: this.createForm.value.descriere,
          imageUrl: res.imageUrl
        }).subscribe({
          next: () => {
            this.createForm.reset();
            this.removeImage();
            this.showCreateForm = false;
            this.isUploading = false;
            this.cdr.detectChanges();
            this.loadPosts();
          },
          error: (err) => {
            this.uploadError = err.name === 'TimeoutError'
              ? 'Timeout: Serverul nu răspunde la crearea postării.'
              : 'Postarea nu a putut fi creată. Detalii: ' + (err.message || err.statusText);
            this.isUploading = false;
            this.cdr.detectChanges();
          }
        });
      },
      error: (err) => {
        this.uploadError = err.name === 'TimeoutError'
          ? 'Timeout: Serverul nu răspunde la încărcarea imaginii.'
          : 'Eroare la încărcarea imaginii. Detalii: ' + (err.message || err.statusText);
        this.isUploading = false;
        this.cdr.detectChanges();
      }
    });
  }

  toggleCreateForm(): void {
    this.showCreateForm = !this.showCreateForm;
    if (!this.showCreateForm) {
      this.createForm.reset();
      this.removeImage();
      this.uploadError = '';
    }
    this.cdr.detectChanges();
  }

  deletePost(postId: number): void {
    if (!confirm('Ești sigur că vrei să ștergi această postare?')) return;
    this.postService.deletePost(postId).subscribe({
      next: () => this.loadPosts(),
      error: (err) => { console.error('Delete post error:', err); this.cdr.detectChanges(); }
    });
  }

  goToPost(postId: number): void { this.router.navigate(['/posts', postId]); }
  goToProfile(): void { this.router.navigate(['/profile']); }
  logout(): void { this.authService.logout(); this.router.navigate(['/login']); }
}
