import { UserRole } from './userRole.model';

export interface JwtPayload {
    nameid: string;
    unique_name: string;
    role: UserRole;
    exp: number;
}