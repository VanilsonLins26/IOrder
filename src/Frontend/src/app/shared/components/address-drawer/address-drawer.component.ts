import { ChangeDetectionStrategy, Component, input, output, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AddressStore } from '../../../core/stores/address.store';
import type { UserAddressResponse } from '../../../core/models';

@Component({
  selector: 'app-address-drawer',
  standalone: true,
  imports: [RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './address-drawer.component.html',
  styleUrl: './address-drawer.component.scss',
})
export class AddressDrawerComponent {
  readonly addressStore = inject(AddressStore);
  readonly visible = input.required<boolean>();
  readonly closeDrawer = output<void>();

  onSelect(address: UserAddressResponse) {
    this.addressStore.selectAddress(address);
    this.closeDrawer.emit();
  }

  onUseCurrentLocation() {
    this.addressStore.useCurrentLocation();
    this.closeDrawer.emit();
  }

  onClose() {
    this.closeDrawer.emit();
  }

  onBackdropClick(event: MouseEvent) {
    if ((event.target as HTMLElement).classList.contains('address-drawer__backdrop')) {
      this.closeDrawer.emit();
    }
  }
}
