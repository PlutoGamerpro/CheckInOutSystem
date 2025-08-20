import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiUrlService } from '../../core/api-url.service';

// Forventede svartyper
interface StatusResponse { isCheckedIn: boolean; }
interface ActionResponse { name: string; }

@Injectable({ providedIn: 'root' })
export class CheckinService {
  constructor(private http: HttpClient, private apiUrl: ApiUrlService) {}

  getStatus(phone: string): Observable<StatusResponse> {
    return this.http.get<StatusResponse>(this.apiUrl.url(`checkin/status/${phone}`));
  }

  checkinByPhone(phone: string): Observable<ActionResponse> {
    return this.http.post<ActionResponse>(this.apiUrl.url(`checkin/byphone/${phone}`), {});
  }

  checkoutByPhone(phone: string): Observable<ActionResponse> {
    return this.http.post<ActionResponse>(this.apiUrl.url(`checkout/byphone/${phone}`), {});
  }

  // List all registrations (used by admin dashboard)
  getAllRegistrations(): Observable<any> {
    return this.http.get(this.apiUrl.url('checkin'));
  }

  // Delete a registration by id
  deleteRegistration(id: string): Observable<any> {
    return this.http.delete(this.apiUrl.url(`checkin/${id}`));
  }
}