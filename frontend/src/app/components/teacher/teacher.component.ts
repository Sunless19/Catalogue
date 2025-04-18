import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { UserService } from '@services/user.service';
import { ClassService } from '@services/class.service';
import { GradeService } from '@services/grade.service';

import { Student } from '@models/student.model';
import { Grade } from '@models/grade.model';
import { Class } from '@models/class.model';

@Component({
  selector: 'app-teacher',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './teacher.component.html',
  styleUrls: ['./teacher.component.css']
})
export class TeacherComponent implements OnInit {
  classes: Class[] = [];

  teacherId: number=0 ; 

  showBulkDescription = false;
  bulkGradeDescription = '';
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

  multipleGradesMode: { [studentId: number]: boolean } = {};
  multipleGradesValues: { [studentId: number]: string } = {};
  isAddingDescription: { [studentId: number]: boolean } = {};
  gradeDescriptions: { [studentId: number]: string } = {};
  isAddingMultipleDescription: { [studentId: number]: boolean } = {};
  multipleGradeDescriptions: { [studentId: number]: string } = {};
  constructor(private userService: UserService, private classService: ClassService, private gradeService: GradeService) {}
  


  selectedStudentIds: Set<number> = new Set<number>();

  isAnyStudentSelected(): boolean {
    return this.selectedStudentIds.size > 0;
  }

  goToMultipleDescription(studentId: number): void {
    this.isAddingMultipleDescription[studentId] = true;
  }
  toggleStudentSelection(studentId: number): void {
    if (this.selectedStudentIds.has(studentId)) {
      this.selectedStudentIds.delete(studentId);
    } else {
      this.selectedStudentIds.add(studentId);
    }
  }

  selectedBulkGradeValue: string | number = '';

  selectedGrades: Set<number> = new Set<number>();

  isAnyGradeSelected(): boolean {
    return this.selectedGrades.size > 0;
  }

  toggleGradeSelection(gradeId: number): void {
    if (this.selectedGrades.has(gradeId)) {
      this.selectedGrades.delete(gradeId);
    } else {
      this.selectedGrades.add(gradeId);
    }
  }

