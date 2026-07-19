import { Injectable, OnDestroy } from '@angular/core';
import { DeliveryApiService } from './api/delivery-api.service';
import { interval, Subscription, BehaviorSubject } from 'rxjs';

export interface Coordinates {
  lat: number;
  lng: number;
}

@Injectable({
  providedIn: 'root'
})
export class FakeGpsService implements OnDestroy {
  private subscription?: Subscription;
  private currentPosSub = new BehaviorSubject<Coordinates | null>(null);
  currentPos$ = this.currentPosSub.asObservable();
  
  constructor(private deliveryApi: DeliveryApiService) {}

  startTracking(courierUserId: string, origin: Coordinates, destination: Coordinates, durationMinutes: number = 2) {
    if (this.subscription) {
      this.stopTracking();
    }

    this.currentPosSub.next({ ...origin });
    const updateIntervalMs = 5000; // 5 seconds
    const totalUpdates = (durationMinutes * 60 * 1000) / updateIntervalMs;
    const latStep = (destination.lat - origin.lat) / totalUpdates;
    const lngStep = (destination.lng - origin.lng) / totalUpdates;
    let stepCount = 0;

    // Send initial location
    this.sendLocation(courierUserId, this.currentPosSub.value!);

    this.subscription = interval(updateIntervalMs).subscribe(() => {
      stepCount++;
      if (stepCount >= totalUpdates) {
        // We arrived!
        this.currentPosSub.next({ ...destination });
        this.sendLocation(courierUserId, this.currentPosSub.value!);
        this.stopTracking();
        return;
      }

      this.currentPosSub.next({
        lat: origin.lat + (latStep * stepCount),
        lng: origin.lng + (lngStep * stepCount)
      });

      this.sendLocation(courierUserId, this.currentPosSub.value!);
    });
  }

  stopTracking() {
    if (this.subscription) {
      this.subscription.unsubscribe();
      this.subscription = undefined;
    }
  }

  private sendLocation(courierUserId: string, pos: Coordinates) {
    this.deliveryApi.updateLocation({
      courierUserId: courierUserId,
      latitude: pos.lat,
      longitude: pos.lng
    }).subscribe({
      error: (err) => console.error('Failed to update fake GPS location', err)
    });
  }

  ngOnDestroy() {
    this.stopTracking();
  }
}
