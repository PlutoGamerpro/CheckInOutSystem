import { Component } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { jwtDecode } from 'jwt-decode';

type RoleOption = 'Admin' | 'Manager' | 'User';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './profile.html',
  styleUrls: ['./profile.scss']
})
export class ProfileComponent {
  photoDataUrl?: string;
  faviconUrl = '/favicon.ico';

  fullName = '';
  email = '';
  role: RoleOption = 'User';
  absenceInfo = '0 fraværsdage (seneste 30 dage)';

  constructor(private location: Location) {
    const token = localStorage.getItem('adminToken') || localStorage.getItem('userToken');
    if (token) {
      try {
        const payload: any = jwtDecode(token);
        this.fullName = payload.name || payload.unique_name || payload.given_name || this.fullName;
        this.email = payload.email || payload.upn || this.email;
        const roleClaimUri = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
        const rawRole: string = payload.role || payload[roleClaimUri] || (localStorage.getItem('adminToken') ? 'Admin' : 'User');
        this.role = (['Admin','Manager','User'].includes(rawRole) ? rawRole : 'User') as RoleOption;
      } catch {
        // ignore malformed tokens
      }
    }
  }

  onPhotoSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;
    const reader = new FileReader();
    reader.onload = () => {
      this.photoDataUrl = reader.result as string;
    };
    reader.readAsDataURL(file);
  }

  goBack(): void {
    this.location.back();
  }
}
