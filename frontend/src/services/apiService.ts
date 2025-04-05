import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { UserRole } from '../models/userRole';
import { jwtDecode } from 'jwt-decode';
import { tap, catchError } from 'rxjs/operators';

import { Grade } from '../app/teacher/teacher.component';

export interface JwtPayload {
  nameid: string;
  unique_name: string;
  role: UserRole;
  exp: number;
}

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = 'http://localhost:5063/User';
  private classApiUrl = 'http://localhost:5063/api/class';
  private gradeApiUrl = 'http://localhost:5063/api/Grade';

  constructor(private http: HttpClient) {}

  // Token and role methods (unchanged)
  private getToken(): string | null {
    return localStorage.getItem('token');
  }

  private getAuthHeaders(): HttpHeaders {
    const token = this.getToken();
    if (!token) {
      console.error('No token found in local storage');
      return new HttpHeaders();
    }
    return new HttpHeaders().set('Authorization', `Bearer ${token}`);
  }

  getTeacherId(): number | 0 {
    const token = this.getToken();
    if (!token) return 0;

    try {
      const decoded = jwtDecode<JwtPayload>(token);
      const userId = parseInt(decoded.nameid, 10);
      return !isNaN(userId) ? userId : 0;
    } catch (err) {
      console.error('Invalid token or missing ID claim', err);
      return 0;
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

  // Auth API calls (unchanged)
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

  // Class API calls (unchanged)
  getClasses(): Observable<any[]> {
    const headers = this.getAuthHeaders();
    if (!headers.has('Authorization')) {
      return throwError(() => new Error('Authorization header missing'));
    }
    console.log('Fetching classes with headers:', headers);
    return this.http.get<any[]>(`${this.classApiUrl}/show-classes`, { headers }).pipe(
      catchError(err => {
        console.error('Error fetching classes:', err);
        return throwError(() => err);
      })
    );
  }

  

  // Grade API calls
  getGradesByTeacher(teacherId: number): Observable<Grade[]> {
    const headers = this.getAuthHeaders();
    if (!headers.has('Authorization')) {
      return throwError(() => new Error('Authorization header missing'));
    }
    return this.http.get<Grade[]>(`${this.gradeApiUrl}/teacher/${teacherId}`, { headers }).pipe(
      tap(grades => console.log(`Fetched ${grades.length} grades for teacher ${teacherId}`)),
      catchError(err => {
        console.error(`Error fetching grades for teacher ${teacherId}:`, err);
        return throwError(() => err);
      })
    );
  } 

  addMultipleGrades(teacherId: number, studentId: number, classId: number, grades: { value: number; date: string }[]): Observable<any> {
    const headers = this.getAuthHeaders();
    if (!headers.has('Authorization')) {
      return throwError(() => new Error('Authorization header missing'));
    }
  
    const payload = {
      TeacherId: teacherId,
      StudentId: studentId,
      ClassId: classId,
      Grades: grades
    };
  
    return this.http.post<any>(`${this.gradeApiUrl}/add-multiple`, payload, { headers }).pipe(
      tap(response => console.log('Added multiple grades successfully:', response)),
      catchError(err => {
        console.error('Error adding multiple grades:', err);
        return throwError(() => err);
      })
    );
  }
  

  // New methods for grade operations
  addGrade(gradeData: Partial<Grade>): Observable<Grade> {
    const headers = this.getAuthHeaders();
    if (!headers.has('Authorization')) {
      return throwError(() => new Error('Authorization header missing'));
    }
    return this.http.post<Grade>(`${this.gradeApiUrl}/POST`, gradeData, { headers }).pipe(
      tap(grade => console.log(`Added grade successfully:`, grade)),
      catchError(err => {
        console.error('Error adding grade:', err);
        return throwError(() => err);
      })
    );
  }
  

  updateGrade(gradeId: number, gradeData: Partial<Grade>): Observable<Grade> {
    const headers = this.getAuthHeaders();
    if (!headers.has('Authorization')) {
      return throwError(() => new Error('Authorization header missing'));
    }
    return this.http.put<Grade>(`${this.gradeApiUrl}/update/${gradeId}`, gradeData, { headers }).pipe(
      tap(grade => console.log(`Updated grade ${gradeId} successfully:`, grade)),
      catchError(err => {
        console.error(`Error updating grade ${gradeId}:`, err);
        return throwError(() => err);
      })
    );
  }

  deleteGrade(gradeId: number): Observable<any> {
    const headers = this.getAuthHeaders();
    if (!headers.has('Authorization')) {
      return throwError(() => new Error('Authorization header missing'));
    }
    return this.http.delete<any>(`${this.gradeApiUrl}/delete/${gradeId}`, { headers }).pipe(
      tap(_ => console.log(`Deleted grade ${gradeId} successfully`)),
      catchError(err => {
        console.error(`Error deleting grade ${gradeId}:`, err);
        return throwError(() => err);
      })
    );
  }
  addStudentToClass(classId: number, studentName: string): Observable<any> {
    const headers = this.getAuthHeaders();
    if (!headers.has('Authorization')) {
        return throwError(() => new Error('Authorization header missing'));
    }
    const payload = { classId, studentName };

    return this.http.post<any>(`${this.classApiUrl}/add-student`, payload, { headers }).pipe(
        tap((response) => console.log('Added student via API:', response)),
        catchError(err => {
            console.error('Error adding student:', err);
            return throwError(() => err);
        })
    );
}


  getStudentClassesAndGrades(studentId: number): Observable<any[]> {
    const headers = this.getAuthHeaders();
    if (!headers.has('Authorization')) {
      return throwError(() => new Error('Authorization header missing'));
    }

    return this.http.get<any[]>(`${this.classApiUrl}/student/${studentId}`, { headers }).pipe(
      tap(classes => console.log(`Fetched classes and grades for student ${studentId}:`, classes)),
      catchError(err => {
        console.error(`Error fetching classes and grades for student ${studentId}:`, err);
        return throwError(() => err);
      })
    );
  }


  deleteStudent(classId: number, studentId: number): Observable<any> {
    const headers = this.getAuthHeaders();
    if (!headers.has('Authorization')) {
      return throwError(() => new Error('Authorization header missing'));
    }
  
    const payload = { classId, studentId };
  
    return this.http.request('DELETE', `${this.classApiUrl}/delete-student`, {
      headers,
      body: payload,
    }).pipe(
      catchError(err => {
        console.error('Error deleting student:', err);
        return throwError(() => err);
      })
    );
  }

  sendResetEmail(email: string): Observable<any> {
    const payload = { email };
    return this.http.post(`${this.apiUrl}/send-reset-email`, payload).pipe(
      tap(() => console.log('Reset link sent to email:', email)),
      catchError(err => {
        console.error('Error sending reset link:', err);
        return throwError(() => err);
      })
    );
  }

  confirmPasswordReset(encodedId: string, newPassword: string): Observable<any> {
    const payload = {
      encodedId,
      newPassword
    };
  
    return this.http.post(`${this.apiUrl}/reset-password`, payload).pipe(
      tap(() => console.log('Password reset successful for ID:', encodedId)),
      catchError(err => {
        console.error('Error resetting password:', err);
        return throwError(() => err);
      })
    );
  }
  
}