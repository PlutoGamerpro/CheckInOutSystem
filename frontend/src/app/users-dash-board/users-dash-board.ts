import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { FormsModule } from '@angular/forms'

@Component({
  selector: 'app-users-dash-board',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './users-dash-board.html',
  styleUrl: './users-dash-board.scss',
 // styleUrls: ['./users-dash-board.scss']
})
export class UsersDashBoard {
  users: any[] = [];
  originalRaw: any[] = [];
  loading = false;
  error = '';

  constructor(
    private http: HttpClient,
    private router: Router,
  ) {}

  ngOnInit(): void {
    this.load();
  }

  editUser: any = null;
  // Helpers de sessão
  get hasAdminToken(): boolean { return !!localStorage.getItem('adminToken'); }
  get hasManagerToken(): boolean { return !!localStorage.getItem('managerToken'); }
  get managerOnly(): boolean { return !this.hasAdminToken && this.hasManagerToken; }

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
    return { headers: new HttpHeaders({ 'X-Admin-Token': token }) };
  }

  saveEdit(): void {
    if (!this.editUser) return;
    const token = localStorage.getItem('adminToken') || localStorage.getItem('managerToken');
    if (!token) { this.error = 'Not authorized'; return; }
    // Manager não pode promover / alterar isManager
    if (this.managerOnly && this.editUser.isManager !== undefined && this.editUser.isManager !== false) {
      this.error = 'Somente admin pode definir isManager.';
      return;
    }
    this.loading = true;
    this.error = '';
    // Monta payload explícito para garantir envio de isManager
    const payload = {
      id: this.editUser.id,
      name: this.editUser.name,
      phone: this.editUser.phone,
      isAdmin: this.editUser.isAdmin,
      isManager: this.hasAdminToken ? this.editUser.isManager : undefined // manager não altera
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



  DeleteUser(id: number): void {
    if (this.loading) return;
    const token = localStorage.getItem('adminToken') || localStorage.getItem('managerToken');
    if (!token) { this.error = 'Not authorized'; return; }

    const user = this.users.find(u => u.id === id);
    const label = user ? `${user.name || ''} (ID: ${id})` : `ID: ${id}`;
    const ok = confirm(`Delete user ${label}?`);
    if (!ok) return;

    this.loading = true;
    this.error = '';
    this.http.delete(`${environment.baseApiUrl}/external/user/${id}`, this.adminHeaders).subscribe({
      next: () => this.load(),
      error: () => {
        this.error = 'Failed to delete user';
        this.loading = false;
      }
    });
  }

}
