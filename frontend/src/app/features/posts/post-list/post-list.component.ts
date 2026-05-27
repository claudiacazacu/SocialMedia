import { Component, OnInit, inject, ElementRef, ViewChild } from '@angular/core';
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

  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;

  posts: Post[] = [];
  likesMap: Map<number, Like[]> = new Map();
  isLoading = true;
  errorMessage = '';
  showCreateForm = false;

  // Upload state
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
    console.log('[PostListComponent] Loading posts...');
    this.postService.getAllPosts().subscribe({
      next: (posts) => {
        console.log('[PostListComponent] Posts loaded successfully:', posts.length);
        this.posts = posts;
        this.isLoading = false;
        posts.forEach(p => this.loadLikes(p.id));
      },
      error: (err) => {
        console.error('[PostListComponent] Error loading posts:', err);
        if (err.name === 'TimeoutError') {
          this.errorMessage = 'Timeout: Serverul nu răspunde. Verifică dacă backend-ul este pornit.';
        } else if (err.status === 401) {
          this.errorMessage = 'Nu ești autentificat. Conectează-te din nou.';
        } else {
          this.errorMessage = 'Nu s-au putut încărca postările. Detalii: ' + (err.message || err.statusText);
        }
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

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    this.uploadError = '';
    this.selectedFile = file;

    // Preview local
    const reader = new FileReader();
    reader.onload = (e) => {
      this.previewUrl = e.target?.result as string;
    };
    reader.readAsDataURL(file);
  }

  removeImage(): void {
    this.selectedFile = null;
    this.previewUrl = null;
    if (this.fileInput) {
      this.fileInput.nativeElement.value = '';
    }
  }

  submitPost(): void {
    if (this.createForm.invalid || !this.selectedFile) return;

    this.isUploading = true;
    this.uploadError = '';

    console.log('[PostListComponent] Starting post creation...');
    this.postService.uploadImage(this.selectedFile).subscribe({
      next: (res) => {
        console.log('[PostListComponent] Image uploaded, URL:', res.imageUrl);
        this.postService.createPost({
          descriere: this.createForm.value.descriere,
          imageUrl: res.imageUrl
        }).subscribe({
          next: () => {
            console.log('[PostListComponent] Post created successfully, reloading...');
            this.createForm.reset();
            this.removeImage();
            this.showCreateForm = false;
            this.isUploading = false;
            this.loadPosts();
          },
          error: (err) => {
            console.error('[PostListComponent] Error creating post:', err);
            if (err.name === 'TimeoutError') {
              this.uploadError = 'Timeout: Serverul nu răspunde la crearea postării.';
            } else {
              this.uploadError = 'Imaginea a fost încărcată, dar postarea nu a putut fi creată. Detalii: ' + (err.message || err.statusText);
            }
            this.isUploading = false;
          }
        });
      },
      error: (err) => {
        console.error('[PostListComponent] Error uploading image:', err);
        if (err.name === 'TimeoutError') {
          this.uploadError = 'Timeout: Serverul nu răspunde la încărcarea imaginii.';
        } else {
          this.uploadError = 'Eroare la încărcarea imaginii. Încearcă din nou. Detalii: ' + (err.message || err.statusText);
        }
        this.isUploading = false;
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
