import { Routes } from '@angular/router';
import { Login } from './login/login';
import { Signup } from './signup/signup';
import { AdminDashboardComponent } from './admin/admin-dashboard.component'; // novo
import { AdminLoginComponent } from './admin/admin-login.component';

export const routes: Routes = [
  { path: '', component: Login },
  { path: 'signup', component: Signup },
  { path: 'admin', component: AdminDashboardComponent },
  { path: 'admin-login', component: AdminLoginComponent }
];

