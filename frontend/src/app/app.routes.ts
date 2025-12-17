import { Routes } from '@angular/router';
import { Login } from './login/login';
import { Signup } from './signup/signup';
import { AdminDashboardComponent } from './admin/admin-dashboard.component'; 
import { AdminLoginComponent } from './admin/admin-login.component';
import { UsersDashBoard } from './users-dash-board/users-dash-board'; 
import { AdminGuard } from './admin-guard';
import { ProtectRouter } from './protect-router';
import { ProfileComponent } from './profile/profile';
import { NotFoundComponent } from './not-found.component';

export const routes: Routes = [
  { path: '', component: Login },
  { path: 'signup', component: Signup },
  { path: 'admin', component: AdminDashboardComponent, canActivate: [AdminGuard] },
  { path: 'admin-login', component: AdminLoginComponent },
  { path: 'users-dashboard', component: UsersDashBoard, canActivate: [AdminGuard] },
  { path: 'profile', component: ProfileComponent },
  { path: '**', component: NotFoundComponent },
];

// protectrouter allow admins and users with a valid token
// adminguard only allows admins with a valid token