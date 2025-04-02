
export interface Grade {
  gradeId: number;
  teacherId: number | null;
  value: string | number; 
  studentName: string; 
  className: string;   
  assignmentName?: string;
  classId?: number;
  studentId: number;
  date?: string;
  isEditing?: boolean;
  editValue?: string | number; 
  editDate?: string;
  id: number;
}

export interface Student {
  name: string;
  grades: Grade[];
  studentId: number; 
}

export interface Class {
  name: string;
  students: Student[];
  expanded: boolean;
  showInput?: boolean;
  newStudentName?: string;
  userId: number;
  classId: number;
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
  newGradeValues: { [studentId: number]: string | number } = {};
  editingGradeId: number | null = null;
  editingGradeValue: string | number = '';
  // Store date as string in YYYY-MM-DD format for <input type="date">
  editingGradeDate: string = '';
  // Store context for updating the local array after save
  private editingContext: { classId: number, studentId: number } | null = null;
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
                classId: classItem.id, // Include classId for proper mapping
                students: (classItem.students || []).map((student: any): Student => ({
                    name: student.studentName, // Correctly map student names
                    studentId: student.studentId, // Include studentId for grade linking
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

addGrade(studentId: number, classId: number) {
  if (!this.newGradeValues[studentId]) {
    console.warn('Grade value is required.');
    return;
  }

  const newGrade: Partial<Grade> = {
    teacherId: this.teacherId,
    studentId: studentId,
    classId: classId,
    value: this.newGradeValues[studentId],
    date: new Date().toISOString()
  };

  this.userService.addGrade(newGrade).subscribe({
    next: (response) => {
      console.log('Grade added successfully:', response);

      // Find the student and update their grades list
      for (const cls of this.classes) {
        if (cls.classId === classId) {
          for (const student of cls.students) {
            if (student.studentId === studentId) {
              student.grades.push(response);
              break;
            }
          }
        }
      }

      this.newGradeValues[studentId] = ''; // Reset input field for that student
    },
    error: (error) => {
      console.error('Error adding grade:', error);
      this.errorMessage = `Error adding grade: ${error?.error?.message || 'Unknown error'}`;
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
    const gradeMap = new Map<number, Map<number, Grade[]>>(); // Use classId & studentId as keys

    for (const grade of grades) {
        if (!grade.classId || !grade.studentId) {
            console.warn("Skipping grade due to missing classId or studentId:", grade);
            continue;
        }

        if (!gradeMap.has(grade.classId)) {
            gradeMap.set(grade.classId, new Map<number, Grade[]>());
        }

        const classGradeMap = gradeMap.get(grade.classId)!;

        if (!classGradeMap.has(grade.studentId)) {
            classGradeMap.set(grade.studentId, []);
        }

        classGradeMap.get(grade.studentId)!.push(grade);
    }

    console.log("Processed grade map:", gradeMap);

    for (const cls of this.classes) {
        const classGradeMap = gradeMap.get(cls.classId);
        if (classGradeMap) {
            for (const student of cls.students) {
                const studentGrades = classGradeMap.get(student.studentId);
                student.grades = studentGrades ? studentGrades : [];
            }
        } else {
            for (const student of cls.students) {
                student.grades = [];
            }
        }
    }
    console.log("Classes after grade processing:", this.classes);
}

startEditGrade(grade: Grade): void {
  console.log('Editing grade:', grade);

  this.cancelEditGrade(grade);

  grade.isEditing = true;
  grade.editValue = grade.value;
  grade.editDate = grade.date ? new Date(grade.date).toISOString().split('T')[0] : new Date().toISOString().split('T')[0];
}


cancelEditGrade(grade: Grade): void {
  grade.isEditing = false;
  grade.editValue = undefined;
  grade.editDate = undefined;
}


saveEditGrade(grade: Grade): void {

  console.log('Saving grade:', grade.gradeId);
  if (!grade.gradeId) {
    console.error('Error: gradeId is missing or undefined.', grade);
    this.errorMessage = 'Cannot update grade: Missing gradeId.';
    return;
  }

  if (!grade.editValue) {
    console.warn('Grade value cannot be empty.');
    return;
  }

  const updatePayload: Partial<Grade> = {
    value: grade.editValue,
    date: new Date(grade.editDate!).toISOString()
  };

  console.log('Updating grade:', grade.gradeId, updatePayload);

  this.userService.updateGrade(grade.gradeId, updatePayload).subscribe({
    next: (updatedGrade) => {
      console.log('Grade updated successfully:', updatedGrade);

      // Update the local grade
      grade.value = updatedGrade.value;
      grade.date = updatedGrade.date;

      // Exit edit mode
      this.cancelEditGrade(grade);
    },
    error: (error) => {
      console.error('Error updating grade:', error);
      this.errorMessage = `Error updating grade: ${error?.error?.message || 'Unknown error'}`;
    }
  });
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
          const newStudent: Student = { name: studentName, grades: [] , studentId: response.studentId };
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