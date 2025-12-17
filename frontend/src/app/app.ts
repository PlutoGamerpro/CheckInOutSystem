import { Component } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected title = 'project';
  protected isProfileMenuOpen = false;



  constructor() {
   // localStorage.removeItem('adminToken');
    //localStorage.removeItem('userToken');
   }


   checkAdminToken(): boolean {
    const token = localStorage.getItem('adminToken');
    return token !== null;
  }
  checkUserToken(): boolean {
    const token = localStorage.getItem('userToken');
    return token !== null;
  }
  private getActiveToken(): string | null {
    return (
      localStorage.getItem('adminToken') ||
      localStorage.getItem('userToken') ||
      null
    );
  }
  private decodeJwt(token: string | null): any | null {
    if (!token) return null;
    const parts = token.split('.');
    if (parts.length < 2) return null;
    try {
      const payload = parts[1]
        .replace(/-/g, '+')
        .replace(/_/g, '/');
      const json = atob(payload);
      return JSON.parse(json);
    } catch {
      return null;
    }
  }
  getProfile(): { name?: string; email?: string; role?: string; exp?: number } | null {
    const token = this.getActiveToken();
    const payload = this.decodeJwt(token);
    if (!payload) {
      const role = this.checkAdminToken() ? 'Admin' : (this.checkUserToken() ? 'User' : undefined);
      return role ? { role } : null;
    }
    const role = this.checkAdminToken() ? 'Admin' : 'User';
    return {
      name: payload.name || payload.unique_name || payload.given_name || undefined,
      email: payload.email || payload.upn || undefined,
      role,
      exp: payload.exp || undefined,
    };
  }
  getProfileStatus(): string {
    if (this.checkAdminToken()) return 'Admin logged in';
    if (this.checkUserToken()) return 'Checked in';
    return 'Guest';
  }
  toggleProfileMenu(): void {
    this.isProfileMenuOpen = !this.isProfileMenuOpen;
  }
  closeProfileMenu(): void {
    this.isProfileMenuOpen = false;
  }
  logout(): void {
    localStorage.removeItem('adminToken');
    localStorage.removeItem('userToken');
    window.location.href = '/';
  }

}

