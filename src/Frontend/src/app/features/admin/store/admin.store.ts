import { inject } from '@angular/core';
import { signalStore, withState, withMethods, patchState, withHooks } from '@ngrx/signals';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { pipe, tap, switchMap, catchError, of, forkJoin } from 'rxjs';
import { tapResponse } from '@ngrx/operators';
import { StoreApiService } from '../../../core/services/api/store-api.service';
import { CategoryApiService } from '../../../core/services/api/category-api.service';
import { ProductApiService } from '../../../core/services/api/product-api.service';
import type { StoreResponse, CategoryResponse, ProductResponse, PagedList } from '../../../core/models';

type AdminState = {
  myStore: StoreResponse | null;
  categories: CategoryResponse[];
  products: ProductResponse[];
  loading: boolean;
  error: string | null;
};

const initialState: AdminState = {
  myStore: null,
  categories: [],
  products: [],
  loading: false,
  error: null,
};

export const AdminStore = signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withMethods((store, storeApi = inject(StoreApiService), categoryApi = inject(CategoryApiService), productApi = inject(ProductApiService)) => ({
    
    // --- LOAD ALL DATA ---
    loadAdminData: rxMethod<void>(
      pipe(
        tap(() => patchState(store, { loading: true, error: null })),
        switchMap(() => {
          return storeApi.getMyStore().pipe(
            switchMap((myStore) => {
              // Now fetch categories and products in parallel using the store ID
              return forkJoin({
                myStore: of(myStore),
                categories: categoryApi.getByStoreId(myStore.id),
                productsPage: productApi.getPaged({ storeId: myStore.id, pageSize: 100 }) // Fetching up to 100 for admin grid
              });
            }),
            tapResponse({
              next: ({ myStore, categories, productsPage }) => {
                patchState(store, {
                  myStore,
                  categories: categories.sort((a, b) => a.position - b.position),
                  products: productsPage.items,
                  loading: false,
                });
              },
              error: (err: any) => {
                patchState(store, {
                  error: err?.message || 'Erro ao carregar dados da loja.',
                  loading: false,
                });
              },
            })
          );
        })
      )
    ),

    // --- STORE MUTATIONS ---
    updateStoreInfo: (updatedStore: StoreResponse) => {
      patchState(store, { myStore: updatedStore });
    },

    // --- CATEGORY MUTATIONS ---
    addCategory: (category: CategoryResponse) => {
      patchState(store, (state) => ({ categories: [...state.categories, category] }));
    },
    updateCategory: (category: CategoryResponse) => {
      patchState(store, (state) => ({
        categories: state.categories.map((c) => (c.id === category.id ? category : c)),
      }));
    },
    deleteCategory: (id: string) => {
      patchState(store, (state) => ({
        categories: state.categories.filter((c) => c.id !== id),
      }));
    },
    setCategories: (categories: CategoryResponse[]) => {
      patchState(store, { categories });
    },

    // --- PRODUCT MUTATIONS ---
    addProduct: (product: ProductResponse) => {
      patchState(store, (state) => ({ products: [...state.products, product] }));
    },
    updateProduct: (product: ProductResponse) => {
      patchState(store, (state) => ({
        products: state.products.map((p) => (p.id === product.id ? product : p)),
      }));
    },
    deleteProduct: (id: string) => {
      patchState(store, (state) => ({
        products: state.products.filter((p) => p.id !== id),
      }));
    },
  })),
  withHooks({
    onInit(store) {
      // Optional: automatically load if auth is ready.
      // For now, we will call loadAdminData from the Layout or Dashboard.
    },
  })
);
