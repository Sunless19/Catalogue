import { Injectable } from '@angular/core';
import { HttpHeaders } from '@angular/common/http';

import { UserRole } from '@models/userRole.model';
import { JwtPayload } from '@models/jwtPayload.model';

import { jwtDecode } from 'jwt-decode';

@Injectable({ providedIn: 'root' })
export class UserService {
    getToken(): string | null {
        return localStorage.getItem('token');
    }

    getAuthHeaders(): HttpHeaders {
        const token = this.getToken();
        return token ? new HttpHeaders().set('Authorization', `Bearer ${token}`) : new HttpHeaders();
    }

    getTeacherId(): number {
        const token = this.getToken();
        if (!token) return 0;
        try {
            const decoded = jwtDecode<JwtPayload>(token);
            return parseInt(decoded.nameid, 10) || 0;
        } catch {
            return 0;
        }
    }

    getUserRole(): UserRole | null {
        const token = this.getToken();
        try {
            return token ? jwtDecode<JwtPayload>(token).role : null;
        } catch {
            return null;
        }
    }

    isStudent(): boolean {
        return this.getUserRole() === UserRole.Student;
    }

    isTeacher(): boolean {
        return this.getUserRole() === UserRole.Teacher;
    }
}
