import { Component, ChangeDetectionStrategy, input, output } from '@angular/core';
import { ModalComponent } from '../modal/modal.component';

@Component({
  selector: 'app-confirmation-modal',
  standalone: true,
  imports: [ModalComponent],
  templateUrl: './confirmation-modal.component.html',
  styleUrl: './confirmation-modal.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ConfirmationModalComponent {
  title = input.required<string>();
  message = input.required<string>();
  confirmLabel = input('Sim');
  cancelLabel = input('Não');
  isOpen = input.required<boolean>();
  loading = input(false);

  confirm = output<void>();
  cancel = output<void>();
  close = output<void>();
}
