import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { SlicePipe } from '@angular/common';
import type { AvailableCourierResponseDto } from '../../../../core/models';

@Component({
  selector: 'app-search-couriers-dialog',
  standalone: true,
  imports: [SlicePipe],
  templateUrl: './search-couriers-dialog.component.html',
  styleUrl: './search-couriers-dialog.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SearchCouriersDialogComponent {
  readonly couriers = input.required<AvailableCourierResponseDto[]>();
  readonly searching = input.required<boolean>();
  readonly assign = output<string>();
  readonly closeDialog = output<void>();
}
