import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../../core/services/user.service';
import { PostService } from '../../core/services/post.service';
import { AuthService } from '../../core/services/auth.service';
import { User, Post } from '../../core/models/post.models';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  private userService = inject(UserService);
  private postService = inject(PostService);
  private authService = inject(AuthService);
  private router = inject(Router);
  private fb = inject(FormBuilder);

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
    if (!userId) {
      this.router.navigate(['/login']);
      return;
    }
    this.loadUser(userId);
    this.loadUserPosts();
  }

  loadUser(id: string): void {
    console.log('[ProfileComponent] Loading user:', id);
    this.userService.getUserById(id).subscribe({
      next: (user) => {
        console.log('[ProfileComponent] User loaded successfully:', user.username);
        this.user = user;
        this.editForm.patchValue({
          email: user.email,
          nume: '',
          prenume: ''
        });
        this.isLoading = false;
      },
      error: (err) => {
        console.error('[ProfileComponent] Error loading user:', err);
        if (err.name === 'TimeoutError') {
          this.errorMessage = 'Timeout: Serverul nu răspunde. Verifică dacă backend-ul este pornit.';
        } else if (err.status === 401) {
          this.errorMessage = 'Nu ești autentificat. Conectează-te din nou.';
        } else {
          this.errorMessage = 'Nu s-au putut încărca datele profilului. Detalii: ' + (err.message || err.statusText);
        }
        this.isLoading = false;
      }
    });
  }

  loadUserPosts(): void {
    console.log('[ProfileComponent] Loading user posts for:', this.currentUsername);
    this.postService.getAllPosts().subscribe({
      next: (posts) => {
        console.log('[ProfileComponent] All posts loaded, filtering by user:', this.currentUsername);
        this.userPosts = posts.filter(post => post.numeAutor === this.currentUsername);
        console.log('[ProfileComponent] User posts:', this.userPosts.length);
        this.isLoadingPosts = false;
      },
      error: (err) => {
        console.error('[ProfileComponent] Error loading posts:', err);
        if (err.name === 'TimeoutError') {
          this.postsErrorMessage = 'Timeout: Serverul nu răspunde la încărcarea postărilor.';
        } else {
          this.postsErrorMessage = 'Nu s-au putut încărca postările. Detalii: ' + (err.message || err.statusText);
        }
        this.isLoadingPosts = false;
      }
    });
  }

  startEdit(): void {
    this.isEditing = true;
    this.updateSuccess = false;
  }

  cancelEdit(): void {
    this.isEditing = false;
    if (this.user) {
      this.editForm.patchValue({ email: this.user.email, nume: '', prenume: '' });
    }
  }

  submitEdit(): void {
    if (this.editForm.invalid || !this.user) return;
    const userId = this.authService.getCurrentUserId()!;
    const { email, nume, prenume } = this.editForm.value;

    console.log('[ProfileComponent] Updating user:', userId);
    this.userService.updateUser(userId, {
      email,
      nume,
      prenume,
      username: this.currentUsername
    }).subscribe({
      next: (updatedUser) => {
        console.log('[ProfileComponent] User updated successfully');
        this.user = updatedUser;
        this.isEditing = false;
        this.updateSuccess = true;
      },
      error: (err) => {
        console.error('[ProfileComponent] Error updating user:', err);
        if (err.name === 'TimeoutError') {
          this.errorMessage = 'Timeout: Serverul nu răspunde la actualizare.';
        } else {
          this.errorMessage = 'Nu s-a putut actualiza profilul. Detalii: ' + (err.message || err.statusText);
        }
      }
    });
  }

  goToFeed(): void {
    this.router.navigate(['/posts']);
  }

  goToPost(postId: number): void {
    this.router.navigate(['/posts', postId]);
  }

  deletePost(postId: number): void {
    if (!confirm('Ești sigur că vrei să ștergi această postare?')) return;
    this.postService.deletePost(postId).subscribe({
      next: () => {
        console.log('[ProfileComponent] Post deleted successfully');
        this.loadUserPosts();
      },
      error: (err) => {
        console.error('[ProfileComponent] Error deleting post:', err);
        alert('Eroare la ștergerea postării. Detalii: ' + (err.message || err.statusText));
      }
    });
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
