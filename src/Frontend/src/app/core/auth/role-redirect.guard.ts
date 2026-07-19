import { inject } from '@angular/core';
import { Router, type CanActivateFn } from '@angular/router';
import { AuthService } from '@auth0/auth0-angular';
import { filter, map, switchMap, take, of } from 'rxjs';
import { Roles, getUserRoles } from './role.guard';

export const roleRedirectGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  return auth.isLoading$.pipe(
    filter(isLoading => !isLoading),
    switchMap(() => auth.isAuthenticated$),
    take(1),
    switchMap(isAuthenticated => {
      if (!isAuthenticated) return of(true);
      return auth.user$.pipe(
        filter(user => !!user),
        take(1),
        map(user => {
          const roles = getUserRoles(user);
          if (roles.includes(Roles.ShopKeeper)) return router.createUrlTree(['/admin']);
          if (roles.includes(Roles.Delivery)) return router.createUrlTree(['/courier']);
          return true;
        })
      );
    })
  );
};
