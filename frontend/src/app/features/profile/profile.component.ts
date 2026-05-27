import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../../core/services/user.service';
import { AuthService } from '../../core/services/auth.service';
import { User } from '../../core/models/post.models';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  private userService = inject(UserService);
  private authService = inject(AuthService);
  private router = inject(Router);
  private fb = inject(FormBuilder);

  user: User | null = null;
  isLoading = true;
  errorMessage = '';
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
  }

  loadUser(id: string): void {
    this.userService.getUserById(id).subscribe({
      next: (user) => {
        this.user = user;
        this.editForm.patchValue({
          email: user.email,
          nume: '',
          prenume: ''
        });
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Nu s-au putut încărca datele profilului.';
        this.isLoading = false;
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

    this.userService.updateUser(userId, {
      email,
      nume,
      prenume,
      username: this.currentUsername
    }).subscribe({
      next: (updatedUser) => {
        this.user = updatedUser;
        this.isEditing = false;
        this.updateSuccess = true;
      },
      error: () => {
        this.errorMessage = 'Nu s-a putut actualiza profilul.';
      }
    });
  }

  goToFeed(): void {
    this.router.navigate(['/posts']);
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
