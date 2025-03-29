import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import * as CryptoJS from 'crypto-js';
import { ApiService } from './api.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private apiUrl = 'https://api.exemplu.com/api/users'; 

  constructor(private api: ApiService) {}

  private encryptPassword(password: string): string {
    return CryptoJS.SHA256(password).toString();
  }

  login(email: string, password: string): Observable<any> {
    const encryptedPassword = this.encryptPassword(password);
    const body = { email, password: encryptedPassword };

    return this.api.post(`${this.apiUrl}/login`, body);
  }

  resetPassword(email: string): Observable<any> {
    return this.api.post(`${this.apiUrl}/reset-password`, { email });
  }
}
