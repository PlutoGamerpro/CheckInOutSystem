import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';
import { RegistrationsService } from '../shared/services/registrations.service';

interface AdminReg {
  id: number | string;
  userName: string | null;
  phone: string | null;
  checkIn: string | null;
  checkOut: string | null;
  isOpen: boolean;
  isAdmin?: boolean; 
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
  lastUrl = ''; // removed 'private' for access in template
  private readonly base = environment.baseApiUrl.replace(/\/$/, '');
  includeAdmins = true; // define false to exclude adminds from list
  currentPeriod: string | null = 'all'; 

  // Estado de calendário
  calendarYear = new Date().getUTCFullYear();
  calendarMonth = new Date().getUTCMonth(); // 0-11
  calendarLabel = '';
  monthInputValue = '';

  constructor(private http: HttpClient, private router: Router, private registrationsService: RegistrationsService) {
    this.updateCalendarLabel();
  }

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
    // Require backend returning userName and phone (fixed in admincontroller 'getall' + create registration with fkuserid)
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
      raw.createdAt, raw.created_at, raw.openedAt, raw.opened_at,
      raw.timeStart, raw.timestart, raw.TimeStart 
    ]);

    const checkOutValue = this.parseDateList([
      raw.checkOut, raw.check_out, raw.endTime, raw.endedAt,
      raw.closedAt, raw.closed_at, raw.finishedAt, raw.finished_at,
      raw.timeEnd, raw.timeend, raw.TimeEnd 
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
    this.currentPeriod = 'all';
    this.loading = true;
    this.error = '';
    this.networkDown = false;
    this.lastUrl = `${this.base}/admin/registrations`;
    console.debug('[admin-dashboard] requesting ALL via service:', this.lastUrl);

    this.registrationsService.getAllRegistrations().subscribe({
      next: (raw: any[]) => {
        console.debug('[admin-dashboard] raw response:', raw);
        this.originalRaw = Array.isArray(raw) ? raw : [];
        this.normalizeAndAssign(); // replaces inline logic
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

  manageUsers(): void {
    this.router.navigate(['/users-dashboard']);
  }

  fetchPeriod(period: 'today' | 'yesterday' | 'week' | 'month' | 'year'): void {
    this.currentPeriod = period;
    this.loading = true;
    this.error = '';
    this.networkDown = false;
    this.lastUrl = `${this.base}/admin/registrations/${period}`;
    console.debug('[admin-dashboard] requesting period via service:', period, this.lastUrl);

   
    // mapped to corresponding wrappers
    let obs;
    switch (period) {
      case 'today': obs = this.registrationsService.getToday(); break;
      case 'yesterday': obs = this.registrationsService.getYesterday(); break;
      case 'week': obs = this.registrationsService.getWeek(); break;
      case 'month': obs = this.registrationsService.getMonth(); break;
      case 'year': obs = this.registrationsService.getYear(); break;
    }
    obs.subscribe({
      next: raw => {
        this.originalRaw = Array.isArray(raw) ? raw : [];
        this.normalizeAndAssign(); // replaces inline logic
        this.loading = false;
      },
      error: (err: HttpErrorResponse) => {
        console.error(`[admin-dashboard] error period ${period}`, err);
        this.loading = false;
        if (err.status === 0) { this.networkDown = true; this.error = 'Backend offline'; }
        else if (err.status === 401) this.error = 'Não autorizado';
        else this.error = `Falha (${period}) status ${err.status}`;
      }
    });
  }

  private normalizeAndAssign(): void {
    const raw = this.originalRaw;
    if (!Array.isArray(raw)) {
      this.registrations = [];
      return;
    }
    const normalized = raw.map(r => this.normalize(r));
    this.registrations = this.includeAdmins
      ? normalized
      : normalized.filter(r => !r.isAdmin);
    if (this.allFieldsEmpty) {
      console.warn('All fields empty after normalization. Keys:', this.detectedKeys);
    }
  }

  deleteRegistration(id: string | number): void { 
    if (id === undefined || id === null) return;
    if (!confirm(`Remover registro ${id}?`)) return;
    this.registrationsService.deleteRegistration(id).subscribe({
      next: () => {
        // updates local list without ful reload(faster)
        this.registrations = this.registrations.filter(r => r.id !== id);
        this.originalRaw = this.originalRaw.filter((r: any) => (r.id ?? r.ID) !== id);
      },
      error: (err: HttpErrorResponse) => {
        console.error(`Erro ao excluir registro ${id}:`, err);
        this.handleError('Falha ao excluir registro');
      }
    });
  }

  Home(): void {
    this.router.navigate(['/']);
  }

  enterCalendarMode() {
    this.currentPeriod = 'calendar';
    const now = new Date();
    this.calendarYear = now.getUTCFullYear();
    this.calendarMonth = now.getUTCMonth();
    this.updateCalendarLabel();
    this.fetchCalendarMonth();
  }

  exitCalendarMode() {
    this.currentPeriod = 'all';
    this.load();
  }

  goThisMonth() {
    const now = new Date();
    this.calendarYear = now.getUTCFullYear();
    this.calendarMonth = now.getUTCMonth();
    this.updateCalendarLabel();
    this.fetchCalendarMonth();
  }

  prevMonth() {
    this.calendarMonth--;
    if (this.calendarMonth < 0) { this.calendarMonth = 11; this.calendarYear--; }
    this.updateCalendarLabel();
    this.fetchCalendarMonth();
  }

  nextMonth() {
    this.calendarMonth++;
    if (this.calendarMonth > 11) { this.calendarMonth = 0; this.calendarYear++; }
    this.updateCalendarLabel();
    this.fetchCalendarMonth();
  }

  onMonthInput(ev: Event) {
    const val = (ev.target as HTMLInputElement).value; // format yyyy-MM
    if (!val) return;
    const [year, month] = val.split('-').map(Number);
    if (!year || !month) return;
    this.calendarYear = year;
    this.calendarMonth = month - 1;
    this.updateCalendarLabel();
    this.fetchCalendarMonth();
  }

  private updateCalendarLabel() {
    const monthNames = ['Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec'];
    this.calendarLabel = `${monthNames[this.calendarMonth]} ${this.calendarYear}`;
    this.monthInputValue = `${this.calendarYear}-${String(this.calendarMonth + 1).padStart(2,'0')}`;
  }

  private fetchCalendarMonth() {
    if (this.currentPeriod !== 'calendar') return;
    const start = new Date(Date.UTC(this.calendarYear, this.calendarMonth, 1));
    const end = new Date(Date.UTC(this.calendarYear, this.calendarMonth + 1, 1));
    const startIso = start.toISOString();
    const endIso = end.toISOString();
    this.loading = true;
   
    const url = `${this.base}/admin/registrations/range?start=${encodeURIComponent(startIso)}&end=${encodeURIComponent(endIso)}`;
    this.lastUrl = url;
    this.http.get<any[]>(url).subscribe({
      next: data => {
        this.originalRaw = data || [];
        this.normalizeAndAssign();
        this.loading = false;
      },
      error: err => {
        this.loading = false;
        // avoids flooding of error if endpoint does not exist (after backend fix it should exist)
        this.error = err?.status === 404 ? '' : 'Failed loading calendar month';
        this.networkDown = !!(err.status === 0);
      }
    });
  }

 
}


