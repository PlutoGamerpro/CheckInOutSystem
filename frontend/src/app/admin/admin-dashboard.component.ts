import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';

interface AdminReg {
  id: number | string;
  userName: string | null;
  phone: string | null;
  checkIn: string | null;
  checkOut: string | null;
  isOpen: boolean;
  isAdmin?: boolean; // novo
}

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, DatePipe],
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.scss']
})
export class AdminDashboardComponent implements OnInit {
  registrations: AdminReg[] = [];
  originalRaw: any[] = [];
  loading = false;
  error = '';
  debug = false;
  networkDown = false;
  lastUrl = ''; // removido 'private' para acesso no template
  private readonly base = environment.baseApiUrl.replace(/\/$/, '');
  includeAdmins = true; // definir false para excluir admins

  constructor(private http: HttpClient, private router: Router) {}

  ngOnInit(): void {
    const token = localStorage.getItem('adminToken');
    if (!token) { this.router.navigate(['/admin-login']); return; }
    this.load();
  }

  get allFieldsEmpty(): boolean {
    return this.registrations.length > 0 &&
      this.registrations.every(r => !r.userName && !r.phone && !r.checkIn && !r.checkOut);
  }

  private pickFirst(values: any[]): any {
    for (const v of values) {
      if (v !== undefined && v !== null && v !== '') return v;
    }
    return null;
  }

  private parseDate(v: unknown): string | null {
    if (!v) return null;
    const d = new Date(v as any);
    return isNaN(d.getTime()) ? null : d.toISOString();
  }

  private parseDateList(values: any[]): string | null {
    for (const v of values) {
      const parsed = this.parseDate(v);
      if (parsed) return parsed;
    }
    return null;
  }

  private normalize(raw: any): AdminReg {
    if (!raw) {
      return { id: 0, userName: null, phone: null, checkIn: null, checkOut: null, isOpen: false };
    }

    const idValue = this.pickFirst([
      raw.id, raw.ID, raw.registrationId, raw._id, raw.guid, raw.uuid
    ]);

    const userNameValue = this.pickFirst([
      raw.userName, raw.user_name, raw.username, raw.name, raw.fullName, raw.full_name,
      raw.user?.name, raw.user?.fullName, raw.user?.username,
      raw.participant?.name, raw.participant?.fullName
    ]) ?? (
      raw.user && (raw.user.firstName || raw.user.lastName)
        ? [raw.user.firstName, raw.user.lastName].filter(Boolean).join(' ')
        : null
    );

    const phoneValue = this.pickFirst([
      raw.phone, raw.phoneNumber, raw.phone_number, raw.telefone, raw.celular,
      raw.mobile, raw.mobilePhone, raw.msisdn,
      raw.user?.phone, raw.user?.phoneNumber, raw.user?.telefone,
      raw.participant?.phone
    ])?.toString() ?? null;

    const checkInValue = this.parseDateList([
      raw.checkIn, raw.check_in, raw.startTime, raw.startedAt,
      raw.createdAt, raw.created_at, raw.openedAt, raw.opened_at
    ]);

    const checkOutValue = this.parseDateList([
      raw.checkOut, raw.check_out, raw.endTime, raw.endedAt,
      raw.closedAt, raw.closed_at, raw.finishedAt, raw.finished_at
    ]);

    const statusRaw = (raw.status ?? raw.state ?? raw.currentStatus);
    const isOpenValue =
      (typeof raw.isOpen === 'boolean') ? raw.isOpen
        : (typeof raw.open === 'boolean') ? raw.open
          : (statusRaw ? statusRaw.toString().toUpperCase() === 'OPEN' : !checkOutValue);

    const isAdminRaw = this.pickFirst([
      raw.isAdmin, raw.admin, raw.is_admin,
      raw.role, raw.user?.role, raw.user?.isAdmin
    ]);
    const isAdminFlag =
      typeof isAdminRaw === 'boolean'
        ? isAdminRaw
        : (typeof isAdminRaw === 'string' ? /admin/i.test(isAdminRaw) : false);

    return {
      id: idValue ?? 0,
      userName: userNameValue,
      phone: phoneValue,
      checkIn: checkInValue,
      checkOut: checkOutValue,
      isOpen: isOpenValue,
      isAdmin: isAdminFlag
    };
  }

  get detectedKeys(): string[] {
    return this.originalRaw.length ? Object.keys(this.originalRaw[0]) : [];
  }

  get firstRawSample(): any {
    return this.originalRaw.length ? this.originalRaw[0] : null;
  }

  load(): void {
    this.loading = true;
    this.error = '';
    this.networkDown = false;
    const url = `${this.base}/registration/admin`;
    this.lastUrl = url;
    console.debug('[admin-dashboard] requesting:', url);

    this.http.get<any[]>(url, this.authHeaders()).subscribe({
      next: (raw: any[]) => {
        console.debug('[admin-dashboard] raw response:', raw);
        if (!Array.isArray(raw)) {
          console.warn('Response is not an array', raw);
          this.originalRaw = [];
          this.registrations = [];
        } else {
          this.originalRaw = raw;
          const normalized = raw.map(r => this.normalize(r));
          this.registrations = this.includeAdmins
            ? normalized
            : normalized.filter(r => !r.isAdmin);
          if (this.allFieldsEmpty) {
            console.warn('All fields empty after normalization. Raw keys:', this.detectedKeys);
          }
        }
        this.loading = false;
      },
      error: (err: HttpErrorResponse) => {
        console.error('Load error', err);
        if (err.status === 0) {
          this.networkDown = true;
          this.error = 'Backend kan ikke kontaktes (forbindelsesfejl).';
        } else if (err.status === 401) {
          this.error = 'Uautoriseret. Log ind igen.';
          this.router.navigate(['/admin-login']);
        } else {
          this.handleError(`Fejl ${err.status || ''}`.trim());
        }
      }
    });
  }

  trackById = (_: number, r: AdminReg) => r.id;

  get debugLabel(): string {
    return this.debug ? 'Hide RAW' : 'Show RAW';
  }

  toggleDebug(): void {
    this.debug = !this.debug;
  }

  private handleError(msg = 'Anmodning mislykkedes'): void {
    this.error = msg;
    this.loading = false;
  }

  private authHeaders() {
    const t = localStorage.getItem('adminToken') || '';
    return { headers: { 'X-Admin-Token': t } };
  }

  manageUsers(): void {
    this.router.navigate(['/users-dashboard']);
  }



  delete(r: AdminReg): void {
    if (!confirm('Delete this registration?')) return;
    this.http.delete(`${this.base}/registration/${r.id}`, this.authHeaders())
      .subscribe({
        next: () => this.load(),
        error: () => this.handleError('Sletning mislykkedes')
      });
  }

  logout(): void {
    this.router.navigate(['/']);
  }

  // Example additional endpoints (commented):
  // loadOpen() { this.http.get<AdminReg[]>(`${this.base}/registration/open`, this.authHeaders()).subscribe(...); }
  // forceCheckout(r: AdminReg) { this.http.patch(`${this.base}/registration/${r.id}/checkout`, {}, this.authHeaders()).subscribe(...); }
}


