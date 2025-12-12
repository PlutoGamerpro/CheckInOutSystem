import { Component } from '@angular/core';
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
  adminLoginForm?: NgForm;

  username: string = '';
  password: string = '';
  //phone: string = '';
  
  loading = false;
  errorMessage = '';


  private readonly base = environment.baseApiUrl.replace(/\/$/, '');

  constructor(private http: HttpClient, private router: Router) {}



  goToMain(): void {
    this.router.navigate(['/']);
  }

  // Allow only number keys and control keys in the phone input
  allowOnlyNumbers(event: KeyboardEvent): void {
    const allowed = ['Backspace','ArrowLeft','ArrowRight','Tab','Delete','Enter','Home','End'];
    const combo = (event.ctrlKey || event.metaKey) && /[acvx]/i.test(event.key);
    if (allowed.includes(event.key) || combo) return;
    if (event.key.length === 1 && !/^[0-9]$/.test(event.key)) event.preventDefault();
  }

  // Only allow pasting numbers into the phone input
  onPasteNumbersOnly(event: ClipboardEvent): void {
    const data = event.clipboardData?.getData('text') ?? '';
    const digits = data.replace(/\D/g, '');
    if (!digits) { event.preventDefault(); return; }
    event.preventDefault();
    const target = event.target as HTMLInputElement;
    const start = target.selectionStart ?? target.value.length;
    const end = target.selectionEnd ?? target.value.length;
    const newValue = target.value.slice(0, start) + digits + target.value.slice(end);
    target.value = newValue.slice(0, 8);
   // this.phone = target.value;
  }

  submit(): void {
    if (this.loading) return;
    this.errorMessage = '';

    const usernameTrimmed = (this.username ?? '').trim();
    const passwordValue = this.password ?? '';
    //const phoneDigits = (this.phone ?? '').replace(/\D/g, '');

    if (!usernameTrimmed) {
      this.errorMessage = 'Please enter a username.';
      return;
    }
    if (!passwordValue) {
      this.errorMessage = 'Please enter a password.';
      return;
    }
    /*
    if (phoneDigits.length !== 8) {
      this.errorMessage = 'Please enter an 8-digit phone number.';
      return;
    }
*/
    this.loading = true;
    this.http.post<{ token: string; role?: string }>(`${this.base}/admin/login`, {
      username: usernameTrimmed,
      password: passwordValue,
     // phone: phoneDigits
    
      // Hvis du senere vil sende landekoden med:
      // countryCode: this.selectedCountryCode !== 'Select Countrycode' ? this.selectedCountryCode : null
    }).subscribe({
      next: res => {
        localStorage.setItem('adminToken', res.token);
        this.loading = false;
        this.username = '';
        this.password = '';
       // this.phone = '';
       
        this.router.navigate(['/admin']);
      },
      error: (err) => {
        this.loading = false;
        const serverMsg = (typeof err?.error === 'string') ? err.error : (err?.error?.message || '');
        if (err?.status === 404) this.errorMessage = serverMsg || 'User not found.';
        else if (err?.status === 401) this.errorMessage = serverMsg || 'Incorrect credentials.';
        else if (err?.status === 409) {
          if (/phone|telefon/i.test(serverMsg)) {
            this.errorMessage = 'Phone number already exists!';
          } else {
            this.errorMessage = serverMsg || 'Login failed.';
          }
        } else this.errorMessage = serverMsg || 'Login failed.';
      }
    });
  }
}
