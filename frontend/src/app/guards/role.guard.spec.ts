import { CanActivateFn, ActivatedRouteSnapshot } from '@angular/router';
import { inject } from '@angular/core';
import { Router } from '@angular/router';

export const roleGuard: CanActivateFn = (route: ActivatedRouteSnapshot) => {
  const router = inject(Router);
  const token = localStorage.getItem('token');

  if (!token) {
    router.navigate(['/login']);
    return false;
  }

  const payload = JSON.parse(atob(token.split('.')[1]));
  const userRole = payload['role'];
  const expectedRole = route.data['role'];

  if (userRole === expectedRole) {
    return true;
  }

  router.navigate(['/error']);
  return false;
};
