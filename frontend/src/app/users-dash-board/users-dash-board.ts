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
  styleUrl: './users-dash-board.scss'
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

  startEdit(user: any): void {
    this.editUser = { ...user }; // clone to avoid direct mutation
  }

  cancelEdit(): void {
    this.editUser = null;
  }

  private get adminHeaders() {
    const token = localStorage.getItem('adminToken') || '';
    return { headers: new HttpHeaders({ 'X-Admin-Token': token }) };
  }

  saveEdit(): void {
    if (!this.editUser) return;
    const token = localStorage.getItem('adminToken');
    if (!token) { this.error = 'Not authorized'; return; }
    this.loading = true;
    this.error = '';
    // backend espera PUT /api/admin/user com o body UserRecordRequest
    this.http.put(`${environment.baseApiUrl}/admin/user`, this.editUser, this.adminHeaders).subscribe({
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
    const token = localStorage.getItem('adminToken');
    if (!token) { this.error = 'Not authorized'; return; }
    this.loading = true;
    this.error = '';
    this.http.delete(`${environment.baseApiUrl}/admin/user/${id}`, this.adminHeaders).subscribe({
      next: () => {
        this.load();
      },
      error: () => {
        this.error = 'Failed to delete user';
        this.loading = false;
      }
    });
  }

}

