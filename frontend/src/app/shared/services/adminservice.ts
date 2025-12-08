import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiUrlService } from '../../core/api-url.service';

@Injectable({
  providedIn: 'root'
})
export class Adminservice {

  constructor(private http: HttpClient, private apiUrlService: ApiUrlService) { }

  private get adminHeaders() {
    const token = localStorage.getItem('adminToken') /*|| localStorage.getItem('managerToken')*/ || '';
    return { headers: new HttpHeaders({ 'Authorization': `Bearer ${token}`, 'X-Admin-Token': token }) };
  }

  DeleteUser(userId: number): Observable<any> {
    return this.http.delete(this.apiUrlService.url(`external/user/${userId}`), this.adminHeaders);
  }

  updateUser(user: any): Observable<void> {
    return this.http.put<void>(this.apiUrlService.url(`external/user`), user, this.adminHeaders);
  }
  // add other admin related methods here later 
}
