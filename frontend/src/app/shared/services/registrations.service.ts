import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiUrlService } from '../../core/api-url.service';

@Injectable({ providedIn: 'root' })
export class RegistrationsService {
  constructor(private http: HttpClient, private apiUrl: ApiUrlService) {}

  private authOpts() { // used to use this as normal........
    const t = localStorage.getItem('adminToken') || '';
    return { headers: { 'X-Admin-Token': t } };
  }
  private adminHeader(){
    const token = localStorage.getItem('adminToken') || localStorage.getItem('managerToken') || '';
    return {headers: { 'Authorization': `Bearer ${token}`, 'X-Admin-Token': token } };
  }



  // Wrapper para "todos" (equivalente a período vazio)
  // getAllRegistrations depende agora de backend com projeção userName/phone corrigida
  getAllRegistrations(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl.url('admin/registrations'), this.adminHeader());
  }
  deleteRegistration(id: number | string): Observable<void> {
    return this.http.delete<void>(this.apiUrl.url(`checkin/${id}`), this.adminHeader());
  }

  // Wrappers semânticos (opcionais)
  getToday(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl.url('admin/registrations/today'), this.adminHeader());
  }
  getYesterday(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl.url('admin/registrations/yesterday'), this.adminHeader());
  }
  getWeek(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl.url('admin/registrations/week'), this.adminHeader());
  }
  getMonth(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl.url('admin/registrations/month'), this.adminHeader());
  }
  getYear(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl.url('admin/registrations/year'), this.adminHeader());
  }
}