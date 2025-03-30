import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
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
  classes: Class[] = [
    { name: 'Mathematics', students: ['Alice', 'Bob', 'Charlie'], expanded: false },
    { name: 'Physics', students: ['David', 'Eve', 'Frank'], expanded: false },
    { name: 'Chemistry', students: ['Grace', 'Hank', 'Ivy'], expanded: false }
  ];

  toggleClass(index: number) {
    this.classes[index].expanded = !this.classes[index].expanded;
  }
}
