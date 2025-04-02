
export interface Grade {
  gradeId: number;
  value: string | number; 
  studentName: string; 
  className: string;   
  assignmentName?: string;
  classId?: number;
  studentId?: number;
}

export interface Student {
  name: string;
  grades: Grade[];
  studentId?: number; 
}

export interface Class {
  name: string;
  students: Student[];
  expanded: boolean;
  showInput?: boolean;
  newStudentName?: string;
  userId: number;
  classId?: number;
}
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserService } from '../../services/apiService';
import { FormsModule } from '@angular/forms';


@Component({
  selector: 'app-teacher',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './teacher.component.html',
  styleUrls: ['./teacher.component.css'] 
})
export class TeacherComponent implements OnInit {
  classes: Class[] = [];
  teacherId: number | null = null; 
  isLoadingClasses = false;
  isLoadingGrades = false;
  errorMessage: string | null = null;

  constructor(private userService: UserService) {}

  ngOnInit(): void {
    this.teacherId = this.userService.getTeacherId(); 
    if (this.teacherId) {
      console.log('Teacher ID found:', this.teacherId);
      this.fetchClasses();
    } else {
      this.errorMessage = 'Could not retrieve Teacher ID. Please log in again.';
      console.error('Teacher ID not found in token.');
    }
  }

  fetchClasses(): void {
    if (!this.teacherId) return;

    this.isLoadingClasses = true;
    this.errorMessage = null;
    this.userService.getClasses().subscribe({
      next: (data) => {
        console.log('Raw classes data:', data);
        this.classes = data.map((classItem: any): Class => ({
          name: classItem.name,
          userId: classItem.userId, 
          students: (classItem.students as string[] || []).map((studentName): Student => ({
             name: studentName,
             grades: [] 
          })),
          expanded: false,
          showInput: false,
          newStudentName: ''
        }));
        console.log('Processed classes:', this.classes);
        this.isLoadingClasses = false;
        this.fetchAndProcessGrades();
      },
      error: (error) => {
        console.error('Error fetching classes:', error);
        this.errorMessage = 'Failed to load classes. Please try again later.';
        this.isLoadingClasses = false;
      }
    });
  }

  fetchAndProcessGrades(): void {
    if (!this.teacherId) {
        console.error("Cannot fetch grades without teacherId");
        return;
    }
    if (this.classes.length === 0) {
        console.log("No classes found, skipping grade fetching.");
        return;
    }

    this.isLoadingGrades = true;
    this.userService.getGradesByTeacher(this.teacherId).subscribe({
        next: (gradesData) => {
            console.log('Fetched grades data:', gradesData);
            this.processGrades(gradesData);
            this.isLoadingGrades = false;
        },
        error: (error) => {
            console.error('Error fetching grades:', error);
            this.errorMessage = 'Failed to load grades. Grades data might be incomplete.';
            this.isLoadingGrades = false;
        }
    });
  }

  processGrades(grades: Grade[]): void {
    const gradeMap = new Map<string, Map<string, Grade[]>>();

    for (const grade of grades) {
        if (!grade.className || !grade.studentName) {
            console.warn("Skipping grade due to missing className or studentName:", grade);
            continue;
        }

        if (!gradeMap.has(grade.className)) {
            gradeMap.set(grade.className, new Map<string, Grade[]>());
        }

        const classGradeMap = gradeMap.get(grade.className)!;

        if (!classGradeMap.has(grade.studentName)) {
            classGradeMap.set(grade.studentName, []);
        }

        classGradeMap.get(grade.studentName)!.push(grade);
    }

    console.log("Processed grade map:", gradeMap);

    for (const cls of this.classes) {
        const classGradeMap = gradeMap.get(cls.name);
        if (classGradeMap) {
            for (const student of cls.students) {
                const studentGrades = classGradeMap.get(student.name);
                if (studentGrades) {
                    student.grades = studentGrades;
                } else {
                    student.grades = []; 
                }
            }
        } else {
             for (const student of cls.students) {
                 student.grades = [];
             }
        }
    }
    console.log("Classes after grade processing:", this.classes);
  }


  toggleClass(index: number, event: Event) {
    const target = event.target as HTMLElement;
    if (target.closest('button, input, .student-list-item')) {
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
          console.log(`Student added successfully via API: ${studentName}`, response);
          const newStudent: Student = { name: studentName, grades: [] };
          this.classes[index].students.push(newStudent);
          this.classes[index].newStudentName = '';
          this.classes[index].showInput = false;
          this.classes[index].expanded = true;
        },
        error: (error) => {
          console.error('Error adding student:', error);
          this.errorMessage = `Error adding student: ${error?.error?.message || 'Unknown error'}`;
        }
      });
    }
  }
}