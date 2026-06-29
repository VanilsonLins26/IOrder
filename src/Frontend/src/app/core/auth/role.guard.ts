import { inject } from '@angular/core';
import { Router, type CanActivateFn } from '@angular/router';
import { AuthService } from '@auth0/auth0-angular';
import { map } from 'rxjs';

export function roleGuard(requiredRole: string): CanActivateFn {
  return () => {
    const auth = inject(AuthService);
    const router = inject(Router);

    return auth.user$.pipe(
      map((user) => {
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
