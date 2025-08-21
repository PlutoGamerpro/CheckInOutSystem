import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiUrlService } from '../../core/api-url.service';

@Injectable({ providedIn: 'root' })
export class RegistrationsService {
  constructor(private http: HttpClient, private apiUrl: ApiUrlService) {}

  private authOpts() {
    const t = localStorage.getItem('adminToken') || '';
    return { headers: { 'X-Admin-Token': t } };
  }

  // Wrapper para "todos" (equivalente a período vazio)
  // getAllRegistrations depende agora de backend com projeção userName/phone corrigida
  getAllRegistrations(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl.url('admin/registrations'), this.authOpts());
  }
  deleteRegistration(id: number | string): Observable<void> {
    return this.http.delete<void>(this.apiUrl.url(`checkin/${id}`), this.authOpts());
  }

  // Wrappers semânticos (opcionais)
  getToday(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl.url('admin/registrations/today'), this.authOpts());
  }
  getYesterday(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl.url('admin/registrations/yesterday'), this.authOpts());
  }
  getWeek(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl.url('admin/registrations/week'), this.authOpts());
  }
  getMonth(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl.url('admin/registrations/month'), this.authOpts());
  }
  getYear(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl.url('admin/registrations/year'), this.authOpts());
  }
}