import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserService } from '../../services/apiService';

interface Class {
  name: string;
  students: string[];
  expanded: boolean;
}

@Component({
  selector: 'app-teacher',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './teacher.component.html',
  styleUrl: './teacher.component.css'
})
export class TeacherComponent {
  classes: any[] = [];

  constructor(private userService: UserService) {}

  ngOnInit(): void {
    this.fetchClasses();
  }

  fetchClasses(): void {
    this.userService.getClasses().subscribe({
      next: (data) => {
        this.classes = data.map((classItem: any) => ({
          ...classItem,
          expanded: false // Default collapsed state
        }));
      },
      error: (error) => {
        console.error('Error fetching classes:', error);
      }
    });
  }
  toggleClass(index: number) {
    this.classes[index].expanded = !this.classes[index].expanded;
  }
  addStudent(index: number, event: Event) {
    event.stopPropagation();
    console.log(`Add student to class: ${this.classes[index].name}`);
  }
}
