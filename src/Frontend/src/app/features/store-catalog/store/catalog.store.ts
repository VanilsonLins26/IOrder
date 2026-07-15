import { computed, inject } from '@angular/core';
import { signalStore, withState, withComputed, withMethods, patchState } from '@ngrx/signals';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { pipe, switchMap, tap, catchError, of } from 'rxjs';
import { StoreApiService } from '../../../core/services/api/store-api.service';
import { StoreCategoryApiService } from '../../../core/services/api/store-category-api.service';
import { GeolocationService } from '../../../core/services/geolocation.service';
import type { StoreResponse } from '../../../core/models/store.model';
import type { StoreCategoryResponse } from '../../../core/models/store-category.model';
import type { StoreSearchQuery } from '../../../core/models/api-response.model';

export interface CatalogState {
  stores: StoreResponse[];
  categories: StoreCategoryResponse[];
  status: 'idle' | 'loading' | 'loaded' | 'error';
  searchQuery: string;
  selectedCategoryId: string | null;
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  latitude: number | null;
  longitude: number | null;
}

const initialState: CatalogState = {
  stores: [],
  categories: [],
  status: 'idle',
  searchQuery: '',
  selectedCategoryId: null,
  pageNumber: 1,
  pageSize: 12,
  totalCount: 0,
  latitude: null,
  longitude: null,
};

export const CatalogStore = signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withComputed((store) => ({
    isLoading: computed(() => store.status() === 'loading'),
    hasLocation: computed(() => store.latitude() !== null && store.longitude() !== null),
    filteredStores: computed(() => {
      const q = store.searchQuery().toLowerCase().trim();
      const catId = store.selectedCategoryId();

      return store.stores().filter(s => {
        const matchName = !q || s.name.toLowerCase().includes(q);
        const matchCat = !catId || s.categoryId === catId;
        return matchName && matchCat;
      });
    }),
  })),
  withMethods((store, storeApi = inject(StoreApiService), categoryApi = inject(StoreCategoryApiService), geoService = inject(GeolocationService)) => ({

    setSearchQuery(query: string) {
      patchState(store, { searchQuery: query });
    },

    setCategory(categoryId: string | null) {
      patchState(store, { selectedCategoryId: categoryId });
    },

    setPage(pageNumber: number) {
      patchState(store, { pageNumber });
    },

    loadCategories: rxMethod<void>(
      pipe(
        switchMap(() => {
          return categoryApi.getAll().pipe(
            tap((categories) => patchState(store, { categories })),
            catchError(() => of([]))
          );
        })
      )
    ),

    loadUserLocation() {
      geoService.getCurrentPosition().subscribe({
        next: (loc) => {
          patchState(store, { latitude: loc.latitude, longitude: loc.longitude });
        },
        error: () => {
          patchState(store, { latitude: null, longitude: null });
        },
      });
    },

    loadStores: rxMethod<StoreSearchQuery>(
      pipe(
        tap(() => patchState(store, { status: 'loading' })),
        switchMap((query) => {
          const lat = store.latitude();
          const lon = store.longitude();
          const enrichedQuery: StoreSearchQuery = {
            ...query,
            ...(lat !== null && lon !== null ? { userLatitude: lat, userLongitude: lon } : {}),
          };

          return storeApi.getPaged(enrichedQuery).pipe(
            tap((response) => {
              patchState(store, {
                stores: response.items,
                status: 'loaded',
                totalCount: response.totalCount || response.items.length
              });
            }),
            catchError(() => {
              patchState(store, { status: 'error' });
              return of([]);
            })
          );
        })
      )
    ),
  }))
);
