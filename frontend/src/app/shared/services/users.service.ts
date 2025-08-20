import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiUrlService } from '../../core/api-url.service';

// Forventede svartyper
interface StatusResponse { isCheckedIn: boolean; }
interface ActionResponse { name: string; }

@Injectable({ providedIn: 'root' })
export class users {
  constructor(private http: HttpClient, private apiUrl: ApiUrlService) {}

  // List all users (used by admin dashboard)
  getAllUsers(): Observable<any> {
    return this.http.get(this.apiUrl.url('user'));
  }

  
}