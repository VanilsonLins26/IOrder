import { Component, ChangeDetectionStrategy } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <footer class="footer">
      <div class="footer__container">
        <div class="footer__brand">
          <span class="footer__logo">
            <span>🛒</span>
            <span class="footer__logo-text">IOrder</span>
          </span>
          <p class="footer__tagline">Marketplace de encomendas personalizadas</p>
        </div>

        <div class="footer__links">
          <div class="footer__section">
            <h4 class="footer__heading">Navegação</h4>
            <a routerLink="/"       class="footer__link">Início</a>
            <a routerLink="/stores" class="footer__link">Lojas</a>
          </div>
          <div class="footer__section">
            <h4 class="footer__heading">Suporte</h4>
            <a href="#" class="footer__link">Central de ajuda</a>
            <a href="#" class="footer__link">Termos de uso</a>
            <a href="#" class="footer__link">Privacidade</a>
          </div>
        </div>

        <div class="footer__bottom">
          <p class="footer__copyright">
            &copy; {{ currentYear }} IOrder. Todos os direitos reservados.
          </p>
        </div>
      </div>
    </footer>
  `,
  styles: [`
    .footer {
      background: var(--color-neutral-900);
      color: var(--color-neutral-400);
      padding: var(--space-12) 0 var(--space-6);
      margin-top: auto;
    }

    .footer__container {
      max-width: var(--container-xl);
      margin: 0 auto;
      padding: 0 var(--space-4);
      @media (min-width: 768px) { padding: 0 var(--space-6); }
    }

    .footer__brand { margin-bottom: var(--space-8); }

    .footer__logo {
      display: flex;
      align-items: center;
      gap: var(--space-2);
      font-size: var(--font-size-lg);
      margin-bottom: var(--space-2);
    }

    .footer__logo-text { font-weight: var(--font-weight-extrabold); color: white; }
    .footer__tagline   { font-size: var(--font-size-sm); color: var(--color-neutral-500); }

    .footer__links {
      display: grid;
      grid-template-columns: repeat(2, 1fr);
      gap: var(--space-8);
      margin-bottom: var(--space-8);
      @media (min-width: 640px) { grid-template-columns: repeat(3, 1fr); }
    }

    .footer__section { display: flex; flex-direction: column; gap: var(--space-2); }

    .footer__heading {
      font-size: var(--font-size-sm);
      font-weight: var(--font-weight-semibold);
      color: white;
      margin-bottom: var(--space-1);
    }

    .footer__link {
      font-size: var(--font-size-sm);
      color: var(--color-neutral-400);
      text-decoration: none;
      transition: color var(--transition-fast);
      &:hover { color: white; }
    }

    .footer__bottom {
      border-top: 1px solid var(--color-neutral-800);
      padding-top: var(--space-6);
    }

    .footer__copyright { font-size: var(--font-size-xs); color: var(--color-neutral-500); }
  `],
})
export class FooterComponent {
  readonly currentYear = new Date().getFullYear();
}
