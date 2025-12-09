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
  }

  cancelEdit(): void {
    this.editUser = null;
  }

  private get adminHeaders() {
    const token = localStorage.getItem('adminToken')
      || localStorage.getItem('managerToken')
      || '';
    return { headers: new HttpHeaders({ 'Authorization': `Bearer ${token}`, 'X-Admin-Token': token }) };
  }




saveEdit(): void {

    if (!this.editUser) return;
    this.loading = true;
    this.error = '';

const payload = {
  Id: this.editUser.id,
  Name: this.editUser.name,
  Phone: this.editUser.phone,
  CountryCode: this.editUser.countryCode,
  IsAdmin: this.editUser.isAdmin,
};

this.adminservice.updateUser(payload).subscribe({
      next: () => {
        this.editUser = null;
        this.load();
      },
      error: () => {
        this.error = 'Failed to update user';
        this.loading = false;
      }
    });
   }
/*
  saveEdit(): void {
    if (!this.editUser) return;
    const token = localStorage.getItem('adminToken') 
    this.loading = true;
    this.error = '';
 
    const payload = {
      id: this.editUser.id,
      name: this.editUser.name,
      phone: this.editUser.phone,
      countryCode: this.editUser.countryCode,
      isAdmin: this.editUser.isAdmin,
  
    };
    this.http.put(`${environment.baseApiUrl}/external/user`, payload, this.adminHeaders).subscribe({
      next: () => {
        this.editUser = null;
        this.load();
      },
      error: () => {
        this.error = 'Failed to update user';
        this.loading = false;
      }
    });
  }
*/
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
    this.labelTextToDisplay = (`DELETE user ${label}? No way to undo! after actions done`);
    // Modal åbnes via data-bs-toggle - sletning sker først i DeleteUserPost() efter bekræftelse
  }

  onAdminRoleChange(event: any): void {
    if (!this.editUser) return;
    
    // Store the pending change and original value
    this.pendingAdminRoleChange = event.target.value === 'true';
    this.originalAdminRole = !this.pendingAdminRoleChange;
    
    // Reset OTP fields
    this.otpCode = ['', '', '', '', '', ''];
    this.otpError = '';
    
    // Open modal using Bootstrap's modal API
    const modalElement = document.getElementById('adminRoleModal');
    if (modalElement) {
      const modal = new (window as any).bootstrap.Modal(modalElement);
      modal.show();
      
      // Focus first input after modal is shown
      setTimeout(() => {
        const inputs = this.otpInputs?.toArray();
        if (inputs && inputs.length > 0) {
          inputs[0].nativeElement.focus();
        }
      }, 300);
    }
  }

  onOtpInput(event: any, index: number): void {
    const input = event.target as HTMLInputElement;
    const value = input.value;
    
    if (value.length > 0) {
      this.otpCode[index] = value[value.length - 1];
      input.value = this.otpCode[index];
      
      // Move to next input
      if (index < 5) {
        const inputs = this.otpInputs.toArray();
        inputs[index + 1].nativeElement.focus();
      }
    }
    this.otpError = '';
  }

  onOtpKeydown(event: KeyboardEvent, index: number): void {
    const input = event.target as HTMLInputElement;
    
    // Handle backspace
    if (event.key === 'Backspace') {
      if (input.value === '' && index > 0) {
        const inputs = this.otpInputs.toArray();
        inputs[index - 1].nativeElement.focus();
      }
      this.otpCode[index] = '';
    }
    
    // Handle paste
    if (event.key === 'v' && (event.ctrlKey || event.metaKey)) {
      event.preventDefault();
      navigator.clipboard.readText().then(text => {
        const digits = text.replace(/\D/g, '').slice(0, 6).split('');
        const inputs = this.otpInputs.toArray();
        digits.forEach((digit, i) => {
          if (i < 6) {
            this.otpCode[i] = digit;
            inputs[i].nativeElement.value = digit;
          }
        });
        if (digits.length > 0) {
          const lastIndex = Math.min(digits.length - 1, 5);
          inputs[lastIndex].nativeElement.focus();
        }
      });
    }
  }

  verifyAndSaveAdminRole(): void {
    const enteredCode = this.otpCode.join('');
    
    if (enteredCode !== this.ADMIN_CODE) {
      this.otpError = 'Invalid verification code';
      return;
    }
    
    // Code is correct, apply the change
    if (this.editUser && this.pendingAdminRoleChange !== null) {
      this.editUser.isAdmin = this.pendingAdminRoleChange;
    }
    
    // Close modal
    const modalElement = document.getElementById('adminRoleModal');
    if (modalElement) {
      const modal = (window as any).bootstrap.Modal.getInstance(modalElement);
      if (modal) modal.hide();
    }
    
    // Reset
    this.pendingAdminRoleChange = null;
    this.originalAdminRole = null;
    this.otpCode = ['', '', '', '', '', ''];
    this.otpError = '';
  }

  cancelAdminRoleChange(): void {
    // Revert the change
    if (this.editUser && this.originalAdminRole !== null) {
      this.editUser.isAdmin = this.originalAdminRole;
    }
    
    // Reset
    this.pendingAdminRoleChange = null;
    this.originalAdminRole = null;
    this.otpCode = ['', '', '', '', '', ''];
    this.otpError = '';
  }

}
