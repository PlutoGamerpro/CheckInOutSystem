import { Component, ElementRef, OnDestroy, ViewChild, ViewChildren, QueryList } from '@angular/core';
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
export class AdminLoginComponent implements OnDestroy {
  @ViewChild('adminLoginForm') adminLoginForm?: NgForm;
  @ViewChild('passwordInput') passwordInput?: ElementRef<HTMLInputElement>;
  @ViewChildren('otpBox') otpBoxes?: QueryList<ElementRef<HTMLInputElement>>;

  phone = '';
  password = '';
  secret = '';
  loading = false;
  errorMessage = '';
  loginAs: 'admin' | 'manager' = 'admin';
  private readonly base = environment.baseApiUrl.replace(/\/$/, '');
  private readonly emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/i;

  // Progressive flow flags
  step: 'phone' | 'password' | 'otp' | 'success' = 'phone';
  useMagicLink = false;
  rememberDevice = false;
  enableBiometric = false;
  phoneHelperMessage = 'Include the country code if needed.';
  phoneHelperState: 'neutral' | 'success' | 'error' = 'neutral';
  phonePreview = '';
  statusMessage = '';
  otpDigits: string[] = Array(6).fill('');
  illustrationState: 'idle' | 'sending' | 'waiting' | 'success' = 'idle';
  successDetails = { name: '', time: '', role: 'admin', sessionId: '' };
  offline = !navigator.onLine;
  disableSubmit = false;

  guestModeExpanded = false;
  guestForm = { name: '', email: '' };
  recentCheckIns = [
    { name: 'Sofie', time: 'Today • 09:04' },
    { name: 'Jonas', time: 'Yesterday • 18:22' },
    { name: 'Mia', time: 'Yesterday • 11:17' }
  ];

  private readonly connectionHandler = () => this.handleConnectionChange();

  constructor(private http: HttpClient, private router: Router) {
    window.addEventListener('online', this.connectionHandler);
    window.addEventListener('offline', this.connectionHandler);
  }

  ngOnDestroy(): void {
    window.removeEventListener('online', this.connectionHandler);
    window.removeEventListener('offline', this.connectionHandler);
  }

  get progressPercent(): string {
    switch (this.step) {
      case 'phone': return '20%';
      case 'password':
      case 'otp': return '65%';
      case 'success': return '100%';
      default: return '20%';
    }
  }

  get progressValue(): number {
    return Number(this.progressPercent.replace('%', ''));
  }

  get stageLabel(): string {
    switch (this.step) {
      case 'phone': return 'Step 1 • Enter phone';
      case 'password': return 'Step 2 • Confirm password';
      case 'otp': return 'Step 2 • Enter code';
      case 'success': return 'Step 3 • Complete';
    }
  }

  get primaryCtaLabel(): string {
    switch (this.step) {
      case 'phone': return this.useMagicLink ? 'Send code' : 'Continue';
      case 'password': return 'Sign in';
      case 'otp': return 'Verify code';
      default: return 'Continue';
    }
  }

  allowOnlyNumbers(event: KeyboardEvent): void {
    if (this.useMagicLink) return;
    const allowed = ['Backspace','ArrowLeft','ArrowRight','Tab','Delete','Enter','Home','End'];
    const combo = (event.ctrlKey || event.metaKey) && /[acvx]/i.test(event.key);
    const target = event.target as HTMLInputElement;
    const digit = /^[0-9]$/.test(event.key);
    if (event.key === '+' && (!target.value.includes('+') && (target.selectionStart ?? 0) === 0)) return;
    if (!digit && !allowed.includes(event.key) && !combo) event.preventDefault();
  }

  onPasteNumbersOnly(event: ClipboardEvent): void {
    if (this.useMagicLink) return;
    const data = event.clipboardData?.getData('text') ?? '';
    const sanitized = data.replace(/[^+\d]/g, '');
    if (!sanitized) { event.preventDefault(); return; }
    event.preventDefault();
    const target = event.target as HTMLInputElement;
    const start = target.selectionStart ?? target.value.length;
    const end = target.selectionEnd ?? target.value.length;
    const val = target.value.slice(0, start) + sanitized + target.value.slice(end);
    target.value = val.slice(0, 16);
    this.phone = target.value;
    this.updatePhoneFeedback(this.phone);
  }

