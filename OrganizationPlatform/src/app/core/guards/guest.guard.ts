import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../auth/auth.service';

export const guestGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router      = inject(Router);

  if (!authService.isAuthenticated()) return true;

  const isSystemAdmin = authService.currentUser()?.roles.includes('SystemAdmin') ?? false;
  return router.createUrlTree([isSystemAdmin ? '/admin' : '/dashboard']);
};
