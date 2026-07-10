import {
  ApplicationConfig,
  provideBrowserGlobalErrorListeners,
  provideZonelessChangeDetection,
} from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

import { provideAuth0 } from '@auth0/auth0-angular';
import { routes } from './app.routes';
import { environment } from '../environments/environment';
import { errorInterceptor } from './core/http/error.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZonelessChangeDetection(),
    provideRouter(routes, withComponentInputBinding()),
    provideHttpClient(
      withInterceptors([errorInterceptor]),
    ),
    provideAuth0({
      domain: environment.auth0.domain,
      clientId: environment.auth0.clientId,
      authorizationParams: {
        redirect_uri: environment.auth0.redirectUri,
        audience: environment.auth0.audience,
        scope: 'openid profile email offline_access',
      },
      httpInterceptor: {
        allowedList: [`${environment.apiUrl}/*`],
      },
      useRefreshTokens: true,
      cacheLocation: 'localstorage',
    }),  ],
};
