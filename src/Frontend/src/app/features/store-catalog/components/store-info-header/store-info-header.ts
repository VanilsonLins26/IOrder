import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { CurrencyPipe, DecimalPipe } from '@angular/common';
import type { StoreResponse } from '../../../../core/models/store.model';

@Component({
  selector: 'app-store-info-header',
  standalone: true,
  imports: [CurrencyPipe, DecimalPipe],
  templateUrl: './store-info-header.html',
  styleUrl: './store-info-header.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class StoreInfoHeaderComponent {
  store = input.required<StoreResponse>();
}
