export interface Grade {
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
    assignments?: string;
    editAssignmentName?: string;
  }