import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../auth/auth.service';

export function roleGuard(...requiredRoles: string[]): CanActivateFn {
  return () => {
    const authService = inject(AuthService);
    const router = inject(Router);

    if (!authService.isAuthenticated()) {
      return router.createUrlTree(['/auth/login']);
    }

    const userRoles = authService.currentUser()?.roles ?? [];
    const hasRole = requiredRoles.some(r => userRoles.includes(r));

    if (hasRole) return true;

    return router.createUrlTree(['/dashboard']);
  };
}
