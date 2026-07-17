import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface GeoLocation {
  latitude: number;
  longitude: number;
}

@Injectable({ providedIn: 'root' })
export class GeolocationService {
  private cachedPosition: GeoLocation | null = null;

  getCurrentPosition(): Observable<GeoLocation> {
    return new Observable(subscriber => {
      if (this.cachedPosition) {
        subscriber.next(this.cachedPosition);
        subscriber.complete();
        return;
      }

      if (!navigator.geolocation) {
        subscriber.error(new Error('Geolocalização não suportada pelo navegador.'));
        return;
      }

      navigator.geolocation.getCurrentPosition(
        (position) => {
          const loc: GeoLocation = {
            latitude: position.coords.latitude,
            longitude: position.coords.longitude,
          };
          this.cachedPosition = loc;
          subscriber.next(loc);
          subscriber.complete();
        },
        (error) => {
          switch (error.code) {
            case error.PERMISSION_DENIED:
              subscriber.error(new Error('Permissão de localização negada.'));
              break;
            case error.POSITION_UNAVAILABLE:
              subscriber.error(new Error('Localização indisponível.'));
              break;
            case error.TIMEOUT:
              subscriber.error(new Error('Timeout ao obter localização.'));
              break;
            default:
              subscriber.error(new Error('Erro ao obter localização.'));
              break;
          }
        },
        { enableHighAccuracy: false, timeout: 10000, maximumAge: 300000 }
      );
    });
  }
}
