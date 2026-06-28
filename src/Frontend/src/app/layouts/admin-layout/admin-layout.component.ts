import { Component, ChangeDetectionStrategy, signal, inject } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AsyncPipe } from '@angular/common';
import { AuthService } from '@auth0/auth0-angular';
import { ThemeService } from '../../core/services/theme.service';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, AsyncPipe],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="admin-layout" [class.admin-layout--collapsed]="sidebarCollapsed()">

      <!-- Sidebar -->
      <aside class="admin-sidebar" role="navigation" aria-label="Menu administrativo">
        <div class="admin-sidebar__header">
          <a routerLink="/" class="admin-sidebar__logo">
            <span>🛒</span>
            @if (!sidebarCollapsed()) {
              <span class="admin-sidebar__logo-text">IOrder</span>
            }
          </a>
          <button class="admin-sidebar__toggle" (click)="toggleSidebar()" aria-label="Toggle sidebar">
            {{ sidebarCollapsed() ? '→' : '←' }}
          </button>
        </div>

        <nav class="admin-sidebar__nav">
          <a routerLink="/admin/dashboard"  routerLinkActive="admin-sidebar__link--active" class="admin-sidebar__link">
            <span class="admin-sidebar__icon">📊</span>
            @if (!sidebarCollapsed()) { <span>Dashboard</span> }
          </a>
          <a routerLink="/admin/products"   routerLinkActive="admin-sidebar__link--active" class="admin-sidebar__link">
            <span class="admin-sidebar__icon">📦</span>
            @if (!sidebarCollapsed()) { <span>Produtos</span> }
          </a>
          <a routerLink="/admin/categories" routerLinkActive="admin-sidebar__link--active" class="admin-sidebar__link">
            <span class="admin-sidebar__icon">🏷️</span>
            @if (!sidebarCollapsed()) { <span>Categorias</span> }
          </a>
          <a routerLink="/admin/store"      routerLinkActive="admin-sidebar__link--active" class="admin-sidebar__link">
            <span class="admin-sidebar__icon">🏪</span>
            @if (!sidebarCollapsed()) { <span>Minha Loja</span> }
          </a>
        </nav>

        <div class="admin-sidebar__footer">
          <button class="admin-sidebar__link" (click)="themeService.toggle()">
            <span class="admin-sidebar__icon">{{ themeService.isDark() ? '☀️' : '🌙' }}</span>
            @if (!sidebarCollapsed()) {
              <span>{{ themeService.isDark() ? 'Modo Claro' : 'Modo Escuro' }}</span>
            }
          </button>
          <a routerLink="/" class="admin-sidebar__link">
            <span class="admin-sidebar__icon">🏠</span>
            @if (!sidebarCollapsed()) { <span>Voltar ao Site</span> }
          </a>
        </div>
      </aside>

      <!-- Main -->
      <div class="admin-main">
        <header class="admin-topbar">
          <h2 class="admin-topbar__title">Painel do Lojista</h2>
          <div class="admin-topbar__actions">
            @if (auth.user$ | async; as user) {
              <span class="admin-topbar__user">{{ user.name }}</span>
              @if (user.picture) {
                <img [src]="user.picture" [alt]="user.name || 'Avatar'"
                     class="admin-topbar__avatar" width="32" height="32" />
              }
            }
          </div>
        </header>
        <div class="admin-content">
          <router-outlet />
        </div>
      </div>
    </div>
  `,
  styles: [`
    .admin-layout {
      display: flex;
      min-height: 100vh;
      min-height: 100dvh;
    }

    .admin-sidebar {
      width: var(--sidebar-width);
      background: var(--surface-secondary);
      border-right: 1px solid var(--border-color);
      display: flex;
      flex-direction: column;
      transition: width var(--transition-base);
      position: fixed;
      top: 0; left: 0; bottom: 0;
      z-index: var(--z-fixed);

      :host-context([data-theme='dark']) & { background: var(--color-neutral-900); }
    }

    .admin-layout--collapsed .admin-sidebar { width: 72px; }

    .admin-sidebar__header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: var(--space-4);
      height: var(--navbar-height);
      border-bottom: 1px solid var(--border-color);
    }

    .admin-sidebar__logo {
      display: flex;
      align-items: center;
      gap: var(--space-2);
      text-decoration: none;
      font-size: 1.25rem;
    }

    .admin-sidebar__logo-text {
      font-weight: var(--font-weight-extrabold);
      background: linear-gradient(135deg, var(--color-primary-500), var(--color-primary-700));
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
      background-clip: text;
    }

    .admin-sidebar__toggle {
      width: 28px; height: 28px;
      display: flex; align-items: center; justify-content: center;
      border-radius: var(--radius-md);
      font-size: var(--font-size-sm);
      color: var(--text-muted);
      transition: background var(--transition-fast);
      &:hover {
        background: var(--color-neutral-200);
        :host-context([data-theme='dark']) & { background: var(--color-neutral-800); }
      }
    }

    .admin-sidebar__nav {
      flex: 1;
      padding: var(--space-3);
      display: flex;
      flex-direction: column;
      gap: var(--space-1);
    }

    .admin-sidebar__link {
      display: flex;
      align-items: center;
      gap: var(--space-3);
      padding: var(--space-2) var(--space-3);
      font-size: var(--font-size-sm);
      font-weight: var(--font-weight-medium);
      color: var(--text-secondary);
      text-decoration: none;
      border-radius: var(--radius-lg);
      transition: all var(--transition-fast);
      white-space: nowrap;

      &:hover {
        background: var(--color-neutral-200);
        color: var(--text-primary);
        :host-context([data-theme='dark']) & { background: var(--color-neutral-800); }
      }

      &--active {
        background: var(--color-primary-50);
        color: var(--color-primary-600);
        :host-context([data-theme='dark']) & {
          background: rgba(249, 115, 22, 0.1);
          color: var(--color-primary-400);
        }
      }
    }

    .admin-sidebar__icon {
      font-size: 1.1rem;
      flex-shrink: 0;
      width: 24px;
      text-align: center;
    }

    .admin-sidebar__footer {
      padding: var(--space-3);
      border-top: 1px solid var(--border-color);
      display: flex;
      flex-direction: column;
      gap: var(--space-1);
    }

    .admin-main {
      flex: 1;
      margin-left: var(--sidebar-width);
      transition: margin-left var(--transition-base);
      display: flex;
      flex-direction: column;
    }

    .admin-layout--collapsed .admin-main { margin-left: 72px; }

    .admin-topbar {
      display: flex;
      align-items: center;
      justify-content: space-between;
      height: var(--navbar-height);
      padding: 0 var(--space-6);
      background: var(--surface-primary);
      border-bottom: 1px solid var(--border-color);
      position: sticky;
      top: 0;
      z-index: var(--z-sticky);
    }

    .admin-topbar__title {
      font-size: var(--font-size-lg);
      font-weight: var(--font-weight-semibold);
      color: var(--text-primary);
    }

    .admin-topbar__actions {
      display: flex;
      align-items: center;
      gap: var(--space-3);
    }

    .admin-topbar__user {
      font-size: var(--font-size-sm);
      font-weight: var(--font-weight-medium);
      color: var(--text-secondary);
    }

    .admin-topbar__avatar {
      width: 32px; height: 32px;
      border-radius: var(--radius-full);
      object-fit: cover;
      border: 2px solid var(--border-color);
    }

    .admin-content {
      flex: 1;
      padding: var(--space-6);
    }

    @media (max-width: 767px) {
      .admin-sidebar { width: 72px; }
      .admin-main { margin-left: 72px; }
    }
  `],
})
export class AdminLayoutComponent {
  protected readonly auth             = inject(AuthService);
  protected readonly themeService     = inject(ThemeService);
  protected readonly sidebarCollapsed = signal(false);

  toggleSidebar(): void { this.sidebarCollapsed.update((v) => !v); }
}
