import { Component, ViewChildren, QueryList, ElementRef } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { FormsModule } from '@angular/forms'
import { Adminservice} from '../shared/services/adminservice';

@Component({
  selector: 'app-users-dash-board',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './users-dash-board.html',
  styleUrls: ['./users-dash-board.scss', './otp-modal.scss']
})
export class UsersDashBoard {
    showPassword: boolean = false;
    showPasswordConfirm: boolean = false;
    eyePop: boolean = false;
    eyePopConfirm: boolean = false;
    toggleShowPassword(): void {
      this.showPassword = !this.showPassword;
      this.eyePop = true;
      setTimeout(() => this.eyePop = false, 180);
    }

    toggleShowPasswordConfirm(): void {
      this.showPasswordConfirm = !this.showPasswordConfirm;
      this.eyePopConfirm = true;
      setTimeout(() => this.eyePopConfirm = false, 180);
    }
  @ViewChildren('otp0, otp1, otp2, otp3, otp4, otp5') otpInputs!: QueryList<ElementRef<HTMLInputElement>>;
  
  PhoneCountryCode = [
    { code: '+45', country: 'Denmark' },
    { code: '+1', country: 'USA' },
    { code: '+44', country: 'UK' },
    { code: '+49', country: 'Germany' },
    { code: '+33', country: 'France' }
  ];
  users: any[] = [];
  originalRaw: any[] = [];
  loading = false;
  error = '';
  //labelDeleteUser = ''; // should not be here but use the one below
  labelTextToDisplay = '';
  selectedUserId: number | null = null;
  
  // OTP verification for admin role change
  otpCode: string[] = ['', '', '', '', '', ''];
  otpError = '';
  private readonly ADMIN_CODE = '123456'; // Hardcoded verification code
  pendingAdminRoleChange: boolean | null = null;
  originalAdminRole: boolean | null = null;
  adminRoleWasChanged = false; // Track if admin role was actually changed


  constructor(
    private http: HttpClient,
    private router: Router,
    private adminservice: Adminservice
  ) {}

  ngOnInit(): void {
    const token = localStorage.getItem('adminToken') || localStorage.getItem('managerToken');
    if (!token) { this.error = 'Not authorized'; return; }


    this.load();
  }

  editUser: any = null;
  // Helpers de sessão
  get hasAdminToken(): boolean { return !!localStorage.getItem('adminToken'); }
  ///get hasManagerToken(): boolean { return !!localStorage.getItem('managerToken'); }
  //get managerOnly(): boolean { return !this.hasAdminToken && this.hasManagerToken; }

  startEdit(user: any): void {
    this.editUser = { ...user }; // clone to avoid direct mutation
    this.originalAdminRole = user.isAdmin; // Store original admin role
    this.adminRoleWasChanged = false;
  }

  cancelEdit(): void {
    this.editUser = null;
    this.originalAdminRole = null;
    this.adminRoleWasChanged = false;
    this.pendingAdminRoleChange = null;
    this.clearOtpState();
  }

  private get adminHeaders() {
    const token = localStorage.getItem('adminToken')
      || localStorage.getItem('managerToken')
      || '';
    return { headers: new HttpHeaders({ 'Authorization': `Bearer ${token}`, 'X-Admin-Token': token }) };
  }




saveEdit(): void {
    if (!this.editUser) return;

    // Check if admin role was changed
    if (this.editUser.isAdmin !== this.originalAdminRole) {
      this.adminRoleWasChanged = true;
      this.pendingAdminRoleChange = this.editUser.isAdmin;
      // Clear and show modal
      this.clearOtpState();
      this.showAdminRoleModal();
      return; // Don't save yet, wait for OTP verification
    }

    // No admin change, save directly
    this.performSave();
  }

  private showAdminRoleModal(): void {
    const modalElement = document.getElementById('adminRoleModal');
    if (modalElement) {
      // Clear all OTP input fields and state before showing
      this.clearOtpState();
      
      const modal = new (window as any).bootstrap.Modal(modalElement, { backdrop: 'static' });
      modal.show();
      
      // Focus first OTP input
      setTimeout(() => {
        const inputs = this.otpInputs?.toArray();
        if (inputs && inputs.length > 0) {
          inputs[0].nativeElement.focus();
        }
      }, 300);
    }
  }

  private clearOtpState(): void {
    // Clear internal array
    this.otpCode = ['', '', '', '', '', ''];
    this.otpError = '';
    
    // Clear DOM inputs
    const inputs = this.otpInputs?.toArray();
    if (inputs) {
      inputs.forEach(input => {
        input.nativeElement.value = '';
        input.nativeElement.blur();
      });
    }
  }

