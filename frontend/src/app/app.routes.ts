import { Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { TeacherComponent } from './teacher/teacher.component';
import { StudentComponent } from './student/student.component';
import { ErrorComponent } from './error/error.component';
import { RegisterComponent } from './register/register.component';
import { AuthGuard } from './guards/auth.guard';
import { RoleGuard } from './guards/role.guard';
import { RecoverPasswordComponent } from './recover-password/recover-password.component';

export const routes: Routes = [
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent},

    {
        path: 'student',
        component: StudentComponent,
        canActivate: [AuthGuard, RoleGuard],
        data: { role: 'Student' }
      },
      {
        path: 'teacher',
        component: TeacherComponent,
        canActivate: [AuthGuard, RoleGuard],  
        data: { role: 'Teacher' }
      },
      { path: 'error', component: ErrorComponent },
      { path: '', redirectTo: 'login', pathMatch: 'full' },
    { path: 'recover', component: RecoverPasswordComponent},
    { path: 'error', component: ErrorComponent},
    { path: '', redirectTo: 'login', pathMatch: 'full' },
];
  