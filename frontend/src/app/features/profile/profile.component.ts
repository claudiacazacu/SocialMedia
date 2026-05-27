import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../../core/services/user.service';
import { PostService } from '../../core/services/post.service';
import { AuthService } from '../../core/services/auth.service';
import { User, Post } from '../../core/models/post.models';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, DatePipe],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  private userService = inject(UserService);
  private postService = inject(PostService);
  private authService = inject(AuthService);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  private cdr = inject(ChangeDetectorRef);

  user: User | null = null;
  userPosts: Post[] = [];
  isLoading = true;
  isLoadingPosts = true;
  errorMessage = '';
  postsErrorMessage = '';
  isEditing = false;
  updateSuccess = false;

  currentUsername = this.authService.getCurrentUsername() ?? '';

  editForm: FormGroup = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    nume: ['', Validators.required],
    prenume: ['', Validators.required]
  });

  ngOnInit(): void {
    const userId = this.authService.getCurrentUserId();
    if (!userId) { this.router.navigate(['/login']); return; }
    this.loadUser(userId);
    this.loadUserPosts(userId);
  }

  loadUser(id: string): void {
    this.userService.getUserById(id).subscribe({
      next: (user) => {
        this.user = user;
        this.editForm.patchValue({ email: user.email, nume: '', prenume: '' });
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        if (err.name === 'TimeoutError') {
          this.errorMessage = 'Timeout: Serverul nu răspunde.';
        } else if (err.status === 401) {
          this.errorMessage = 'Nu ești autentificat. Conectează-te din nou.';
        } else {
          this.errorMessage = 'Nu s-au putut încărca datele profilului. Detalii: ' + (err.message || err.statusText);
        }
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  loadUserPosts(userId: string): void {
    this.postService.getPostsByUserId(userId).subscribe({
      next: (posts) => {
        this.userPosts = posts;
        this.isLoadingPosts = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.postsErrorMessage = err.name === 'TimeoutError'
          ? 'Timeout: Serverul nu răspunde.'
          : 'Nu s-au putut încărca postările. Detalii: ' + (err.message || err.statusText);
        this.isLoadingPosts = false;
        this.cdr.detectChanges();
      }
    });
  }

  startEdit(): void { this.isEditing = true; this.updateSuccess = false; this.cdr.detectChanges(); }

  cancelEdit(): void {
    this.isEditing = false;
    if (this.user) this.editForm.patchValue({ email: this.user.email, nume: '', prenume: '' });
    this.cdr.detectChanges();
  }

  submitEdit(): void {
    if (this.editForm.invalid || !this.user) return;
    const userId = this.authService.getCurrentUserId()!;
    const { email, nume, prenume } = this.editForm.value;
    this.userService.updateUser(userId, { email, nume, prenume, username: this.currentUsername }).subscribe({
      next: (updatedUser) => {
        this.user = updatedUser;
        this.isEditing = false;
        this.updateSuccess = true;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.errorMessage = err.name === 'TimeoutError'
          ? 'Timeout: Serverul nu răspunde.'
          : 'Nu s-a putut actualiza profilul. Detalii: ' + (err.message || err.statusText);
        this.cdr.detectChanges();
      }
    });
  }

  deletePost(postId: number): void {
    if (!confirm('Ești sigur că vrei să ștergi această postare?')) return;
    const userId = this.authService.getCurrentUserId();
    this.postService.deletePost(postId).subscribe({
      next: () => { if (userId) this.loadUserPosts(userId); this.cdr.detectChanges(); },
      error: (err) => { alert('Eroare: ' + (err.message || err.statusText)); this.cdr.detectChanges(); }
    });
  }

  goToFeed(): void { this.router.navigate(['/posts']); }
  goToPost(postId: number): void { this.router.navigate(['/posts', postId]); }
  logout(): void { this.authService.logout(); this.router.navigate(['/login']); }
}
