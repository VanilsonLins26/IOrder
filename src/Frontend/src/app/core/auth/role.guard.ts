import { inject } from '@angular/core';
import { Router, type CanActivateFn } from '@angular/router';
import { AuthService } from '@auth0/auth0-angular';
import { combineLatest, filter, map, take } from 'rxjs';

export function roleGuard(requiredRole: string): CanActivateFn {
  return () => {
    const auth = inject(AuthService);
    const router = inject(Router);

    return combineLatest([auth.isLoading$, auth.user$]).pipe(
      filter(([isLoading]) => !isLoading),
      take(1),
      map(([_, user]) => {
        const roles: string[] =
          user?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ?? [];

        if (roles.includes(requiredRole)) {
          return true;
        }

        router.navigate(['/']);
        return false;
      }),
    );
  };
}

export const Roles = {
  ShopKeeper: 'ShopKeeper',
  Client: 'Client',
  Delivery: 'Delivery',
} as const;
