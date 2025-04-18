import { Routes } from '@angular/router';

import { LoginComponent } from '@components/login/login.component';
import { TeacherComponent } from '@components/teacher/teacher.component';
import { StudentComponent } from '@components/student/student.component';
import { ErrorComponent } from '@components/error/error.component';
import { RegisterComponent } from '@components/register/register.component';
import { RecoverPasswordComponent } from '@components/recover-password/recover-password.component';
import { ResetPasswordComponent } from '@components/reset-password/reset-password.component';

import { AuthGuard } from '@guards/auth.guard';
import { RoleGuard } from '@guards/role.guard';

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
    { path: 'reset-password', component: ResetPasswordComponent },
];
  