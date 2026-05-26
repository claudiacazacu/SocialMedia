import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { PostListComponent } from './features/posts/post-list/post-list.component';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  // Rute publice
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  
  { 
    path: 'posts', 
    component: PostListComponent, 
    canActivate: [authGuard] 
  },
  
  { path: '', redirectTo: '/posts', pathMatch: 'full' },
  { path: '**', redirectTo: '/posts' } 
];