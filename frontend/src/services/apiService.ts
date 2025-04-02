import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs'; // Import throwError
import { UserRole } from '../models/userRole';
import { jwtDecode } from 'jwt-decode';
import { tap, catchError } from 'rxjs/operators'; // Import catchError

import { TeacherComponent ,Grade} from '../app/teacher/teacher.component';

export interface JwtPayload {
  nameid: string; // Standard claim for User ID
  unique_name: string; // Standard claim for Username (often email)
  role: UserRole;
  exp: number;
  // Add other custom claims your token might have
}


@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = 'http://localhost:5063/User';
  private classApiUrl = 'http://localhost:5063/api/class'; // Base URL for class API
  private gradeApiUrl = 'http://localhost:5063/api/Grade'; // Base URL for grade API

  constructor(private http: HttpClient) {}

  // --- Token and Role Methods ---

  private getToken(): string | null {
    return localStorage.getItem('token');
  }

  private getAuthHeaders(): HttpHeaders {
    const token = this.getToken();
    if (!token) {
      console.error('No token found in local storage');
      // In a real app, you might redirect to login here
      return new HttpHeaders(); // Return empty headers or handle appropriately
    }
    return new HttpHeaders().set('Authorization', `Bearer ${token}`);
  }

  // Method to get Teacher ID reliably from token
  getTeacherId(): number | null {
    const token = this.getToken();
    if (!token) return null;

    try {
      const decoded = jwtDecode<JwtPayload>(token);
      // Assuming 'nameid' claim holds the user ID (common practice)
      // Adjust 'nameid' if your token uses a different claim like 'sub' or a custom one
      const userId = parseInt(decoded.nameid, 10);
      return !isNaN(userId) ? userId : null;
    } catch (err) {
      console.error('Invalid token or missing ID claim', err);
      return null;
    }
  }

  getUserRole(): UserRole | null {
    const token = this.getToken();
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

  // --- Auth API Calls ---

  login(email: string, password: string): Observable<{ token: string }> {
    const payload = { username: email, password };
    return this.http.post<{ token: string }>(`${this.apiUrl}/login`, payload).pipe(
      tap((response) => {
        if (response && response.token) {
          console.log('Saving token:', response.token);
          localStorage.setItem('token', response.token);
        } else {
          console.error('No token received from server');
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

  // --- Class API Calls ---

  // Assuming the class API returns raw class data including student names as strings
  getClasses(): Observable<any[]> {
      const headers = this.getAuthHeaders();
      if (!headers.has('Authorization')) {
          return throwError(() => new Error('Authorization header missing'));
      }
      console.log('Fetching classes with headers:', headers);
      return this.http.get<any[]>(`${this.classApiUrl}/show-classes`, { headers }).pipe(
          catchError(err => {
              console.error('Error fetching classes:', err);
              return throwError(() => err); // Propagate the error
          })
      );
  }


  addStudentToClass(className: string, studentName: string): Observable<any> {
    const headers = this.getAuthHeaders();
    if (!headers.has('Authorization')) {
       return throwError(() => new Error('Authorization header missing'));
    }
    const payload = { className, studentName };
    return this.http.post<any>(`${this.classApiUrl}/add-student`, payload, { headers }).pipe(
        catchError(err => {
            console.error('Error adding student:', err);
            return throwError(() => err); // Propagate the error
        })
    );
  }

  // --- Grade API Calls ---

  getGradesByTeacher(teacherId: number): Observable<Grade[]> {
    const headers = this.getAuthHeaders();
     if (!headers.has('Authorization')) {
        return throwError(() => new Error('Authorization header missing'));
     }
    return this.http.get<Grade[]>(`${this.gradeApiUrl}/teacher/${teacherId}`, { headers }).pipe(
        tap(grades => console.log(`Fetched ${grades.length} grades for teacher ${teacherId}`)),
        catchError(err => {
            console.error(`Error fetching grades for teacher ${teacherId}:`, err);
            return throwError(() => err); // Propagate the error
        })
    );
  }
}