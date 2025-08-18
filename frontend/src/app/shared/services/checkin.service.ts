import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiUrlService } from '../../core/api-url.service';

/**
 * CheckinService
 *
 * Purpose:
 * - Centralizes all HTTP calls related to check-in / check-out flows.
 * - Builds URLs via ApiUrlService to keep them consistent.
 * - Returns typed Observables (no side-effects or subscriptions here).
 *
 * About Observable & subscribe:
 * - HttpClient methods (get/post/delete/put) return an Observable<T>.
 * - An Observable is a lazy stream: nothing happens until you call subscribe().
 * - subscribe() is where you provide callbacks (next / error / complete) to handle the result.
 * - After the HTTP request finishes, the Observable completes automatically (no manual unsubscribe needed for single-shot HTTP calls).
 * - Keep subscribe() usage inside components (or facades), not inside services, so components control UI state and error handling.
 */

// Response shapes (export if needed elsewhere)
export interface StatusResponse { isCheckedIn: boolean; }
export interface ActionResponse { name: string; }

@Injectable({ providedIn: 'root' })
export class CheckinService {
  constructor(private http: HttpClient, private apiUrl: ApiUrlService) {}

  /**
   * Get current check-in status for a phone number.
   * The component will subscribe and decide whether to check in or out.
   */
  getStatus(phone: string): Observable<StatusResponse> {
    return this.http.get<StatusResponse>(this.apiUrl.url(`checkin/status/${phone}`));
  }

  /**
   * Perform check-in for the given phone.
   * Returns the user's display name (ActionResponse).
   */
  checkinByPhone(phone: string): Observable<ActionResponse> {
    return this.http.post<ActionResponse>(this.apiUrl.url(`checkin/byphone/${phone}`), {});
  }

  /**
   * Perform check-out for the given phone.
   * Returns the user's display name (ActionResponse).
   */
  checkoutByPhone(phone: string): Observable<ActionResponse> {
    return this.http.post<ActionResponse>(this.apiUrl.url(`checkout/byphone/${phone}`), {});
  }

  /**
   * Fetch all registrations (admin/dashboard usage).
   * Replace 'any' with a proper interface when the shape is stable.
   */
  getAllRegistrations(): Observable<any> {
    return this.http.get(this.apiUrl.url('checkin'));
  }

  /**
   * Delete a registration by its id.
   */
  deleteRegistration(id: string): Observable<any> {
    return this.http.delete(this.apiUrl.url(`checkin/${id}`));
  }
}