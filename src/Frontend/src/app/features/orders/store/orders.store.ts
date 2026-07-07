import { inject } from '@angular/core';
import { signalStore, withState, withMethods, patchState } from '@ngrx/signals';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { pipe, switchMap, tap } from 'rxjs';
import { tapResponse } from '@ngrx/operators';
import { OrderApiService } from '../../../core/services/api/order-api.service';
import { ToastService } from '../../../core/services/toast.service';
import type { OrderResponseDto, PagedList } from '../../../core/models';

type OrdersState = {
  orders: OrderResponseDto[];
  currentPage: number;
  totalPages: number;
  totalCount: number;
  pageSize: number;
  loading: boolean;
};

const initialState: OrdersState = {
  orders: [],
  currentPage: 1,
  totalPages: 0,
  totalCount: 0,
  pageSize: 10,
  loading: false,
};

export const OrdersStore = signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withMethods((store, orderApi = inject(OrderApiService), toast = inject(ToastService)) => ({
    loadUserOrders: rxMethod<{ page: number }>(
      pipe(
        tap(() => patchState(store, { loading: true })),
        switchMap(({ page }) =>
          orderApi.getUserOrders(page, store.pageSize()).pipe(
            tapResponse({
              next: (data: PagedList<OrderResponseDto>) =>
                patchState(store, {
                  orders: data.items,
                  currentPage: data.currentPage,
                  totalPages: data.totalPages,
                  totalCount: data.totalCount,
                  loading: false,
                }),
              error: () => {
                patchState(store, { loading: false });
                toast.error('Erro ao carregar pedidos.');
              },
            }),
          ),
        ),
      ),
    ),

    loadStoreOrders: rxMethod<{ page: number }>(
      pipe(
        tap(() => patchState(store, { loading: true })),
        switchMap(({ page }) =>
          orderApi.getStoreOrders(page, store.pageSize()).pipe(
            tapResponse({
              next: (data: PagedList<OrderResponseDto>) =>
                patchState(store, {
                  orders: data.items,
                  currentPage: data.currentPage,
                  totalPages: data.totalPages,
                  totalCount: data.totalCount,
                  loading: false,
                }),
              error: () => {
                patchState(store, { loading: false });
                toast.error('Erro ao carregar pedidos.');
              },
            }),
          ),
        ),
      ),
    ),

    goToPage: (page: number) => {
      patchState(store, { currentPage: page });
    },
  })),
);
