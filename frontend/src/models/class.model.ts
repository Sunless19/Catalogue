import { Student } from '../models/student.model';

export interface Class {
  name: string;
  students: Student[];
  expanded: boolean;
  showInput?: boolean;
  newStudentName?: string;
  userId: number;
  classId: number;
  inputMode: string;
}