  bulkAddToSelected(): void {
    const value = this.selectedBulkGradeValue;
  
    if (this.isGradeInvalid(value)) {
      alert('Please enter a valid grade between 1 and 10.');
      return;
    }
  
    if (this.selectedStudentIds.size === 0) {
      alert('Please select at least one student.');
      return;
    }
  
    if (!confirm(`Are you sure you want to add grade "${value}" to all selected students?`)) {
      return;
    }
  
    let successfulAdds = 0;
    let failedAdds = 0;
  
    for (const cls of this.classes) {
      for (const student of cls.students) {
        if (this.selectedStudentIds.has(student.studentId)) {
          const newGrade: Partial<Grade> = {
            teacherId: this.teacherId,
            studentId: student.studentId,
            classId: cls.classId,
            value: value,
            date: new Date().toISOString(),
            assignments: this.bulkGradeDescription
          };
    
          this.gradeService.addGrade(newGrade).subscribe({
            next: (response: any) => {
              student.grades.push(response.grade);
              successfulAdds++;
              console.log(`Added grade to ${student.name}`);
    
              if (successfulAdds + failedAdds === this.selectedStudentIds.size) {
                alert(`Bulk add complete: ${successfulAdds} success, ${failedAdds} failed.`);
                this.selectedBulkGradeValue = '';
                this.bulkGradeDescription = '';
                this.selectedStudentIds.clear();
              }
            },
            error: (error) => {
              failedAdds++;
              console.error(`Failed to add grade to ${student.name}:`, error);
              this.errorMessage = `Bulk add error: ${error?.error?.message || 'Unknown error'}`;
    
              if (successfulAdds + failedAdds === this.selectedStudentIds.size) {
                alert(`Bulk add complete: ${successfulAdds} success, ${failedAdds} failed.`);
                this.selectedBulkGradeValue = '';
                this.bulkGradeDescription = '';
                this.selectedStudentIds.clear();
              }
            }
          });
        }
      }
    }
  
    this.selectedBulkGradeValue = '';
    this.selectedStudentIds.clear();
  }



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
    this.classService.getClasses().subscribe({
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
          newStudentName: '',
          inputMode: ''
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

    this.gradeService.addGrade(newGrade).subscribe({
      next: (response: any) => {
        console.log('Grade added successfully:', response);

        // Find the student and update their grades list
        for (const cls of this.classes) {
          if (cls.classId === classId) {
            for (const student of cls.students) {
              if (student.studentId === studentId) {
                student.grades.push(response.grade);
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

  addMultipleGrades(studentId: number, classId: number): void {
  const gradesInput = this.multipleGradesValues[studentId];
  
  if (!gradesInput || gradesInput.trim() === '') {
    console.warn('Grades values are required.');
    return;
    
  }
  
  // Parse comma-separated grades
  const gradeValues = gradesInput.split(',')
    .map(g => g.trim())
    .filter(g => g !== '');
  
  // Validate each grade
  for (const value of gradeValues) {
    if (this.isGradeInvalid(value)) {
      alert(`Invalid grade value: ${value}. All grades must be integers between 1 and 10.`);
      return;
    }
  }
  
  // Create grades array for API
  const grades = gradeValues.map(value => ({
    value: parseInt(value),
    date: new Date().toISOString(),
    assignments: this.multipleGradeDescriptions[studentId]
  }));
  
  // Call the service method
  this.gradeService.addMultipleGrades(this.teacherId, studentId, classId, grades).subscribe({
    next: (response: any) => {
      console.log('Multiple grades added successfully:', response);
      
      // Find the student and update their grades list
      for (const cls of this.classes) {
        if (cls.classId === classId) {
          for (const student of cls.students) {
            if (student.studentId === studentId) {
              // Add all the new grades to the student's grades array
              if (response.grades && Array.isArray(response.grades)) {
                student.grades = [...student.grades, ...response.grades];
              }
              break;
            }
          }
        }
      }
      
      // Reset inputs
      this.multipleGradesValues[studentId] = '';
      this.multipleGradesMode[studentId] = false;
      this.multipleGradeDescriptions[studentId] = '';
    },
    error: (error) => {
      console.error('Error adding multiple grades:', error);
      this.errorMessage = `Error adding multiple grades: ${error?.error?.message || 'Unknown error'}`;
    }
  });
}

toggleMultipleGradesMode(studentId: number): void {
  this.multipleGradesMode[studentId] = !this.multipleGradesMode[studentId];
  if (this.multipleGradesMode[studentId]) {
    this.multipleGradesValues[studentId] = '';
  }
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
    this.gradeService.getGradesByTeacher(this.teacherId).subscribe({
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
    grade.editAssignmentName = grade.assignments;
  }

  bulkDelete(): void {
    if (!confirm(`Are you sure you want to delete ${this.selectedGrades.size} grades?`)) return;

    const gradeIds = Array.from(this.selectedGrades);
    gradeIds.forEach((id) => {
      // Găsește studentul care are această notă
      for (const cls of this.classes) {
        for (const student of cls.students) {
          const grade = student.grades.find(g => g.id === id);
          if (grade) {
            this.deleteGrade(grade, student); // Refolosim logica existentă
          }
        }
      }
    });

    this.selectedGrades.clear();
  }

  bulkEdit(): void {
    const gradeIds = Array.from(this.selectedGrades);
    for (const cls of this.classes) {
      for (const student of cls.students) {
        for (const grade of student.grades) {
          if (gradeIds.includes(grade.id)) {
            this.startEditGrade(grade);
          }
        }
      }
    }
  }


  cancelEditGrade(grade: Grade): void {
    grade.isEditing = false;
    grade.editValue = undefined;
    grade.editDate = undefined;
    grade.editAssignmentName = '';
  }


  saveEditGrade(grade: Grade): void {

    console.log('Saving grade:', grade.id);
    if (!grade.id) {
      console.error('Error: id is missing or undefined.', grade);
      this.errorMessage = 'Cannot update grade: Missing id.';
      return;
    }

    if (!grade.editValue) {
      console.warn('Grade value cannot be empty.');
      return;
    }

    const updatePayload: Partial<Grade> = {
      value: grade.editValue,
      date: new Date(grade.editDate!).toISOString(),
      assignments: grade.editAssignmentName
    };

    console.log('Updating grade:', grade.id, updatePayload);

    this.gradeService.updateGrade(grade.id, updatePayload).subscribe({
      next: (res: any) => {
        console.log('Grade updated successfully:', res.grade);

        grade.value = res.grade.value;
        grade.date = res.grade.date;
        grade.assignmentName = res.grade.assignmentName;

        this.cancelEditGrade(grade);

        console.log(grade)

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

  showInputField(classIndex: number, event: Event, mode: 'add' | 'delete') {
    event.stopPropagation();
    this.classes[classIndex].showInput = true;
    this.classes[classIndex].inputMode = mode;
    this.classes[classIndex].newStudentName = ''; // Reset input field
  }

  deleteGrade(grade: Grade, student: Student): void {
    if (!grade.id) return;

    if (!confirm('Are you sure you want to delete this grade?')) return;

    this.gradeService.deleteGrade(grade.id).subscribe({
      next: () => {
        console.log('Grade deleted successfully');

        student.grades = student.grades.filter(g => g.id !== grade.id);
      },
      error: (error) => {
        console.error('Error deleting grade:', error);
        this.errorMessage = `Error deleting grade: ${error?.error?.message || 'Unknown error'}`;
      }
    });
  }

  isGradeInvalid(value: string | number | undefined | null): boolean {
    if (value === null || value === undefined || value === '') return true;

    const strValue = String(value).trim();
    const num = Number(strValue);

    return isNaN(num) || num < 1 || num > 10 || !Number.isInteger(num);
  }



  addStudent(index: number, event: Event) {
    event.stopPropagation();

    const studentName = this.classes[index].newStudentName?.trim();
    const classId = this.classes[index].classId;

    if (!studentName) {
      alert('Student name cannot be empty.');
      return;
    }

    this.classService.addStudentToClass(classId, studentName).subscribe({
      next: (response) => {
        console.log(`Student added successfully via API: ${studentName}`, response);

        if (response.studentId) {
          const newStudent: Student = {
            name: studentName,
            grades: [],
            studentId: response.studentId,
          };
          this.classes[index].students.push(newStudent);
        } else {
          console.warn('Student added but no ID returned:', response);
        }

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

  deleteStudent(classIndex: number, event: Event) {
    event.stopPropagation();

    const studentName = this.classes[classIndex].newStudentName?.trim();
    if (!studentName) return;

    const student = this.classes[classIndex].students.find(s => s.name === studentName);
    if (!student) {
      alert("Student not found!");
      return;
    }

    const classId = this.classes[classIndex].classId;

    this.classService.deleteStudent(classId, student.studentId).subscribe({
      next: () => {
        console.log(`Student ${student.studentId} deleted from class ${classId}`);
        this.classes[classIndex].students = this.classes[classIndex].students.filter(s => s.studentId !== student.studentId);
        this.classes[classIndex].newStudentName = '';
        this.classes[classIndex].showInput = false;
      },
      error: (error) => {
        console.error('Error deleting student:', error);
        this.errorMessage = `Error deleting student: ${error?.error?.message || 'Unknown error'}`;
      }
    });
  }

  sortGradesByDate(student: any): void {
    if (student.grades && student.grades.length > 0) {
      student.grades.sort((a: any, b: any) => new Date(a.date).getTime() - new Date(b.date).getTime());
    }
  }
  handleGradeSubmit(studentId: number, classId: number) {
    if (!this.isAddingDescription[studentId]) {
      // First step: go to description input
      if (this.isGradeInvalid(this.newGradeValues[studentId])) {
        console.warn('Grade value is required.');
        return;
      }
      this.isAddingDescription[studentId] = true;
    } else {
      // Second step: submit with description
      const newGrade: Partial<Grade> = {
        teacherId: this.teacherId,
        studentId: studentId,
        classId: classId,
        value: this.newGradeValues[studentId],
        assignments: this.gradeDescriptions[studentId], // Add this to your backend model
        date: new Date().toISOString()
      };
  
      this.gradeService.addGrade(newGrade).subscribe({
        next: (response: any) => {
          console.log('Grade added successfully:', response);
  
          const classObj = this.classes.find(c => c.classId === classId);
          const studentObj = classObj?.students.find(s => s.studentId === studentId);
          studentObj?.grades.push(response.grade);
  
          // Reset all input states
          this.newGradeValues[studentId] = '';
          this.gradeDescriptions[studentId] = '';
          this.isAddingDescription[studentId] = false;
        },
        error: (error) => {
          console.error('Error adding grade:', error);
          this.errorMessage = `Error adding grade: ${error?.error?.message || 'Unknown error'}`;
        }
      });
    }
  }

  canSubmitGrade(studentId: number): boolean {
    if (!this.isAddingDescription[studentId]) {
      return !this.isGradeInvalid(this.newGradeValues[studentId]);
    } else {
      return !!this.gradeDescriptions[studentId]?.trim();
    }
  }
}