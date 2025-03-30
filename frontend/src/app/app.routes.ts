import { Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { TeacherComponent } from './teacher/teacher.component';
import { StudentComponent } from './student/student.component';
import { ErrorComponent } from './error/error.component';

export const routes: Routes = [
    { path: 'login', component: LoginComponent },
    { path: 'teacher', component: TeacherComponent},
    { path: 'student', component: StudentComponent},
    { path: 'error', component: ErrorComponent},
    { path: '', redirectTo: 'login', pathMatch: 'full' },
];
  