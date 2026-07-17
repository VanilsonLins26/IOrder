import { inject } from '@angular/core';
import { Router, type CanActivateFn } from '@angular/router';
import { StoreApiService } from '../services/api/store-api.service';
import { AdminStore } from '../../features/admin/store/admin.store';
import { catchError, map, of } from 'rxjs';

export const storeGuard: CanActivateFn = () => {
  const adminStore = inject(AdminStore);
  const storeApi = inject(StoreApiService);
  const router = inject(Router);

  if (adminStore.myStore()) {
    return true;
  }

  return storeApi.getMyStore().pipe(
    map(() => true),
    catchError(() => {
      router.navigate(['/admin/setup-store']);
      return of(false);
    })
  );
};
