import { Grade } from '../models/grade.model';

export interface Student {
  name: string;
  grades: Grade[];
  studentId: number;
}