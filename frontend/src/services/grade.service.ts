import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Grade } from '../app/teacher/teacher.component';
import { UserService } from './user.service';

@Injectable({ providedIn: 'root' })
export class GradeService {
  private gradeApiUrl = 'http://localhost:5063/api/Grade';

  constructor(private http: HttpClient, private userService: UserService) {}

  getGradesByTeacher(teacherId: number): Observable<Grade[]> {
    const headers = this.userService.getAuthHeaders();
    return this.http.get<Grade[]>(`${this.gradeApiUrl}/teacher/${teacherId}`, { headers });
  }

  addGrade(gradeData: Partial<Grade>): Observable<Grade> {
    const headers = this.userService.getAuthHeaders();
    return this.http.post<Grade>(`${this.gradeApiUrl}/POST`, gradeData, { headers });
  }

  addMultipleGrades(teacherId: number, studentId: number, classId: number, grades: { value: number; date: string }[]): Observable<any> {
    const headers = this.userService.getAuthHeaders();
    const payload = { TeacherId: teacherId, StudentId: studentId, ClassId: classId, Grades: grades };
    return this.http.post<any>(`${this.gradeApiUrl}/add-multiple`, payload, { headers });
  }

  updateGrade(gradeId: number, gradeData: Partial<Grade>): Observable<Grade> {
    const headers = this.userService.getAuthHeaders();
    return this.http.put<Grade>(`${this.gradeApiUrl}/update/${gradeId}`, gradeData, { headers });
  }

  deleteGrade(gradeId: number): Observable<any> {
    const headers = this.userService.getAuthHeaders();
    return this.http.delete<any>(`${this.gradeApiUrl}/delete/${gradeId}`, { headers });
  }
}
