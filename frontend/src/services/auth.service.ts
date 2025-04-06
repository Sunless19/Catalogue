import { Injectable } from '@angular/core';
import { HttpClient} from '@angular/common/http';

import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private apiUrl = 'http://localhost:5063/User';

  constructor(private http: HttpClient) {}

  login(email: string, password: string): Observable<{ token: string }> {
    const payload = { username: email, password };
    return this.http.post<{ token: string }>(`${this.apiUrl}/login`, payload).pipe(
      tap((response) => {
        if (response?.token) {
          localStorage.setItem('token', response.token);
        }
      })
    );
  }

  register(payload: any): Observable<string> {
    return this.http.post<string>(`${this.apiUrl}/register`, payload);
  }

  resetPassword(email: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/reset-password`, { email });
  }

  sendResetEmail(email: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/send-reset-email`, { email });
  }

  confirmPasswordReset(encodedId: string, newPassword: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/reset-password`, { encodedId, newPassword });
  }
}