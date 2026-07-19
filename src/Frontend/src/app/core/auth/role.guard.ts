import { inject } from '@angular/core';
import { Router, type CanActivateFn } from '@angular/router';
import { AuthService } from '@auth0/auth0-angular';
import { filter, map, switchMap, take, of } from 'rxjs';

export function getUserRoles(user: any): string[] {
  if (!user) return [];
  const rawRoles = user['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] 
                || user['https://iorder.com/roles']
                || user['roles']
                || user['role']
                || [];
  if (Array.isArray(rawRoles)) return rawRoles;
  if (typeof rawRoles === 'string') return [rawRoles];
  return [];
}

export function roleGuard(requiredRole: string): CanActivateFn {
  return () => {
    const auth = inject(AuthService);
    const router = inject(Router);

    return auth.isLoading$.pipe(
      filter(isLoading => !isLoading),
      switchMap(() => auth.isAuthenticated$),
      take(1),
      switchMap(isAuthenticated => {
        if (!isAuthenticated) {
          router.navigate(['/']);
          return of(false);
        }
        return auth.user$.pipe(
          filter(user => !!user),
          take(1),
          map(user => {
            const roles = getUserRoles(user);
            if (roles.includes(requiredRole)) return true;
            router.navigate(['/']);
            return false;
          })
        );
      })
    );
  };
}

export const Roles = {
  ShopKeeper: 'ShopKeeper',
  Client: 'Client',
  Delivery: 'DeliveryPerson',
} as const;
