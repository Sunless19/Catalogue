import { Injectable } from '@angular/core';
import { HttpClient} from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserService } from './user.service';

@Injectable({ providedIn: 'root' })
export class ClassService {
  private classApiUrl = 'http://localhost:5063/api/class';

  constructor(private http: HttpClient, private userService: UserService) {}

  getClasses(): Observable<any[]> {
    const headers = this.userService.getAuthHeaders();
    return this.http.get<any[]>(`${this.classApiUrl}/show-classes`, { headers });
  }

  addStudentToClass(classId: number, studentName: string): Observable<any> {
    const headers = this.userService.getAuthHeaders();
    return this.http.post<any>(`${this.classApiUrl}/add-student`, { classId, studentName }, { headers });
  }

  getStudentClassesAndGrades(studentId: number): Observable<any[]> {
    const headers = this.userService.getAuthHeaders();
    return this.http.get<any[]>(`${this.classApiUrl}/student/${studentId}`, { headers });
  }

  deleteStudent(classId: number, studentId: number): Observable<any> {
    const headers = this.userService.getAuthHeaders();
    return this.http.request('DELETE', `${this.classApiUrl}/delete-student`, {
      headers,
      body: { classId, studentId },
    });
  }
}
