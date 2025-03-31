import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserRole } from '../models/userRole';
import { jwtDecode } from 'jwt-decode';
import { tap } from 'rxjs/operators';


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

  login(email: string, password: string): Observable<{ token: string }> {
    const payload = { username: email, password };
  
    return this.http.post<{ token: string }>(`${this.apiUrl}/login`, payload).pipe(
      tap((response) => {
        if (response && response.token) {
          console.log('Saving token:', response.token); // 🔥 Debugging step
          localStorage.setItem('token', response.token);
        } else {
          console.error('❌ No token received from server');
        }
      })
    );
  }

  
  register(payload: any): Observable<string> {
    return this.http.post<string>(`${this.apiUrl}/register`, payload);
  }
  
  resetPassword(email: string): Observable<any> {
    const payload = { email };
    return this.http.post(`${this.apiUrl}/reset-password`, payload);
  }

  getClasses(): Observable<any[]> {
    const token = localStorage.getItem('token'); 
    console.log('Token being sent:', token); // 🔥 Debugging step
  
    if (!token) {
      console.error('❌ No token found in local storage');
      return new Observable(observer => {
        observer.error('No token available');
      });
    }
  
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<any[]>(`http://localhost:5063/api/teacher/classes`, { headers });
  }
}
