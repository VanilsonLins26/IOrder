import { Component, ChangeDetectionStrategy } from '@angular/core';

@Component({
  selector: 'app-home-page',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="home-page">
      <section class="hero">
        <div class="hero__container">
          <h1 class="hero__title">Encomendas sob medida,<br />do seu jeito</h1>
          <p class="hero__subtitle">Descubra lojas incríveis e personalize seus produtos favoritos</p>
        </div>
      </section>
    </div>
  `,
  styles: [`
    .hero {
      padding: var(--space-16) var(--space-4);
      text-align: center;
      background: linear-gradient(135deg, var(--color-primary-50) 0%, var(--surface-primary) 100%);

      :host-context([data-theme='dark']) & {
        background: linear-gradient(135deg, rgba(249, 115, 22, 0.08) 0%, var(--surface-primary) 100%);
      }
    }

    .hero__container {
      max-width: var(--container-lg);
      margin: 0 auto;
    }

    .hero__title {
      font-size: var(--font-size-4xl);
      font-weight: var(--font-weight-extrabold);
      line-height: var(--line-height-tight);
      color: var(--text-primary);
      margin-bottom: var(--space-4);

      @media (min-width: 768px) { font-size: var(--font-size-5xl); }
    }

    .hero__subtitle {
      font-size: var(--font-size-lg);
      color: var(--text-secondary);
      max-width: 520px;
      margin: 0 auto;
    }
  `],
})
export class HomePageComponent {}
