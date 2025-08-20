import { Routes } from '@angular/router';
import { Login } from './login/login';
import { Signup } from './signup/signup';
import { AdminDashboardComponent } from './admin/admin-dashboard.component'; 
import { AdminLoginComponent } from './admin/admin-login.component';
import { UsersDashBoard } from './users-dash-board/users-dash-board'; 


export const routes: Routes = [
  { path: '', component: Login },
  { path: 'signup', component: Signup },
  { path: 'admin', component: AdminDashboardComponent },
  { path: 'admin-login', component: AdminLoginComponent },
  { path: 'users-dashboard', component: UsersDashBoard },
];

