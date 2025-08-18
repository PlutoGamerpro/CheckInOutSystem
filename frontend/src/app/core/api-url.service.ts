import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ApiUrlService {
  baseUrl = environment.baseApiUrl;

  url(path: string): string {
    // Ensures no double slashes
    return `${this.baseUrl.replace(/\/$/, '')}/${path.replace(/^\//, '')}`;
  }
}
