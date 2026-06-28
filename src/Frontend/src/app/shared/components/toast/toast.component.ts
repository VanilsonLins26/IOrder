import { Component, inject, ChangeDetectionStrategy } from '@angular/core';
import { ToastService, type Toast } from '../../../core/services/toast.service';

@Component({
  selector: 'app-toast',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="toast-container" aria-live="polite" aria-atomic="true">
      @for (toast of toastService.toasts(); track toast.id) {
        <div
          class="toast toast--{{ toast.type }}"
          role="alert"
          (click)="toastService.dismiss(toast.id)"
          (keydown.enter)="toastService.dismiss(toast.id)"
          tabindex="0"
        >
          <div class="toast__icon">
            @switch (toast.type) {
              @case ('success') { <span>✓</span> }
              @case ('error') { <span>✕</span> }
              @case ('warning') { <span>⚠</span> }
              @case ('info') { <span>ℹ</span> }
            }
          </div>
          <p class="toast__message">{{ toast.message }}</p>
          <button
            class="toast__close"
            (click)="$event.stopPropagation(); toastService.dismiss(toast.id)"
            aria-label="Fechar notificação"
          >
            ✕
          </button>
        </div>
      }
    </div>
  `,
  styles: [`
    .toast-container {
      position: fixed;
      top: var(--space-4);
      right: var(--space-4);
      z-index: var(--z-toast);
      display: flex;
      flex-direction: column;
      gap: var(--space-3);
      max-width: 420px;
      width: calc(100% - var(--space-8));
    }

    .toast {
      display: flex;
      align-items: flex-start;
      gap: var(--space-3);
      padding: var(--space-3) var(--space-4);
      border-radius: var(--radius-lg);
      box-shadow: var(--shadow-lg);
      cursor: pointer;
      animation: slideInRight 300ms ease forwards;
      transition: opacity var(--transition-fast), transform var(--transition-fast);
      border-left: 4px solid;

      &:hover {
        transform: translateX(-4px);
      }

      &--success {
        background-color: var(--color-success-light);
        border-color: var(--color-success);
        color: var(--color-success-dark);
      }

      &--error {
        background-color: var(--color-error-light);
        border-color: var(--color-error);
        color: var(--color-error-dark);
      }

      &--warning {
        background-color: var(--color-warning-light);
        border-color: var(--color-warning);
        color: var(--color-warning-dark);
      }

      &--info {
        background-color: var(--color-info-light);
        border-color: var(--color-info);
        color: var(--color-info-dark);
      }
    }

    :host-context([data-theme='dark']) .toast {
      &--success {
        background-color: rgba(34, 197, 94, 0.15);
        color: #86efac;
      }
      &--error {
        background-color: rgba(239, 68, 68, 0.15);
        color: #fca5a5;
      }
      &--warning {
        background-color: rgba(234, 179, 8, 0.15);
        color: #fde047;
      }
      &--info {
        background-color: rgba(59, 130, 246, 0.15);
        color: #93c5fd;
      }
    }

    .toast__icon {
      flex-shrink: 0;
      font-size: var(--font-size-lg);
      line-height: 1;
      margin-top: 1px;
    }

    .toast__message {
      flex: 1;
      font-size: var(--font-size-sm);
      font-weight: var(--font-weight-medium);
      line-height: var(--line-height-normal);
      color: inherit;
    }

    .toast__close {
      flex-shrink: 0;
      display: flex;
      align-items: center;
      justify-content: center;
      width: 20px;
      height: 20px;
      font-size: var(--font-size-xs);
      opacity: 0.6;
      transition: opacity var(--transition-fast);
      border-radius: var(--radius-sm);

      &:hover {
        opacity: 1;
      }
    }

    @keyframes slideInRight {
      from {
        opacity: 0;
        transform: translateX(100%);
      }
      to {
        opacity: 1;
        transform: translateX(0);
      }
    }
  `],
})
export class ToastComponent {
  protected readonly toastService = inject(ToastService);
}
