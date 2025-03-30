import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = 'http://localhost:5063/User'; 

  constructor(private http: HttpClient) {}

  login(email: string, password: string): Observable<string> {
    const payload = { username: email, password };
    return this.http.post<string>(`${this.apiUrl}/login`, payload);
  }
  
  resetPassword(email: string): Observable<any> {
    const payload = { email };
    return this.http.post(`${this.apiUrl}/reset-password`, payload);
  }
}
