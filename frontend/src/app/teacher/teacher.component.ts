import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserService } from '../../services/apiService';
import { FormsModule } from '@angular/forms';


interface Class {
  name: string;
  students: string[];
  expanded: boolean;
  showInput?: boolean;
  newStudentName?: string;
}

@Component({
  selector: 'app-teacher',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './teacher.component.html',
  styleUrl: './teacher.component.css'
})
export class TeacherComponent {
  classes: Class[] = [];

  constructor(private userService: UserService) {}

  ngOnInit(): void {
    this.fetchClasses();
  }

  fetchClasses(): void {
    this.userService.getClasses().subscribe({
      next: (data) => {
        this.classes = data.map((classItem: any) => ({
          ...classItem,
          expanded: false,
          showInput: false,
          newStudentName: ''
        }));
      },
      error: (error) => {
        console.error('Error fetching classes:', error);
      }
    });
  }

  toggleClass(index: number, event: Event) {
    const target = event.target as HTMLElement;
  
    if (target.tagName === 'BUTTON' || target.tagName === 'INPUT' || target.tagName === 'LI') {
      event.stopPropagation(); 
      return;
    }
  
    this.classes[index].expanded = !this.classes[index].expanded;
  }

  showInputField(index: number, event: Event) {
    event.stopPropagation();
    this.classes[index].showInput = true;
  }

  addStudent(index: number, event: Event) {
    event.stopPropagation();
  
    const studentName = this.classes[index].newStudentName?.trim();
    const className = this.classes[index].name;
  
    if (studentName) {
      this.userService.addStudentToClass(className, studentName).subscribe({
        next: (response) => {
          console.log(`Student added successfully: ${studentName}`, response);
          this.classes[index].students.push(studentName);
          this.classes[index].newStudentName = '';
          this.classes[index].showInput = false;
        },
        error: (error) => {
          console.error('Error adding student:', error?.error?.message || 'Unknown error occurred.');
        }
      });
    }
  }
}