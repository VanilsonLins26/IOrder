import { Component, ChangeDetectionStrategy, inject, signal, OnInit } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AsyncPipe } from '@angular/common';
import { AuthService } from '@auth0/auth0-angular';
import { ThemeService } from '../../../core/services/theme.service';
import { CartStore } from '../../../features/cart/store/cart.store';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, AsyncPipe],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <nav class="navbar" role="navigation" aria-label="Navegação principal">
      <div class="navbar__container">

        <a routerLink="/" class="navbar__logo" aria-label="IOrder - Página inicial">
          <span class="navbar__logo-icon">🛒</span>
          <span class="navbar__logo-text">IOrder</span>
        </a>

        <div class="navbar__links">
          <a routerLink="/stores" routerLinkActive="navbar__link--active" class="navbar__link">
            Lojas
          </a>
        </div>

        <div class="navbar__actions">
          <a
            routerLink="/cart"
            class="navbar__cart-btn"
            aria-label="Abrir carrinho"
          >
            🛒
            @if (cartStore.itemCount() > 0) {
              <span class="navbar__cart-badge">{{ cartStore.itemCount() }}</span>
            }
          </a>

          <button
            class="navbar__icon-btn"
            (click)="themeService.toggle()"
            [attr.aria-label]="themeService.isDark() ? 'Ativar modo claro' : 'Ativar modo escuro'"
          >
            {{ themeService.isDark() ? '☀️' : '🌙' }}
          </button>

          @if (auth.isAuthenticated$ | async) {
            @if (auth.user$ | async; as user) {
              <div class="navbar__user" (click)="toggleDropdown()" (keydown.enter)="toggleDropdown()" tabindex="0" role="button" aria-haspopup="true" [attr.aria-expanded]="dropdownOpen()">
                @if (user.picture) {
                  <img [src]="user.picture" [alt]="user.name || 'Avatar'"
                       class="navbar__avatar" width="32" height="32" />
                }
                <span class="navbar__user-name">{{ user.name }}</span>
                <span class="navbar__user-chevron">▼</span>
                
                @if (dropdownOpen()) {
                  <div class="navbar__dropdown">
                    <div class="navbar__dropdown-header">
                      <span class="navbar__dropdown-name">{{ user.name }}</span>
                      <span class="navbar__dropdown-email">{{ user.email }}</span>
                    </div>
                    
                    <div class="navbar__dropdown-body">
                      @if (hasRole(user, 'ShopKeeper')) {
                        <a routerLink="/admin" class="navbar__dropdown-item" (click)="closeDropdown()">
                          <span class="navbar__dropdown-icon">🏪</span>
                          Painel Lojista
                        </a>
                      } @else {
                        <a routerLink="/become-partner" class="navbar__dropdown-item" (click)="closeDropdown()">
                          <span class="navbar__dropdown-icon">💼</span>
                          Venda no IOrder
                        </a>
                      }
                    </div>
                    
                    <div class="navbar__dropdown-footer">
                      <button class="navbar__dropdown-item navbar__dropdown-item--danger" (click)="logout()">
                        <span class="navbar__dropdown-icon">🚪</span>
                        Sair da conta
                      </button>
                    </div>
                  </div>
                }
              </div>
            }
          } @else {
            <button class="navbar__btn navbar__btn--primary" (click)="login()">Entrar</button>
          }

          <button
            class="navbar__hamburger"
            (click)="toggleMobileMenu()"
            [attr.aria-expanded]="mobileMenuOpen()"
            aria-label="Menu de navegação"
          >
            <span class="navbar__hamburger-line"></span>
            <span class="navbar__hamburger-line"></span>
            <span class="navbar__hamburger-line"></span>
          </button>
        </div>
      </div>

      @if (mobileMenuOpen()) {
        <div class="navbar__mobile-menu" role="menu">
          <a routerLink="/stores" routerLinkActive="navbar__link--active"
             class="navbar__mobile-link" (click)="closeMobileMenu()">
            Lojas
          </a>
          <a routerLink="/cart" class="navbar__mobile-link" (click)="closeMobileMenu()">
            Carrinho
            @if (cartStore.itemCount() > 0) {
              ({{ cartStore.itemCount() }})
            }
          </a>
        </div>
      }
    </nav>
  `,
  styles: [`
    .navbar {
      position: sticky;
      top: 0;
      z-index: var(--z-sticky);
      background: var(--surface-primary);
      border-bottom: 1px solid var(--border-color);
      backdrop-filter: blur(12px);
      -webkit-backdrop-filter: blur(12px);

      :host-context([data-theme='dark']) & {
        background: rgba(10, 10, 10, 0.9);
      }
    }

    .navbar__container {
      display: flex;
      align-items: center;
      justify-content: space-between;
      height: var(--navbar-height);
      max-width: var(--container-xl);
      margin: 0 auto;
      padding: 0 var(--space-4);

      @media (min-width: 768px) { padding: 0 var(--space-6); }
    }

    .navbar__logo {
      display: flex;
      align-items: center;
      gap: var(--space-2);
      text-decoration: none;
      color: var(--text-primary);
      transition: opacity var(--transition-fast);
      &:hover { opacity: 0.8; }
    }

    .navbar__logo-icon { font-size: 1.5rem; }

    .navbar__logo-text {
      font-size: var(--font-size-xl);
      font-weight: var(--font-weight-extrabold);
      background: linear-gradient(135deg, var(--color-primary-500), var(--color-primary-700));
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
      background-clip: text;
    }

    .navbar__links {
      display: none;
      align-items: center;
      gap: var(--space-1);
      @media (min-width: 768px) { display: flex; }
    }

    .navbar__link {
      padding: var(--space-2) var(--space-3);
      font-size: var(--font-size-sm);
      font-weight: var(--font-weight-medium);
      color: var(--text-secondary);
      text-decoration: none;
      border-radius: var(--radius-lg);
      transition: color var(--transition-fast), background var(--transition-fast);

      &:hover {
        color: var(--text-primary);
        background: var(--color-neutral-100);
        :host-context([data-theme='dark']) & { background: var(--color-neutral-800); }
      }

      &--active {
        color: var(--color-primary-600);
        background: var(--color-primary-50);
        :host-context([data-theme='dark']) & {
          color: var(--color-primary-400);
          background: rgba(249, 115, 22, 0.1);
        }
      }
    }

    .navbar__actions {
      display: flex;
      align-items: center;
      gap: var(--space-2);
    }

    .navbar__icon-btn,
    .navbar__cart-btn {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 36px;
      height: 36px;
      border-radius: var(--radius-lg);
      font-size: 1.1rem;
      transition: background var(--transition-fast);

      &:hover {
        background: var(--color-neutral-100);
        :host-context([data-theme='dark']) & { background: var(--color-neutral-800); }
      }
    }

    .navbar__cart-btn {
      position: relative;
      text-decoration: none;
      color: var(--text-primary);
    }

    .navbar__cart-badge {
      position: absolute;
      top: -4px;
      right: -4px;
      min-width: 18px;
      height: 18px;
      padding: 0 4px;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 10px;
      font-weight: var(--font-weight-bold);
      color: white;
      background: var(--color-error);
      border-radius: var(--radius-full);
      line-height: 1;
    }

    .navbar__user {
      display: none;
      align-items: center;
      gap: var(--space-2);
      padding: var(--space-1) var(--space-3) var(--space-1) var(--space-1);
      border-radius: var(--radius-full);
      cursor: pointer;
      position: relative;
      transition: background var(--transition-fast);
      border: 1px solid transparent;

      &:hover, &:focus-visible {
        background: var(--surface-secondary);
        border-color: var(--border-color);
      }
      
      @media (min-width: 768px) { display: flex; }
    }

    .navbar__avatar {
      width: 32px;
      height: 32px;
      border-radius: var(--radius-full);
      object-fit: cover;
      border: 2px solid var(--border-color);
    }

    .navbar__user-name {
      font-size: var(--font-size-sm);
      font-weight: var(--font-weight-medium);
      color: var(--text-primary);
      max-width: 120px;
      overflow: hidden;
      text-overflow: ellipsis;
      white-space: nowrap;
    }
    
    .navbar__user-chevron {
      font-size: 0.7rem;
      color: var(--text-muted);
    }
    
    .navbar__dropdown {
      position: absolute;
      top: calc(100% + 10px);
      right: 0;
      width: 240px;
      background: var(--surface-primary);
      border: 1px solid var(--border-color);
      border-radius: var(--radius-xl);
      box-shadow: 0 10px 25px rgba(0,0,0,0.1);
      overflow: hidden;
      animation: dropdownFadeIn 0.2s cubic-bezier(0.16, 1, 0.3, 1);
      transform-origin: top right;
      z-index: 100;
      
      :host-context([data-theme='dark']) & {
        box-shadow: 0 10px 30px rgba(0,0,0,0.5);
      }
    }
    
    .navbar__dropdown-header {
      padding: var(--space-4);
      border-bottom: 1px solid var(--border-color);
      display: flex;
      flex-direction: column;
    }
    
    .navbar__dropdown-name {
      font-weight: var(--font-weight-bold);
      color: var(--text-primary);
      font-size: var(--font-size-sm);
    }
    
    .navbar__dropdown-email {
      color: var(--text-muted);
      font-size: var(--font-size-xs);
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }
    
    .navbar__dropdown-body {
      padding: var(--space-2);
    }
    
    .navbar__dropdown-footer {
      padding: var(--space-2);
      border-top: 1px solid var(--border-color);
      background: var(--surface-secondary);
    }
    
    .navbar__dropdown-item {
      display: flex;
      align-items: center;
      gap: var(--space-3);
      padding: var(--space-2) var(--space-3);
      width: 100%;
      text-align: left;
      font-size: var(--font-size-sm);
      font-weight: var(--font-weight-medium);
      color: var(--text-secondary);
      text-decoration: none;
      border: none;
      background: transparent;
      border-radius: var(--radius-lg);
      cursor: pointer;
      transition: all var(--transition-fast);
      
      &:hover {
        background: var(--color-primary-50);
        color: var(--color-primary-600);
        
        :host-context([data-theme='dark']) & {
          background: rgba(249, 115, 22, 0.1);
          color: var(--color-primary-400);
        }
      }
      
      &--danger {
        &:hover {
          background: rgba(239, 68, 68, 0.1);
          color: #ef4444;
        }
      }
    }
    
    .navbar__dropdown-icon {
      font-size: 1.1rem;
    }
    
    @keyframes dropdownFadeIn {
      from { opacity: 0; transform: scale(0.95); }
      to { opacity: 1; transform: scale(1); }
    }

    .navbar__btn {
      padding: var(--space-2) var(--space-4);
      font-size: var(--font-size-sm);
      font-weight: var(--font-weight-semibold);
      border-radius: var(--radius-lg);
      transition: all var(--transition-fast);
      white-space: nowrap;

      &--primary {
        background: var(--color-primary-500);
        color: white;
        &:hover { background: var(--color-primary-600); transform: translateY(-1px); }
      }

      &--outline {
        background: transparent;
        color: var(--text-secondary);
        border: 1px solid var(--border-color);
        &:hover {
          border-color: var(--border-color-hover);
          color: var(--text-primary);
          background: var(--color-neutral-50);
          :host-context([data-theme='dark']) & { background: var(--color-neutral-800); }
        }
      }
    }

    .navbar__hamburger {
      display: flex;
      flex-direction: column;
      justify-content: center;
      gap: 4px;
      width: 36px;
      height: 36px;
      padding: 8px;
      border-radius: var(--radius-lg);
      transition: background var(--transition-fast);

      &:hover {
        background: var(--color-neutral-100);
        :host-context([data-theme='dark']) & { background: var(--color-neutral-800); }
      }

      @media (min-width: 768px) { display: none; }
    }

    .navbar__hamburger-line {
      display: block;
      width: 100%;
      height: 2px;
      background: var(--text-primary);
      border-radius: 1px;
    }

    .navbar__mobile-menu {
      display: flex;
      flex-direction: column;
      padding: var(--space-2) var(--space-4) var(--space-4);
      border-top: 1px solid var(--border-color);
      animation: fadeInDown 200ms ease forwards;
      @media (min-width: 768px) { display: none; }
    }

    .navbar__mobile-link {
      padding: var(--space-3);
      font-size: var(--font-size-base);
      font-weight: var(--font-weight-medium);
      color: var(--text-secondary);
      text-decoration: none;
      border-radius: var(--radius-lg);
      transition: background var(--transition-fast), color var(--transition-fast);

      &:hover {
        background: var(--color-neutral-100);
        color: var(--text-primary);
        :host-context([data-theme='dark']) & { background: var(--color-neutral-800); }
      }
    }

    @keyframes fadeInDown {
      from { opacity: 0; transform: translateY(-8px); }
      to   { opacity: 1; transform: translateY(0); }
    }
  `],
})
export class NavbarComponent {
  protected readonly auth         = inject(AuthService);
  protected readonly themeService = inject(ThemeService);
  protected readonly cartStore    = inject(CartStore);
  protected readonly mobileMenuOpen = signal(false);
  protected readonly dropdownOpen = signal(false);

  login():  void { this.auth.loginWithRedirect(); }
  logout(): void { this.auth.logout({ logoutParams: { returnTo: window.location.origin } }); }
  toggleMobileMenu(): void { this.mobileMenuOpen.update((v) => !v); }
  closeMobileMenu():  void { this.mobileMenuOpen.set(false); }
  
  toggleDropdown(): void { this.dropdownOpen.update(v => !v); }
  closeDropdown(): void { this.dropdownOpen.set(false); }

  hasRole(user: any, role: string): boolean {
    const roles: string[] = user?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ?? [];
    return roles.includes(role);
  }
}
