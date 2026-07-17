import { Component, ChangeDetectionStrategy, input } from '@angular/core';

@Component({
  selector: 'app-loading-skeleton',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div
      class="skeleton"
      [style.width]="width()"
      [style.height]="height()"
      [style.border-radius]="borderRadius()"
    ></div>
  `,
  styles: [`
    :host { display: block; }

    .skeleton {
      background: linear-gradient(
        90deg,
        var(--color-neutral-200) 25%,
        var(--color-neutral-100) 50%,
        var(--color-neutral-200) 75%
      );
      background-size: 200% 100%;
      animation: shimmer 1.5s ease-in-out infinite;
    }

    @keyframes shimmer {
      0%   { background-position: -200% 0; }
      100% { background-position:  200% 0; }
    }
  `],
})
export class LoadingSkeletonComponent {
  width        = input<string>('100%');
  height       = input<string>('20px');
  borderRadius = input<string>('var(--radius-md)');
}
