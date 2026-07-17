import { Component, ChangeDetectionStrategy, input, output, effect, ElementRef, inject } from '@angular/core';
import { FocusTrapFactory, FocusTrap } from '@angular/cdk/a11y';

@Component({
  selector: 'app-modal',
  standalone: true,
  imports: [],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './modal.component.html',
  styleUrl: './modal.component.scss',
})
export class ModalComponent {
  title = input.required<string>();
  isOpen = input.required<boolean>();

  close = output<void>();

  private el = inject(ElementRef<HTMLElement>);
  private focusTrapFactory = inject(FocusTrapFactory);
  private focusTrap: FocusTrap | null = null;

  constructor() {
    effect(() => {
      if (this.isOpen()) {
        setTimeout(() => {
          this.focusTrap = this.focusTrapFactory.create(this.el.nativeElement.querySelector('.modal-content')!);
          this.focusTrap.focusFirstTabbableElement();
        }, 50);
      } else {
        this.focusTrap?.destroy();
        this.focusTrap = null;
      }
    });
  }

  onBackdropClick(event: MouseEvent) {
    if ((event.target as HTMLElement).classList.contains('modal-backdrop')) {
      this.close.emit();
    }
  }
}