  setPasswordValue: string = '';
  setPasswordConfirm: string = '';
  setPasswordError: string = '';
  // Password rule flags
  passwordHasMinLength: boolean = false;
  passwordHasUpper: boolean = false;
  passwordHasLower: boolean = false;
  passwordHasDigit: boolean = false;
  passwordHasSpecial: boolean = false;

  openSetPasswordModal(): void {
    this.setPasswordValue = '';
    this.setPasswordConfirm = '';
    this.setPasswordError = '';
    this.resetPasswordRuleFlags();
    // @ts-ignore
    window.bootstrap?.Modal.getOrCreateInstance(document.getElementById('setPasswordModal')).show();
  }

  closeSetPasswordModal(): void {
    this.setPasswordValue = '';
    this.setPasswordConfirm = '';
    this.setPasswordError = '';
    this.resetPasswordRuleFlags();
    // @ts-ignore
    window.bootstrap?.Modal.getOrCreateInstance(document.getElementById('setPasswordModal')).hide();
  }

  saveSetPasswordModal(): void {
    // Password rules
    const pwd = this.setPasswordValue || '';
    const confirm = this.setPasswordConfirm || '';
    this.onPasswordInput();
    if (!pwd) {
      this.setPasswordError = 'Password is required.';
      return;
    }
    if (!confirm) {
      this.setPasswordError = 'Please repeat the password.';
      return;
    }
    if (pwd !== confirm) {
      this.setPasswordError = 'Passwords do not match.';
      return;
    }
    if (!this.passwordHasMinLength || !this.passwordHasUpper || !this.passwordHasLower || !this.passwordHasDigit || !this.passwordHasSpecial) {
      this.setPasswordError = 'Password does not meet all requirements.';
      return;
    }
    // Tilføj password til editUser og gem
    if (this.editUser) {
      this.editUser.password = pwd;
    }
    this.closeSetPasswordModal();
    this.performSave();
  }

  onPasswordInput(): void {
    const pwd = this.setPasswordValue || '';
    this.passwordHasMinLength = pwd.length >= 10;
    this.passwordHasUpper = /[A-Z]/.test(pwd);
    this.passwordHasLower = /[a-z]/.test(pwd);
    this.passwordHasDigit = /[0-9]/.test(pwd);
    this.passwordHasSpecial = /[^A-Za-z0-9]/.test(pwd);
  }

  private resetPasswordRuleFlags(): void {
    this.passwordHasMinLength = false;
    this.passwordHasUpper = false;
    this.passwordHasLower = false;
    this.passwordHasDigit = false;
    this.passwordHasSpecial = false;
  }

  private performSave(): void {
    this.loading = true;
    this.error = '';

    const payload: any = {
      Id: this.editUser.id,
      Name: this.editUser.name,
      Phone: this.editUser.phone,
      CountryCode: this.editUser.countryCode,
      Password: this.editUser.password, // MAYBE ERROR ADDED HERE... CHECK PUT REQUEST
      IsAdmin: this.editUser.isAdmin,
    };
    // Tilføj password hvis sat
    if (this.editUser.password) {
      payload.Password = this.editUser.password;
    }

    this.adminservice.updateUser(payload).subscribe({
      next: () => {
        this.editUser = null;
        this.adminRoleWasChanged = false;
        this.load();
      },
      error: () => {
        this.error = 'Failed to update user';
        this.loading = false;
      }
    });
  }

  load(): void {
    this.loading = true;
    this.error = '';
    const url = `${environment.baseApiUrl}/user`;
    this.http.get<any[]>(url).subscribe({
      next: (raw: any[]) => {
        this.originalRaw = raw;
        this.users = raw; // You can normalize if needed
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load users';
        this.loading = false;
      }
    });
  }

  Home(): void {
    this.router.navigate(['/']);
  }
  Registrations(): void {
    this.router.navigate(['/admin']);
  }

  DeleteUserPost(id: number){

  const token = localStorage.getItem('adminToken') /*|| localStorage.getItem('managerToken')*/;
    if (!token) { this.error = 'Not authorized'; return; }

    if (this.loading) return;
 
    const user = this.users.find(u => u.id === id);
       this.selectedUserId = id;
    this.loading = true;
    this.error = '';


  
    this.adminservice.DeleteUser(id).subscribe({
      next: () => this.load(),
      error: () => {
        this.error = 'Failed to delete user';
        this.loading = false;
      }
    });
    
  }

