import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '@auth0/auth0-angular';
import { catchError, switchMap, take, of } from 'rxjs';
import { environment } from '../../../environments/environment';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);

  // Only intercept requests going to our API
  if (!req.url.startsWith(environment.apiUrl)) {
    return next(req);
  }

  // Check authentication state FIRST (synchronously via snapshot).
  // Only fetch a token if the user is already authenticated.
  // This avoids the ~60s Auth0 silent-auth timeout on public endpoints.
  return auth.isAuthenticated$.pipe(
    take(1),
    switchMap((isAuthenticated) => {
      if (!isAuthenticated) {
        // User is not logged in → send request without Authorization header
        return next(req);
      }

      // User is logged in → attach the JWT Bearer token
      return auth.getAccessTokenSilently().pipe(
        take(1),
        switchMap((token) =>
          next(
            req.clone({
              setHeaders: { Authorization: `Bearer ${token}` },
            }),
          ),
        ),
        catchError((err) => {
          // If silent token fetch fails (e.g., missing refresh token, expired session),
          // proceed without a token so public endpoints don't break.
          // Private endpoints will return 401, handled by errorInterceptor.
          console.warn('AuthInterceptor: Failed to get token silently, proceeding without token.', err);
          return next(req);
        })
      );
    }),
  );
};
