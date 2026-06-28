import { Component, ChangeDetectionStrategy, input, output } from '@angular/core';

@Component({
  selector: 'app-empty-state',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="empty-state">
      <div class="empty-state__icon">
        <span>{{ icon() }}</span>
      </div>
      <h3 class="empty-state__title">{{ title() }}</h3>
      @if (description()) {
        <p class="empty-state__description">{{ description() }}</p>
      }
      @if (actionLabel()) {
        <button class="empty-state__action" (click)="action.emit()">
          {{ actionLabel() }}
        </button>
      }
    </div>
  `,
  styles: [`
    .empty-state {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: var(--space-12) var(--space-4);
      text-align: center;
    }

    .empty-state__icon {
      width: 80px;
      height: 80px;
      display: flex;
      align-items: center;
      justify-content: center;
      background: var(--color-neutral-100);
      border-radius: var(--radius-full);
      margin-bottom: var(--space-6);
      font-size: 2rem;

      :host-context([data-theme='dark']) & {
        background: var(--color-neutral-800);
      }
    }

    .empty-state__title {
      font-size: var(--font-size-xl);
      font-weight: var(--font-weight-semibold);
      color: var(--text-primary);
      margin-bottom: var(--space-2);
    }

    .empty-state__description {
      font-size: var(--font-size-sm);
      color: var(--text-muted);
      max-width: 360px;
      line-height: var(--line-height-relaxed);
    }

    .empty-state__action {
      margin-top: var(--space-6);
      padding: var(--space-3) var(--space-6);
      background: var(--color-primary-500);
      color: white;
      font-weight: var(--font-weight-semibold);
      font-size: var(--font-size-sm);
      border-radius: var(--radius-lg);
      transition: background var(--transition-fast), transform var(--transition-fast);

      &:hover {
        background: var(--color-primary-600);
        transform: translateY(-1px);
      }

      &:active { transform: translateY(0); }
    }
  `],
})
export class EmptyStateComponent {
  icon        = input<string>('📦');
  title       = input.required<string>();
  description = input<string>('');
  actionLabel = input<string>('');
  action      = output<void>();
}
