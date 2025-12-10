import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';

@Injectable({ providedIn: 'root' })
export class ProtectRouter implements CanActivate {
  constructor(private router: Router) {}

  canActivate(): boolean {
    const token = localStorage.getItem('adminToken');
    
    if (!token) {
      this.router.navigate(['/admin-login']);
      return false;
    }

    try {
      const decoded: any = jwtDecode(token);
      // Any valid admin token can access users-dashboard
      // (admin users managing other users)
      return true;
    } catch (error) {
      console.error('Invalid token', error);
    }

    this.router.navigate(['/admin-login']);
    return false;
  }
}