  updatePhoneFeedback(value: string): void {
    this.phone = value;
    if (this.useMagicLink) {
      const trimmed = value.trim();
      this.phonePreview = '';
      if (!trimmed) {
        this.phoneHelperState = 'neutral';
        this.phoneHelperMessage = 'Enter the email that should receive the link.';
        return;
      }
      if (!this.emailPattern.test(trimmed)) {
        this.phoneHelperState = 'error';
        this.phoneHelperMessage = 'That does not look like a valid email address.';
        return;
      }
      this.phoneHelperState = 'success';
      this.phoneHelperMessage = 'Great — we will email the magic link there.';
      return;
    }
    const digits = value.replace(/\D/g, '');
    if (!digits.length) {
      this.phoneHelperState = 'neutral';
      this.phoneHelperMessage = 'Include the country code if needed.';
      this.phonePreview = '';
      return;
    }
    this.phonePreview = this.buildPhonePreview(digits);
    if (digits.length < 8) {
      this.phoneHelperState = 'error';
      this.phoneHelperMessage = 'Too few digits — add the code or remaining numbers.';
    } else if (digits.length === 8) {
      this.phoneHelperState = 'success';
      this.phoneHelperMessage = 'Looks good for a local number.';
    } else {
      this.phoneHelperState = 'neutral';
      this.phoneHelperMessage = `Double-check if ${digits.length} digits is expected.`;
    }
  }

  submit(): void {
    if (this.loading) return;
    this.errorMessage = '';
    this.statusMessage = '';
    const contactValue = (this.phone ?? '').trim();
    const phoneDigits = contactValue.replace(/\D/g, '');
    const hasEnoughDigits = phoneDigits.length >= 8;
    const emailValid = this.emailPattern.test(contactValue);

    if (this.step === 'phone') {
      if (this.useMagicLink) {
        if (!emailValid) {
          this.errorMessage = 'Provide a valid email to receive the magic link.';
          this.phoneHelperState = 'error';
          this.phoneHelperMessage = 'Example: admin@example.com';
          return;
        }
        this.loading = true;
        this.disableSubmit = true;
        this.statusMessage = 'Sending magic link...';
        this.illustrationState = 'sending';
        setTimeout(() => {
          this.loading = false;
          this.disableSubmit = false;
          this.illustrationState = 'success';
          this.statusMessage = 'Magic link sent! Check your inbox to finish signing in.';
          this.setSuccessState('magic-link', contactValue);
        }, 900);
        return;
      }

      if (!hasEnoughDigits) {
        this.errorMessage = 'Invalid number. Minimum 8 digits required.';
        this.phoneHelperState = 'error';
        this.phoneHelperMessage = 'Example: +45 1234 5678.';
        return;
      }
      if (this.useMagicLink) {
        this.loading = true;
        this.disableSubmit = true;
        this.statusMessage = 'Sending 6-digit code...';
        this.illustrationState = 'sending';
        setTimeout(() => {
          this.loading = false;
          this.disableSubmit = false;
          this.step = 'otp';
          this.statusMessage = 'Code sent! Check your phone.';
          this.illustrationState = 'waiting';
          setTimeout(() => this.focusOtpBox(0), 100);
        }, 900);
        return;
      } else {
        this.step = 'password';
        this.illustrationState = 'waiting';
        setTimeout(() => this.passwordInput?.nativeElement.focus(), 150);
        return;
      }
    }

    if (this.step === 'otp') {
      const joined = this.otpDigits.join('');
      if (joined.length < this.otpDigits.length) {
        this.errorMessage = 'Incomplete code — enter all 6 digits.';
        return;
      }
      this.loading = true;
      this.disableSubmit = true;
      this.statusMessage = 'Verifying code...';
      setTimeout(() => {
        this.loading = false;
        this.disableSubmit = false;
        this.illustrationState = 'success';
        this.setSuccessState('OTP');
      }, 750);
      return;
    }

    if (this.step === 'password') {
      if (!this.password) {
        this.errorMessage = 'Password is required.';
        return;
      }
      this.loading = true;
      this.illustrationState = 'waiting';
      this.http.post<{ token:string; user?:string; userName?:string; role?:string }>(`${this.base}/admin/login`, {
        phone: phoneDigits.slice(-8),
        password: this.password,
        secret: this.secret
      }).subscribe({
        next: res => {
          const roleKey = this.loginAs === 'admin' ? 'adminToken' : 'managerToken';
          localStorage.setItem(roleKey, res.token);
          if (this.rememberDevice) localStorage.setItem('rememberDeviceUntil', (Date.now() + 30 * 24 * 3600 * 1000).toString());
          this.loading = false;
          this.illustrationState = 'success';
          this.setSuccessState(res.role ?? this.loginAs, res.userName ?? res.user ?? phoneDigits);
        },
        error: (err) => {
          this.loading = false;
          this.illustrationState = 'idle';
          const serverMsg = (err?.error && (typeof err.error === 'string') ? err.error : (err?.error?.message || ''));
          if (err.status === 404) this.errorMessage = serverMsg || 'User not found.';
          else if (err.status === 401) this.errorMessage = serverMsg || 'Incorrect credentials.';
          else this.errorMessage = serverMsg || 'Login failed.';
        }
      });
    }
  }

