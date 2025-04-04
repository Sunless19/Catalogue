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
          expanded: false
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

  toggleSubject(index: number): void {
    this.studentClasses[index].expanded = !this.studentClasses[index].expanded;
  }
}