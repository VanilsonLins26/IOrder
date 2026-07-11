import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import type { StoreResponse } from '../../../core/models/store.model';

@Component({
  selector: 'app-store-card',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './store-card.html',
  styleUrl: './store-card.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class StoreCardComponent {
  store = input.required<StoreResponse>();
}
