import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const roleGuard = (allowedRoles: string[]) => {
  return () => {
    const authService = inject(AuthService);
    const router = inject(Router);

    const userRole = authService.getUserRole();
    
    if (authService.isAuthenticated() && userRole && allowedRoles.includes(userRole)) {
      return true;
    }

    router.navigate(['/dashboard']);
    return false;
  };
};