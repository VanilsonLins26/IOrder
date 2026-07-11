import { inject } from '@angular/core';
import { signalStore, withState, withMethods, patchState, withComputed } from '@ngrx/signals';
import { computed } from '@angular/core';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { pipe, switchMap, tap, catchError, of } from 'rxjs';
import { tapResponse } from '@ngrx/operators';
import { CartApiService } from '../../../core/services/api/cart-api.service';
import { ToastService } from '../../../core/services/toast.service';
import type { CartItemResponseDto, CartResponseDto } from '../../../core/models';

type CartState = {
  items: CartItemResponseDto[];
  couponCode: string | null;
  cartTotal: number;
  discountValue: number | null;
  discountedTotal: number | null;
  loading: boolean;
  error: string | null;
};

const initialState: CartState = {
  items: [],
  couponCode: null,
  cartTotal: 0,
  discountValue: null,
  discountedTotal: null,
  loading: false,
  error: null,
};

export const CartStore = signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withComputed(({ items }) => ({
    itemCount: computed(() => items().reduce((sum, item) => sum + item.quantity, 0)),
  })),
  withMethods((store, cartApi = inject(CartApiService), toast = inject(ToastService)) => ({

    loadCart: rxMethod<void>(
      pipe(
        tap(() => patchState(store, { loading: true, error: null })),
        switchMap(() =>
          cartApi.get().pipe(
            tapResponse({
              next: (cart) => patchState(store, {
                items: cart.items,
                couponCode: cart.couponCode,
                cartTotal: cart.cartTotal,
                discountValue: cart.discountValue,
                discountedTotal: cart.discountedTotal,
                loading: false,
              }),
              error: (err: any) => {
                patchState(store, { loading: false, error: err?.message || null });
                if (err.status !== 404) {
                  toast.error('Erro ao carregar carrinho.');
                }
              },
            }),
          ),
        ),
      ),
    ),

    addItem: rxMethod<{ productId: string; quantity: number; customize?: string; imageUrls?: string[]; selectedOptionIds?: string[] }>(
      pipe(
        tap(() => patchState(store, { loading: true })),
        switchMap(({ productId, quantity, customize, imageUrls, selectedOptionIds }) =>
          cartApi.addItem({
            productId,
            quantity,
            customize: customize || '',
            imageUrls: imageUrls || [],
            selectedOptionIds: selectedOptionIds || [],
          }).pipe(
            tapResponse({
              next: (cart) => {
                patchState(store, {
                  items: cart.items,
                  couponCode: cart.couponCode,
                  cartTotal: cart.cartTotal,
                  discountValue: cart.discountValue,
                  discountedTotal: cart.discountedTotal,
                  loading: false,
                });
                toast.success('Item adicionado ao carrinho!');
              },
              error: (err: any) => {
                patchState(store, { loading: false, error: err?.message || null });
                toast.error(err.error?.errors?.[0] || 'Erro ao adicionar item.');
              },
            }),
          ),
        ),
      ),
    ),

    changeQuantity: rxMethod<{ cartItemId: string; newQuantity: number }>(
      pipe(
        tap(() => patchState(store, { loading: true })),
        switchMap(({ cartItemId, newQuantity }) =>
          cartApi.changeQuantity({ cartItemId, newQuantity }).pipe(
            tapResponse({
              next: (cart) => patchState(store, {
                items: cart.items,
                couponCode: cart.couponCode,
                cartTotal: cart.cartTotal,
                discountValue: cart.discountValue,
                discountedTotal: cart.discountedTotal,
                loading: false,
              }),
              error: (err: any) => {
                patchState(store, { loading: false, error: err?.message || null });
                toast.error(err.error?.errors?.[0] || 'Erro ao alterar quantidade.');
              },
            }),
          ),
        ),
      ),
    ),

    applyCoupon: rxMethod<string>(
      pipe(
        tap(() => patchState(store, { loading: true })),
        switchMap((couponCode) =>
          cartApi.applyCoupon({ couponCode }).pipe(
            tapResponse({
              next: (cart) => {
                patchState(store, {
                  couponCode: cart.couponCode,
                  cartTotal: cart.cartTotal,
                  discountValue: cart.discountValue,
                  discountedTotal: cart.discountedTotal,
                  loading: false,
                });
                toast.success('Cupom aplicado com sucesso!');
              },
              error: (err: any) => {
                patchState(store, { loading: false, error: err?.message || null });
                toast.error(err.error?.errors?.[0] || 'Erro ao aplicar cupom.');
              },
            }),
          ),
        ),
      ),
    ),

    removeItem: rxMethod<string>(
      pipe(
        tap(() => patchState(store, { loading: true })),
        switchMap((cartItemId) =>
          cartApi.removeItem(cartItemId).pipe(
            tapResponse({
              next: (cart) => patchState(store, {
                items: cart.items,
                cartTotal: cart.cartTotal,
                couponCode: cart.couponCode,
                discountValue: cart.discountValue,
                discountedTotal: cart.discountedTotal,
                loading: false,
              }),
              error: (err: any) => {
                patchState(store, { loading: false, error: err?.message || null });
                toast.error(err.error?.errors?.[0] || 'Erro ao remover item.');
              },
            }),
          ),
        ),
      ),
    ),

    clearCart: rxMethod<void>(
      pipe(
        tap(() => patchState(store, { loading: true })),
        switchMap(() =>
          cartApi.clear().pipe(
            tapResponse({
              next: () => {
                patchState(store, { ...initialState });
                toast.success('Carrinho limpo!');
              },
              error: (err: any) => {
                patchState(store, { loading: false, error: err?.message || null });
                toast.error('Erro ao limpar carrinho.');
              },
            }),
          ),
        ),
      ),
    ),

    resetCart: () => patchState(store, { ...initialState }),
  })),
);
