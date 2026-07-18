import { inject } from '@angular/core';
import { Router, type CanActivateFn } from '@angular/router';
import { AuthService } from '@auth0/auth0-angular';
import { combineLatest, filter, map, take } from 'rxjs';
import { Roles } from './role.guard';

export const roleRedirectGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  return combineLatest([auth.isLoading$, auth.user$]).pipe(
    filter(([isLoading]) => !isLoading),
    take(1),
    map(([_, user]) => {
      if (!user) return true;

      const roles: string[] =
        user?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ?? [];

      if (roles.includes(Roles.ShopKeeper)) {
        return router.createUrlTree(['/admin']);
      }

      if (roles.includes(Roles.Delivery)) {
        return router.createUrlTree(['/courier']);
      }

      return true;
    }),
  );
};
