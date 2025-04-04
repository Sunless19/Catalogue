import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/apiService';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-student',
  templateUrl: './student.component.html',
  standalone: true,
  imports: [CommonModule],
  styleUrls: ['./student.component.css']
})
export class StudentComponent implements OnInit {
  studentClasses: any[] = [];
  isLoading = true;
  errorMessage: string = '';

  constructor(private userService: UserService) {}

  ngOnInit(): void {
    const studentId = this.userService.getTeacherId();
    this.userService.getStudentClassesAndGrades(studentId).subscribe({
      next: (data) => {
        this.studentClasses = data.map(subject => ({
          ...subject,
          expanded: false,
          sortDescending: true // default: cele mai noi primele
        }));
        this.isLoading = false;
      },
      error: (err) => {
        this.errorMessage = 'Failed to load subjects or grades.';
        this.isLoading = false;
        console.error(err);
      }
    });
  }
  
  // 🔁 Toggle sort order for a subject
  toggleSort(index: number): void {
    const subject = this.studentClasses[index];
    subject.sortDescending = !subject.sortDescending;
  
    subject.grades.sort((a: any, b: any) => {
      const dateA = new Date(a.date).getTime();
      const dateB = new Date(b.date).getTime();
      return subject.sortDescending ? dateB - dateA : dateA - dateB;
    });
  }

  averageForSubject(grades: any[]): number {
    if (!grades || grades.length === 0) return 0;
    const total = grades.reduce((sum, grade) => sum + grade.value, 0);
    return parseFloat((total / grades.length).toFixed(2));
  }
  
  get overallAverage(): number {
    const subjectAverages = this.studentClasses
      .map(subject => {
        const grades = subject.grades;
        if (!grades || grades.length === 0) return null;
        const total = grades.reduce((sum: number, grade: any) => sum + grade.value, 0);
        return total / grades.length;
      })
      .filter((avg): avg is number => avg !== null); 
  
    if (subjectAverages.length === 0) return 0;
  
    const overall = subjectAverages.reduce((sum: number, avg: number) => sum + avg, 0) / subjectAverages.length;
    return parseFloat(overall.toFixed(2));
  }
  
  toggleSubject(index: number): void {
    this.studentClasses[index].expanded = !this.studentClasses[index].expanded;
  }
}