import { Component, ChangeDetectionStrategy, AfterViewInit, OnDestroy, input, effect, ElementRef, ViewChild, OnChanges, SimpleChanges } from '@angular/core';
import * as L from 'leaflet';
import { Coordinates } from '../../../core/services/fake-gps.service';

@Component({
  selector: 'app-delivery-map',
  standalone: true,
  template: `
    <div class="map-container">
      <div #mapElement class="map-frame"></div>
    </div>
  `,
  styles: [`
    .map-container {
      width: 100%;
      height: 300px;
      border-radius: 16px;
      overflow: hidden;
      box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
      position: relative;
      background: #f8fafc;
      border: 1px solid #e2e8f0;
      z-index: 1;
    }
    .map-frame {
      width: 100%;
      height: 100%;
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DeliveryMapComponent implements AfterViewInit, OnDestroy, OnChanges {
  readonly storeLocation = input.required<Coordinates>();
  readonly clientLocation = input.required<Coordinates>();
  readonly courierLocation = input<Coordinates | null>(null);

  @ViewChild('mapElement') mapElement!: ElementRef;
  
  private map: L.Map | null = null;
  private storeMarker: L.Marker | null = null;
  private clientMarker: L.Marker | null = null;
  private courierMarker: L.Marker | null = null;
  private routeLine: L.Polyline | null = null;

  ngAfterViewInit() {
    this.initMap();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (this.map) {
      if (changes['courierLocation'] && this.courierLocation()) {
        this.updateCourierPosition(this.courierLocation()!);
      }
    }
  }

  ngOnDestroy() {
    if (this.map) {
      this.map.remove();
      this.map = null;
    }
  }

  private initMap() {
    const store = this.storeLocation();
    const client = this.clientLocation();
    
    // Calculate center
    const centerLat = (store.lat + client.lat) / 2;
    const centerLng = (store.lng + client.lng) / 2;

    this.map = L.map(this.mapElement.nativeElement, {
      zoomControl: false,
      attributionControl: false
    }).setView([centerLat, centerLng], 14);

    L.tileLayer('https://{s}.basemaps.cartocdn.com/rastertiles/voyager/{z}/{x}/{y}{r}.png', {
      maxZoom: 19,
    }).addTo(this.map);

    // Custom Icons
    const storeIcon = L.divIcon({
      html: '<div style="font-size: 24px; text-shadow: 0 2px 4px rgba(0,0,0,0.2);">🏪</div>',
      className: 'custom-leaflet-icon',
      iconSize: [30, 30],
      iconAnchor: [15, 15]
    });

    const clientIcon = L.divIcon({
      html: '<div style="font-size: 24px; text-shadow: 0 2px 4px rgba(0,0,0,0.2);">🏠</div>',
      className: 'custom-leaflet-icon',
      iconSize: [30, 30],
      iconAnchor: [15, 15]
    });

    const courierIcon = L.divIcon({
      html: '<div style="font-size: 28px; text-shadow: 0 2px 4px rgba(0,0,0,0.3); transform: scaleX(-1);">🛵</div>',
      className: 'custom-leaflet-icon courier-icon',
      iconSize: [34, 34],
      iconAnchor: [17, 17]
    });

    this.storeMarker = L.marker([store.lat, store.lng], { icon: storeIcon }).addTo(this.map);
    this.clientMarker = L.marker([client.lat, client.lng], { icon: clientIcon }).addTo(this.map);

    // Draw dotted line
    this.routeLine = L.polyline([[store.lat, store.lng], [client.lat, client.lng]], {
      color: '#3b82f6',
      weight: 3,
      dashArray: '5, 10',
      opacity: 0.6
    }).addTo(this.map);

    // Fit bounds to show both
    this.map.fitBounds(L.latLngBounds([
      [store.lat, store.lng],
      [client.lat, client.lng]
    ]), { padding: [30, 30] });

    if (this.courierLocation()) {
      this.courierMarker = L.marker([this.courierLocation()!.lat, this.courierLocation()!.lng], { icon: courierIcon }).addTo(this.map);
    }
  }

  private updateCourierPosition(pos: Coordinates) {
    if (!this.map) return;
    
    if (this.courierMarker) {
      this.courierMarker.setLatLng([pos.lat, pos.lng]);
    } else {
      const courierIcon = L.divIcon({
        html: '<div style="font-size: 28px; text-shadow: 0 2px 4px rgba(0,0,0,0.3); transform: scaleX(-1);">🛵</div>',
        className: 'custom-leaflet-icon courier-icon',
        iconSize: [34, 34],
        iconAnchor: [17, 17]
      });
      this.courierMarker = L.marker([pos.lat, pos.lng], { icon: courierIcon }).addTo(this.map);
    }
  }
}
