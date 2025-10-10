import { Component, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { HttpClientModule } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';
import { CheckinService } from '../shared/services/checkin.service';

// cspell:words Indtast Telefonnummer Telefonnummeret checkin Checkin checket opstod fejl Prøv igen byphone logget eksisterer ikke systemet cifre

// English: Backend response shapes
interface StatusResponse {
  isCheckedIn: boolean;
}
interface ActionResponse {
  name?: string;
  phone?: string;
}

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule, HttpClientModule],
  templateUrl: './login.html',
  styleUrls: ['./login.scss'],
})
export class Login {
  @ViewChild('loginForm') loginForm?: NgForm;
  private readonly apiBase = environment.baseApiUrl;

  phone = '';
  message = '';
  userName = '';
  checkInTime?: string;
  checkOutTime?: string;
  isCheckedIn = false;
  loading = false;

  constructor(
    private http: HttpClient,
    private router: Router,
    private checkinService: CheckinService
  ) {}

  // English: Allow only digits & control/navigation keys
  allowOnlyNumbers(event: KeyboardEvent): void {
    const allowedKeys = [
      'Backspace',
      'ArrowLeft',
      'ArrowRight',
      'Tab',
      'Delete',
      'Enter',
      'Home',
      'End',
    ];
    const ctrlCombo =
      (event.ctrlKey || event.metaKey) &&
      ['a', 'c', 'v', 'x', 'A', 'C', 'V', 'X'].includes(event.key);
    const isNumberKey = event.key >= '0' && event.key <= '9';
    if (!isNumberKey && !allowedKeys.includes(event.key) && !ctrlCombo) {
      event.preventDefault();
    }
  }

  // English: Force pasted content to digits only (max 8)
  onPasteNumbersOnly(event: ClipboardEvent): void {
    const data = event.clipboardData?.getData('text') ?? '';
    const digits = data.replace(/\D/g, '');
    if (!digits) {
      event.preventDefault();
      return;
    }
    event.preventDefault();
    const target = event.target as HTMLInputElement;
    const start = target.selectionStart ?? target.value.length;
    const end = target.selectionEnd ?? target.value.length;
    const newValue =
      target.value.slice(0, start) + digits + target.value.slice(end);
    target.value = newValue.slice(0, 8);
    this.phone = target.value;
  }

  // English: Entry point toggling check-in / check-out
  checkInOrOut(): void {
    if (this.loading) return;
    this.phone = (this.phone ?? '').replace(/\D/g, '');
    if (!this.phone) {
      this.message = 'Indtast telefonnummer';
      return;
    }
    if (!/^\d{8}$/.test(this.phone)) {
      this.message = 'Telefonnummer skal være 8 cifre';
      return;
    }

    this.loading = true;
    this.message = '';

    this.checkinService.getStatus(this.phone).subscribe({
      next: (res: StatusResponse) => this.onStatusLoaded(res),
      error: (err: unknown) => {
        this.setErrorMessage(err, 'Der opstod en fejl. Prøv igen senere.');
        this.loading = false;
      },
    });
  }

  // Compute dynamic button label used by the template
  get actionLabel(): string {
    return this.isCheckedIn ? 'Check ud' : 'Check ind';
  }

  // Compute status chip text used by the template
  get statusText(): string {
    if (this.loading) return 'Opdaterer status…';
    if (this.isCheckedIn) return 'Du er aktuelt checket ind.';
    if (this.checkOutTime) return 'Du er logget ud.';
    return 'Klar til at checke ind.';
  }

  // Centralized status handling to reduce conditional complexity in subscribe
  private onStatusLoaded(res: StatusResponse): void {
    this.isCheckedIn = res.isCheckedIn;
    if (res.isCheckedIn) {
      this.performCheckout();
    } else {
      this.performCheckin();
    }
  }

  // English: Performs check-in
  private performCheckin(): void {
    this.checkinService.checkinByPhone(this.phone).subscribe({
      next: (inRes: ActionResponse) => {
        // se backend não enviar name por algum motivo, usar phone ou texto padrão
        this.userName = inRes.name ?? inRes.phone ?? 'Bruger';
        this.checkInTime = new Date().toISOString();
        this.checkOutTime = undefined;
        this.isCheckedIn = true;
        this.message = `Du er logget ind, ${this.userName}!`;
        this.afterActionReset();
      },
      error: (err: unknown) => {
        this.setErrorMessage(err, 'Der opstod en fejl. Prøv igen.');
        this.loading = false;
      },
    });
  }

  // English: Performs check-out
  private performCheckout(): void {
    this.checkinService.checkoutByPhone(this.phone).subscribe({
      next: (outRes: ActionResponse) => {
        const name = outRes.name ?? outRes.phone ?? 'Bruger';
        this.message = `Du er nu checket ud, ${name}!`;
        this.checkOutTime = new Date().toISOString();
        this.checkInTime = undefined;
        this.isCheckedIn = false;
        this.afterActionReset();
      },
      error: () => {
        this.message = 'Der opstod en fejl ved check-out.';
        this.loading = false;
      },
    });
  }

  // English: Helper to compose full URL
  private buildUrl(path: string): string {
    // Ensure no double slashes
    return `${this.apiBase.replace(/\/$/, '')}/${path.replace(/^\//, '')}`;
  }

  // English: Common cleanup after success
  private afterActionReset(): void {
    this.phone = '';
    this.loading = false;
    this.loginForm?.resetForm({ phone: '' });
  }

  // English: Unified error handling
  private setErrorMessage(err: unknown, fallback: string): void {
    // Narrowing minimal: treat as any for status access
    const status = (err as any)?.status;
    if (status === 404) {
      this.message = 'Telefonnummeret eksisterer ikke i systemet';
    } else {
      this.message = fallback;
    }
  }

  goToSignup(): void {
    this.router.navigate(['/signup']);
  }

  // Home navigation used by the template
  goHome(): void {
    this.router.navigate(['/']);
  }
}
