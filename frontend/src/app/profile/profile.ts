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
  phone = '';
  countryCode = '';
  role: RoleOption = 'User';
  absenceInfo = '0 fraværsdage (seneste 30 dage)';
  isCheckedIn: boolean | null = null;

  constructor(private location: Location) {
    const token = localStorage.getItem('adminToken') || localStorage.getItem('userToken');
    if (token) {
      try {
        const payload: any = jwtDecode(token);
        this.fullName = payload.name || payload.unique_name || payload.given_name || this.fullName;
        this.email = payload.email || payload.upn || this.email;
        this.phone = payload.phone || payload.telephone || payload.phonenumber || '';
        this.countryCode = payload.countryCode || payload.country_code || '';
        const roleClaimUri = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
        const rawRole: string = payload.role || payload[roleClaimUri] || (localStorage.getItem('adminToken') ? 'Admin' : 'User');
        this.role = (['Admin','Manager','User'].includes(rawRole) ? rawRole : 'User') as RoleOption;
        if (typeof payload.isCheckedIn !== 'undefined') {
          this.isCheckedIn = payload.isCheckedIn === true || payload.isCheckedIn === 'true';
        }
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