  DeleteUser(id: number): void {
    if (this.loading) return;
   // const token = localStorage.getItem('adminToken') || localStorage.getItem('managerToken');
    //if (!token) { this.error = 'Not authorized'; return; }

    const user = this.users.find(u => u.id === id);
    const label = user ? `${user.name || ''} (ID: ${id})` : `ID: ${id}`;
    this.labelTextToDisplay = `Are you sure you want to permanently delete this user?\n\n${label}\n\nThis action cannot be undone.`;
    // Modal åbnes via data-bs-toggle - sletning sker først i DeleteUserPost() efter bekræftelse
  }

  onAdminRoleChange(event: any): void {
    // Just track the dropdown change, don't show modal yet
    // Modal will appear when Save is clicked if role was actually changed
  }

  onOtpInput(event: any, index: number): void {
    const input = event.target as HTMLInputElement;
    let value = input.value;
    
    // Keep only digits
    value = value.replace(/\D/g, '');
    
    if (value.length > 0) {
      // Take only the last digit entered
      const digit = value[value.length - 1];
      input.value = digit;
      this.otpCode[index] = digit;
      
      // Move to next input if this one is filled
      if (index < 5) {
        const inputs = this.otpInputs.toArray();
        inputs[index + 1].nativeElement.focus();
      }
    } else {
      input.value = '';
      this.otpCode[index] = '';
    }
    
    this.otpError = '';
  }

  onOtpKeydown(event: KeyboardEvent, index: number): void {
    const input = event.target as HTMLInputElement;
    const inputs = this.otpInputs.toArray();
    
    // Handle backspace
    if (event.key === 'Backspace') {
      event.preventDefault();
      input.value = '';
      this.otpCode[index] = '';
      
      // Move focus to previous field
      if (index > 0) {
        inputs[index - 1].nativeElement.focus();
      }
      return;
    }
    
    // Handle paste (Ctrl+V or Cmd+V)
    if ((event.ctrlKey || event.metaKey) && event.key.toLowerCase() === 'v') {
      event.preventDefault();
      navigator.clipboard.readText().then(text => {
        const digits = text.replace(/\D/g, '').slice(0, 6);
        for (let i = 0; i < digits.length && i < 6; i++) {
          inputs[i].nativeElement.value = digits[i];
          this.otpCode[i] = digits[i];
        }
        // Focus last filled input
        const lastIndex = Math.min(digits.length - 1, 5);
        if (inputs[lastIndex]) {
          inputs[lastIndex].nativeElement.focus();
        }
      }).catch(() => {
        // Clipboard read failed - silently ignore
      });
    }
  }

  verifyAndSaveAdminRole(): void {
    // Read OTP directly from DOM to ensure we get current values
    const inputs = this.otpInputs?.toArray() || [];
    const enteredCode = inputs.map(inp => inp.nativeElement.value).join('');
    
    console.log('OTP Verification:', { enteredCode, expected: this.ADMIN_CODE });
    
    if (enteredCode.length !== 6) {
      this.otpError = 'Please enter all 6 digits';
      return;
    }
    
    if (enteredCode !== this.ADMIN_CODE) {
      this.otpError = 'Invalid verification code';
      return;
    }
    
    // Code is correct, close modal and proceed
    const modalElement = document.getElementById('adminRoleModal');
    if (modalElement) {
      const modal = (window as any).bootstrap.Modal.getInstance(modalElement);
      if (modal) modal.hide();
    }
    this.clearOtpState();

    // Hvis vi deranker (admin -> none), så fjern password og gem direkte
    if (
      this.editUser &&
      this.originalAdminRole === true &&
      this.editUser.isAdmin === false
    ) {
      delete this.editUser.password;
      this.closeSetPasswordModal();
      this.performSave();
      return;
    }

    // Hvis vi promoverer til admin, og password allerede er sat, så gem direkte uden at spørge
    if (
      this.editUser &&
      this.originalAdminRole === false &&
      this.editUser.isAdmin === true &&
      this.editUser.password && this.editUser.password.length > 0
    ) {
      this.performSave();
      return;
    }

    // Hvis vi promoverer til admin og password ikke er sat, så vis password-modal
    this.openSetPasswordModal();
  }

  cancelAdminRoleChange(): void {
    // Revert the change in the UI
    if (this.editUser && this.originalAdminRole !== null) {
      this.editUser.isAdmin = this.originalAdminRole;
    }
    
    // Reset all state
    this.adminRoleWasChanged = false;
    this.pendingAdminRoleChange = null;
    this.clearOtpState();
  }

}
