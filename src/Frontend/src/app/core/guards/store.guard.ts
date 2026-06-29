import { inject } from '@angular/core';
import { Router, type CanActivateFn } from '@angular/router';
import { StoreApiService } from '../services/api/store-api.service';
import { catchError, map, of } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';

export const storeGuard: CanActivateFn = () => {
  const storeApi = inject(StoreApiService);
  const router = inject(Router);

  return storeApi.getMyStore().pipe(
    map(() => true), // Store exists, allow navigation
    catchError((err: HttpErrorResponse) => {
      // If store not found, redirect to setup
      if (err.status === 404) {
        router.navigate(['/admin/setup-store']);
      }
      return of(false); // Cancel original navigation
    })
  );
};
