import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';

@Injectable({ providedIn: 'root' })
export class AdminGuard implements CanActivate {
  constructor(private router: Router) {}

  canActivate(): boolean {
    const token = localStorage.getItem('adminToken');
    
    if (!token) {
      this.router.navigate(['/admin-login']);
      return false;
    }

    try {
      const decoded: any = jwtDecode(token);
      
      // Check multiple ways the role claim might appear
      const roleClaimUri = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
      const role = decoded.role ?? decoded[roleClaimUri];
      const isAdmin = decoded.isAdmin === 'true' || role === 'Admin';
      
      if (isAdmin) {
        return true;
      }
    } catch (error) {
      console.error('Invalid token', error);
    }

    this.router.navigate(['/admin-login']);
    return false;
  }
}
