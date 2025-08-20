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
  @ViewChild('signupForm') signupForm?: NgForm;
  name: string = '';
  phone: string = '';
  errorMessage: string = '';
  successMessage: string = '';
  loading = false;

  @ViewChild('nameInput') nameInput?: ElementRef<HTMLInputElement>;
  @ViewChild('phoneInput') phoneInput?: ElementRef<HTMLInputElement>;

  private nameRegex = /^[A-Za-zÀ-ÖØ-öø-ÿ]+(?:\s+[A-Za-zÀ-ÖØ-öø-ÿ'-]+)+$/;

  constructor(private http: HttpClient, private router: Router) {}
  

   // Allow only number keys and control keys in the phone input
 allowOnlyNumbers(event: KeyboardEvent): void {
    const allowedKeys = [
      'Backspace', 'ArrowLeft', 'ArrowRight', 'Tab', 'Delete', 'Enter', 'Home', 'End'
    ];
    const ctrlCombo =
      event.ctrlKey || event.metaKey
        ? ['a', 'c', 'v', 'x', 'A', 'C', 'V', 'X'].includes(event.key)
        : false;

    if (allowedKeys.includes(event.key) || ctrlCombo) return;

    if (event.key.length === 1) {
      const isDigit = /^[0-9]$/.test(event.key);
      if (!isDigit) event.preventDefault();
    }
  }

  allowOnlyLetters(event: KeyboardEvent): void {
      const allowedKeys = [
      'Backspace', 'ArrowLeft', 'ArrowRight', 'Tab', 'Delete', 'Enter', 'Home', 'End'
    ];
    const ctrlCombo =
      event.ctrlKey || event.metaKey
        ? ['a', 'c', 'v', 'x', 'A', 'C', 'V', 'X'].includes(event.key)
        : false;

    if (allowedKeys.includes(event.key) || ctrlCombo) return;

    if (event.key.length === 1) {
      const isAllowedChar = /^[A-Za-zÀ-ÖØ-öø-ÿ' -]$/.test(event.key);
      if (!isAllowedChar) event.preventDefault();
    }
  }

  // Only allow pasting numbers into the phone input
  onPasteNumbersOnly(event: ClipboardEvent): void {
    const data = event.clipboardData?.getData('text') ?? '';
    const digits = data.replace(/\D/g, '');
    if (digits.length === 0) {
      event.preventDefault();
      return;
    }
    event.preventDefault();
    const target = event.target as HTMLInputElement;
    const start = target.selectionStart ?? target.value.length;
    const end = target.selectionEnd ?? target.value.length;
    const newValue = target.value.slice(0, start) + digits + target.value.slice(end);
    target.value = newValue.slice(0, 8);
    this.phone = target.value;
  }

  // handles the login redirection
  goToLogin() {
    this.router.navigate(['/']);
  }

  private focusFirstInvalid() {
  
    if (!this.isValidName(this.name)) {
      this.nameInput?.nativeElement.focus();
      return;
    }
    const phoneDigits = (this.phone ?? '').replace(/\D/g, '');
    if (phoneDigits.length !== 8) {
      this.phoneInput?.nativeElement.focus();
    }
  }

  private isValidName(value: string): boolean {
    const v = (value ?? '').trim();
    return v.length > 0 && this.nameRegex.test(v);
  }

  // onsubmit creating a user 
  onSubmit() { 
    this.errorMessage = '';
    this.successMessage = '';

    const nameTrimmed = (this.name ?? '').trim();
    const phoneDigits = (this.phone ?? '').replace(/\D/g, '');
    this.phone = phoneDigits; 

    if (!this.isValidName(nameTrimmed)) {
      this.errorMessage = 'Fornavn & Efternavn skal udfyldes (indtast mindst to ord).';
      this.focusFirstInvalid();
      return;
    }

    if (phoneDigits.length !== 8) {
      this.errorMessage = 'Telefonnummer skal være 8 cifre.';
      this.focusFirstInvalid();
      return;
    }

    this.loading = true;
    // Use environment.baseApiUrl for the API endpoint
    this.http.post(`${environment.baseApiUrl}/user`, { name: nameTrimmed, phone: phoneDigits }).subscribe({
      next: () => {
        this.successMessage = 'Bruger oprettet!';
        this.loading = false;
         this.signupForm?.resetForm({ name: '', phone: '' }); // evita mensagens pós-sucesso
        this.name = '';
        this.phone = '';
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
          if (/name/i.test(raw)) {
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