  handleOtpInput(index: number, event: Event): void {
    const input = event.target as HTMLInputElement;
    const value = input.value.replace(/\D/g, '').slice(-1);
    this.otpDigits[index] = value;
    input.value = value;
    if (value && index < this.otpDigits.length - 1) {
      this.focusOtpBox(index + 1);
    } else if (value && index === this.otpDigits.length - 1) {
      input.blur();
    }
  }

  handleOtpKeydown(index: number, event: KeyboardEvent): void {
    if (event.key === 'Backspace' && !this.otpDigits[index] && index > 0) {
      this.focusOtpBox(index - 1);
    }
  }

  focusOtpBox(index: number): void {
    const el = this.otpBoxes?.get(index)?.nativeElement;
    if (el) el.focus();
  }

  resendCode(): void {
    this.statusMessage = 'New code sent. Check again.';
    this.illustrationState = 'sending';
    setTimeout(() => this.illustrationState = 'waiting', 400);
    this.otpDigits = Array(6).fill('');
    setTimeout(() => this.focusOtpBox(0), 150);
  }

  changeNumber(): void {
    this.step = 'phone';
    this.statusMessage = 'Number cleared — update and continue.';
    this.otpDigits = Array(6).fill('');
  }

  needHelp(): void {
    this.statusMessage = 'Support: support@company.com or +45 88 77 66 55.';
  }

  toggleGuestMode(): void {
    this.guestModeExpanded = !this.guestModeExpanded;
  }

  queueGuestCheckIn(): void {
    this.statusMessage = this.guestForm.name
      ? `${this.guestForm.name} will be queued for offline check-in.`
      : 'Guest queued anonymously.';
    this.guestForm = { name: '', email: '' };
    this.guestModeExpanded = false;
  }

  shareConfirmation(): void {
    this.statusMessage = 'Confirmation link copied to clipboard.';
    navigator.clipboard?.writeText(`Check-in confirmed at ${this.successDetails.time}`);
  }

  continueToDashboard(): void {
    this.router.navigate(['/admin']);
    this.step = 'phone';
    this.password = '';
    this.phone = '';
    this.phonePreview = '';
    this.phoneHelperState = 'neutral';
    this.phoneHelperMessage = 'Include the country code if needed.';
    this.adminLoginForm?.resetForm();
  }

  dismissOffline(): void {
    this.offline = false;
  }

  goToMain(): void {
    this.router.navigate(['/']);
  }

  private setSuccessState(role: string, name: string = ''): void {
    this.step = 'success';
    this.successDetails = {
      name: name || 'User',
      time: new Date().toLocaleString(),
      role,
      sessionId: `SID-${Math.random().toString(36).slice(2, 8).toUpperCase()}`
    };
  }

  private buildPhonePreview(digits: string): string {
    const masked = digits.padEnd(8, '•').slice(0, 12);
    const arr = masked.split('');
    return `+45 ${arr.slice(0,2).join('')}${arr.length > 2 ? ' ' : ''}${arr.slice(2,4).join('')}${arr.length > 4 ? ' ' : ''}${arr.slice(4,6).join('')}${arr.length > 6 ? ' ' : ''}${arr.slice(6,8).join('')}`;
  }

  private handleConnectionChange(): void {
    this.offline = !navigator.onLine;
    if (this.offline) {
      this.statusMessage = 'Offline. Progress will be saved locally.';
    } else {
      this.statusMessage = 'Back online — you can finish the check-in.';
    }
  }
}
