import { inject } from '@angular/core';
import { Router, type CanActivateFn } from '@angular/router';
import { AuthService } from '@auth0/auth0-angular';
import { map } from 'rxjs';
import { Roles } from './role.guard';

export const roleRedirectGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  return auth.user$.pipe(
    map((user) => {
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
