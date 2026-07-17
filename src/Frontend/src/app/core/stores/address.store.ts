import { computed, inject } from '@angular/core';
import { signalStore, withState, withComputed, withMethods, patchState } from '@ngrx/signals';
import { AuthService } from '@auth0/auth0-angular';
import { switchMap, take, tap } from 'rxjs';
import { UserAddressApiService } from '../services/api/user-address-api.service';
import { GeolocationService } from '../services/geolocation.service';
import type { UserAddressResponse } from '../models';

export interface AddressState {
  addresses: UserAddressResponse[];
  selectedAddress: UserAddressResponse | null;
  latitude: number | null;
  longitude: number | null;
  isUsingGeolocation: boolean;
  status: 'idle' | 'loading' | 'loaded' | 'error';
}

const initialState: AddressState = {
  addresses: [],
  selectedAddress: null,
  latitude: null,
  longitude: null,
  isUsingGeolocation: false,
  status: 'idle',
};

export const AddressStore = signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withComputed((store) => ({
    hasLocation: computed(() => store.latitude() !== null && store.longitude() !== null),
    addressSummary: computed(() => {
      const addr = store.selectedAddress();
      if (!addr) return null;
      const parts = [addr.street, addr.number].filter(Boolean);
      return `${parts.join(', ')} - ${addr.neighborhood}, ${addr.city}`;
    }),
  })),
  withMethods((store, auth = inject(AuthService), addressApi = inject(UserAddressApiService), geoService = inject(GeolocationService)) => ({

    loadAddresses() {
      auth.isAuthenticated$.pipe(
        take(1),
        switchMap(isAuth => {
          if (!isAuth) return [];
          patchState(store, { status: 'loading' });
          return addressApi.getAll();
        })
      ).subscribe({
        next: (addresses) => {
          if (addresses.length === 0) {
            patchState(store, { addresses: [], status: 'loaded' });
            return;
          }
          const defaultAddr = addresses.find(a => a.isDefault) ?? addresses[0];
          patchState(store, {
            addresses,
            selectedAddress: defaultAddr,
            latitude: defaultAddr.latitude,
            longitude: defaultAddr.longitude,
            isUsingGeolocation: false,
            status: 'loaded',
          });
        },
        error: () => {
          patchState(store, { status: 'error' });
        },
      });
    },

    selectAddress(address: UserAddressResponse) {
      patchState(store, {
        selectedAddress: address,
        latitude: address.latitude,
        longitude: address.longitude,
        isUsingGeolocation: false,
      });
    },

    useCurrentLocation() {
      geoService.getCurrentPosition().subscribe({
        next: (loc) => {
          patchState(store, {
            latitude: loc.latitude,
            longitude: loc.longitude,
            isUsingGeolocation: true,
          });
        },
        error: () => {
          patchState(store, { latitude: null, longitude: null });
        },
      });
    },
  }))
);
