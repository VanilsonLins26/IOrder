import { ChangeDetectionStrategy, Component, output, inject } from '@angular/core';
import { AddressStore } from '../../../core/stores/address.store';

@Component({
  selector: 'app-address-bar',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './address-bar.component.html',
  styleUrl: './address-bar.component.scss',
})
export class AddressBarComponent {
  readonly addressStore = inject(AddressStore);
  readonly openDrawer = output<void>();

  get summary(): string {
    return this.addressStore.addressSummary() ?? 'Informe seu endereço';
  }

  get isUsingGeo(): boolean {
    return this.addressStore.isUsingGeolocation();
  }
}
