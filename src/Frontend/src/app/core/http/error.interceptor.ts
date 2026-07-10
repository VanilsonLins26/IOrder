import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '@auth0/auth0-angular';
import { catchError, throwError } from 'rxjs';
import { ToastService } from '../services/toast.service';
import type { ResponseError } from '../models';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const toast = inject(ToastService);
  const auth = inject(AuthService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      switch (error.status) {
        case 0:
          toast.error(
            'Sem conexão com o servidor. Verifique sua internet.',
          );
          break;

        case 400: {
          const body = error.error as ResponseError;
          if (body?.errors?.length) {
            body.errors.forEach((msg) => toast.error(msg));
          } else {
            toast.error('Dados inválidos. Verifique os campos e tente novamente.');
          }
          break;
        }

        case 401:
          if (req.url.toLowerCase().includes('mystore')) {
            break;
          }
          toast.warning('Sessão expirada. Redirecionando para o login...');
          auth.logout({ logoutParams: { returnTo: window.location.origin } });
          break;

        case 403:
          toast.error('Você não tem permissão para realizar esta ação.');
          break;

        case 404:
          // Ignore 404 for mystore, the store guard will handle it
          if (!req.url.toLowerCase().includes('mystore')) {
            toast.error('Recurso não encontrado.');
          }
          break;

        case 409:
          toast.error('Conflito: este recurso já existe.');
          break;

        case 422: {
          const body422 = error.error as ResponseError;
          if (body422?.errors?.length) {
            body422.errors.forEach((msg) => toast.error(msg));
          } else {
            toast.error('Erro de validação.');
          }
          break;
        }

        case 500:
          toast.error(
            'Erro interno do servidor. Tente novamente mais tarde.',
          );
          break;

        default:
          toast.error('Ocorreu um erro inesperado.');
          break;
      }

      return throwError(() => error);
    }),
  );
};
