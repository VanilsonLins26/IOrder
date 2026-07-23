import { Component, ChangeDetectionStrategy, input, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-review-modal',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './review-modal.component.html',
  styleUrl: './review-modal.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ReviewModalComponent {
  readonly isOpen = input.required<boolean>();
  readonly hasCourier = input<boolean>(false);
  readonly loading = input<boolean>(false);

  readonly close = output<void>();
  readonly submitReview = output<{ storeRating: number; courierRating?: number; comment?: string }>();

  storeRating = signal<number>(0);
  courierRating = signal<number>(0);
  comment = signal<string>('');

  setStoreRating(rating: number) {
    this.storeRating.set(rating);
  }

  setCourierRating(rating: number) {
    this.courierRating.set(rating);
  }

  onClose() {
    this.close.emit();
  }

  onSubmit() {
    if (this.storeRating() === 0) return;

    this.submitReview.emit({
      storeRating: this.storeRating(),
      courierRating: (this.hasCourier() && this.courierRating() > 0) ? this.courierRating() : undefined,
      comment: this.comment()
    });
  }
}
