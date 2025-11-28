import { Component, ElementRef, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { NgForm } from '@angular/forms';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './signup.html',
  styleUrls: ['./signup.scss']
})
export class Signup {


  PhoneCountryCode = [
    { code: '+45', country: 'Denmark' },
    { code: '+1', country: 'USA' },
    { code: '+44', country: 'UK' },
    { code: '+49', country: 'Germany' },
    { code: '+33', country: 'France' }
  ];
  
  countryCode: string = 'Select Countrycode'; // Default til Danmark
  isDropdownOpen = false;
  selectedCountryCode = 'Select Countrycode'; // Initialize with default value

  toggleDropdown() {
    this.isDropdownOpen = !this.isDropdownOpen;
}

selectItem(item: any) {
  this.selectedCountryCode = item.code;
  this.isDropdownOpen = false;
}


  @ViewChild('signupForm') signupForm?: NgForm;
  username: string = '';
  phone: string = '';
  CountryCode: string = 'Select Countrycode';
  errorMessage: string = '';
  successMessage: string = '';
  loading = false;

  @ViewChild('usernameInput') usernameInput?: ElementRef<HTMLInputElement>;
  @ViewChild('phoneInput') phoneInput?: ElementRef<HTMLInputElement>;


  constructor(private http: HttpClient, private router: Router) {}
  

  // handles the login redirection
  goToLogin() {
    this.router.navigate(['/']);
  }

  private focusFirstInvalid() {
    if (!this.username || this.username.trim().length === 0) {
      this.usernameInput?.nativeElement.focus();
      return;
    }
    const digits = (this.phone ?? '').replace(/\D/g, '');
    if (digits.length !== 8) {
      this.phoneInput?.nativeElement.focus();
    }
    const countryCodeValid = this.selectedCountryCode !== 'Select Countrycode';
    if (!countryCodeValid) {
      this.phoneInput?.nativeElement.focus();
    }
  }

  // Allow only letters, spaces, hyphen and apostrophe in the name field
  allowOnlyLetters(event: KeyboardEvent): void {
    const allowed = ['Backspace','ArrowLeft','ArrowRight','Tab','Delete','Enter','Home','End'];
    const combo = (event.ctrlKey || event.metaKey) && /[acvx]/i.test(event.key);
    if (allowed.includes(event.key) || combo) return;
    if (event.key.length === 1 && !/^[A-Za-zÀ-ÖØ-öø-ÿ' -]$/.test(event.key)) event.preventDefault();
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
    this.phone = target.value;
  }

  // onsubmit creating a user 
  onSubmit() { 
    this.errorMessage = '';
    this.successMessage = '';

    const usernameTrimmed = (this.username ?? '').trim();
    const phoneDigits = (this.phone ?? '').replace(/\D/g, '');
    const countryCode = this.selectedCountryCode !== 'Select Countrycode' ? this.selectedCountryCode : '';

    const nameRegex = /^[A-Za-zÀ-ÖØ-öø-ÿ]+(?:\s+[A-Za-zÀ-ÖØ-öø-ÿ'-]+)+$/;
    if (!usernameTrimmed || !nameRegex.test(usernameTrimmed)) {
      this.errorMessage = 'First and last name are required (at least two words).';
      this.focusFirstInvalid();
      return;
    }
    if (phoneDigits.length !== 8) {
      this.errorMessage = 'Phone number must be 8 digits.';
      this.focusFirstInvalid();
      return;
    }
    if(!countryCode) {
      this.errorMessage = 'Please select a country code.';
      this.focusFirstInvalid();
      return;
    }

    this.loading = true;
    // Use environment.baseApiUrl for the API endpoint
    this.http.post(`${environment.baseApiUrl}/user`, { name: usernameTrimmed, phone: phoneDigits, countryCode: countryCode }).subscribe({
      next: () => {
        this.successMessage = 'User created!';
        this.loading = false;
        this.signupForm?.resetForm({ username: '', phone: '', countryCode: '',   });
        this.username = '';
        this.phone = '';
        this.selectedCountryCode = '';
        setTimeout(() => {
          this.router.navigate(['/']);
        }, 1000);
      },
      error: (err) => {
        this.loading = false;
        if (err?.status === 409) {
          const raw = typeof err.error === 'string'
            ? err.error
            : (err.error?.message || JSON.stringify(err.error || ''));
          if (/name|username/i.test(raw)) {
            this.errorMessage = 'Navnet eksisterer allerede!';
          } else if (/phone|telefon/i.test(raw)) {
            this.errorMessage = 'Telefonnummeret eksisterer allerede!';
          } else {
            this.errorMessage = 'Navn eller telefon eksisterer allerede!';
          }
        } else {
          this.errorMessage = 'Der opstod en fejl. Prøv igen.';
        }
      }
    });
  }
}
