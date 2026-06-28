import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import type { StoreResponse } from '../../../../core/models/store.model';

@Component({
  selector: 'app-store-info-header',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './store-info-header.html',
  styleUrl: './store-info-header.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class StoreInfoHeaderComponent {
  store = input.required<StoreResponse>();
}
