import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserRole } from '../models/userRole';
import { jwtDecode } from 'jwt-decode';

export interface JwtPayload {
  name: string;
  role: UserRole;
  exp: number;
}


@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = 'http://localhost:5063/User'; 

  constructor(private http: HttpClient) {}
  
  getUserRole(): UserRole | null {
    const token = localStorage.getItem('token');
    if (!token) return null;

    try {
      const decoded = jwtDecode<JwtPayload>(token);
      return decoded.role;
    } catch (err) {
      console.error('Invalid token', err);
      return null;
    }
  }

  isStudent(): boolean {
    return this.getUserRole() === UserRole.Student;
  }

  isTeacher(): boolean {
    return this.getUserRole() === UserRole.Teacher;
  }

  login(email: string, password: string): Observable<string> {
    const payload = { username: email, password };
    return this.http.post<string>(`${this.apiUrl}/login`, payload);
  }

  
  register(payload: any): Observable<string> {
    return this.http.post<string>(`${this.apiUrl}/register`, payload);
  }
  
  resetPassword(email: string): Observable<any> {
    const payload = { email };
    return this.http.post(`${this.apiUrl}/reset-password`, payload);
  }
}
