import { Component, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-admin-login',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './admin-login.component.html',
  styleUrls: ['./admin-login.component.scss']
})
export class AdminLoginComponent {
  @ViewChild('adminLoginForm') adminLoginForm?: NgForm;

  phone = '';
  secret = '';
  loading = false;
  errorMessage = '';
  private readonly base = environment.baseApiUrl.replace(/\/$/, '');

  constructor(private http: HttpClient, private router: Router) {}

  allowOnlyNumbers(event: KeyboardEvent): void {
    const allowed = ['Backspace','ArrowLeft','ArrowRight','Tab','Delete','Enter','Home','End'];
    const combo = (event.ctrlKey || event.metaKey) && /[acvx]/i.test(event.key);
    const digit = /^[0-9]$/.test(event.key);
    if (!digit && !allowed.includes(event.key) && !combo) event.preventDefault();
  }

  onPasteNumbersOnly(event: ClipboardEvent): void {
    const data = event.clipboardData?.getData('text') ?? '';
    const digits = data.replace(/\D/g, '');
    if (!digits) { event.preventDefault(); return; }
    event.preventDefault();
    const target = event.target as HTMLInputElement;
    const start = target.selectionStart ?? target.value.length;
    const end = target.selectionEnd ?? target.value.length;
    const val = target.value.slice(0, start) + digits + target.value.slice(end);
    target.value = val.slice(0, 8);
    this.phone = target.value;
  }

  submit(): void {
    if (this.loading) return;
    this.errorMessage = '';

    const phoneDigits = (this.phone ?? '').replace(/\D/g, '');
    if (!/^\d{8}$/.test(phoneDigits)) {
      this.errorMessage = 'Telefon skal være 8 cifre.';
      return;
    }
    if (!this.secret) {
      this.errorMessage = 'Secret er påkrævet.';
      return;
    }

    this.loading = true;
    this.http.post<{token:string,user:string}>(`${this.base}/admin/login`, {
      phone: phoneDigits,
      secret: this.secret
    }).subscribe({
      next: res => {
        localStorage.setItem('adminToken', res.token);
        this.loading = false;
        this.adminLoginForm?.resetForm();
        this.router.navigate(['/admin']);
      },
      error: () => {
        this.loading = false;
        this.errorMessage = 'Uautoriseret eller forkert secret.';
      }
    });
  }

  goToMain(): void {
    this.router.navigate(['/']);
  }
}
    