import { Component, ChangeDetectionStrategy, signal, inject } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AsyncPipe } from '@angular/common';
import { AuthService } from '@auth0/auth0-angular';

@Component({
  selector: 'app-courier-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, AsyncPipe],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="courier-layout" [class.courier-layout--collapsed]="sidebarCollapsed()">

      <!-- Sidebar -->
      <aside class="courier-sidebar" role="navigation" aria-label="Menu entregador">
        <div class="courier-sidebar__header">
          <a routerLink="/" class="courier-sidebar__logo">
            <span>🛵</span>
            @if (!sidebarCollapsed()) {
              <span class="courier-sidebar__logo-text">IOrder</span>
            }
          </a>
          <button class="courier-sidebar__toggle" (click)="toggleSidebar()" aria-label="Toggle sidebar">
            {{ sidebarCollapsed() ? '→' : '←' }}
          </button>
        </div>

        <nav class="courier-sidebar__nav">
          <a routerLink="/courier/dashboard" routerLinkActive="courier-sidebar__link--active" class="courier-sidebar__link">
            <span class="courier-sidebar__icon">📊</span>
            @if (!sidebarCollapsed()) { <span>Dashboard</span> }
          </a>
          <a routerLink="/courier/deliveries" routerLinkActive="courier-sidebar__link--active" class="courier-sidebar__link">
            <span class="courier-sidebar__icon">📦</span>
            @if (!sidebarCollapsed()) { <span>Minhas Entregas</span> }
          </a>
        </nav>

        <div class="courier-sidebar__footer">
          <a routerLink="/" class="courier-sidebar__link">
            <span class="courier-sidebar__icon">🏠</span>
            @if (!sidebarCollapsed()) { <span>Voltar ao Site</span> }
          </a>
        </div>
      </aside>

      <!-- Main -->
      <div class="courier-main">
        <header class="courier-topbar">
          <h2 class="courier-topbar__title">Painel do Entregador</h2>
          <div class="courier-topbar__actions">
            @if (auth.user$ | async; as user) {
              <span class="courier-topbar__user">{{ user.name }}</span>
              @if (user.picture) {
                <img [src]="user.picture" [alt]="user.name || 'Avatar'"
                     class="courier-topbar__avatar" width="32" height="32" />
              }
            }
          </div>
        </header>
        <div class="courier-content">
          <router-outlet />
        </div>
      </div>
    </div>
  `,
  styles: [`
    .courier-layout {
      display: flex;
      min-height: 100vh;
      min-height: 100dvh;
    }

    .courier-sidebar {
      width: var(--sidebar-width);
      background: var(--surface-secondary);
      border-right: 1px solid var(--border-color);
      display: flex;
      flex-direction: column;
      transition: width var(--transition-base);
      position: fixed;
      top: 0; left: 0; bottom: 0;
      z-index: var(--z-fixed);
    }

    .courier-layout--collapsed .courier-sidebar { width: 72px; }

    .courier-sidebar__header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: var(--space-4);
      height: var(--navbar-height);
      border-bottom: 1px solid var(--border-color);
    }

    .courier-sidebar__logo {
      display: flex;
      align-items: center;
      gap: var(--space-2);
      text-decoration: none;
      font-size: 1.25rem;
    }

    .courier-sidebar__logo-text {
      font-weight: var(--font-weight-extrabold);
      background: linear-gradient(135deg, var(--color-primary-500), var(--color-primary-700));
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
      background-clip: text;
    }

    .courier-sidebar__toggle {
      width: 28px; height: 28px;
      display: flex; align-items: center; justify-content: center;
      border-radius: var(--radius-md);
      font-size: var(--font-size-sm);
      color: var(--text-muted);
      transition: background var(--transition-fast);
      &:hover {
        background: var(--color-neutral-200);
      }
    }

    .courier-sidebar__nav {
      flex: 1;
      padding: var(--space-3);
      display: flex;
      flex-direction: column;
      gap: var(--space-1);
    }

    .courier-sidebar__link {
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
      }

      &--active {
        background: var(--color-primary-50);
        color: var(--color-primary-600);
      }
    }

    .courier-sidebar__icon {
      font-size: 1.1rem;
      flex-shrink: 0;
      width: 24px;
      text-align: center;
    }

    .courier-sidebar__footer {
      padding: var(--space-3);
      border-top: 1px solid var(--border-color);
      display: flex;
      flex-direction: column;
      gap: var(--space-1);
    }

    .courier-main {
      flex: 1;
      margin-left: var(--sidebar-width);
      transition: margin-left var(--transition-base);
      display: flex;
      flex-direction: column;
    }

    .courier-layout--collapsed .courier-main { margin-left: 72px; }

    .courier-topbar {
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

    .courier-topbar__title {
      font-size: var(--font-size-lg);
      font-weight: var(--font-weight-semibold);
      color: var(--text-primary);
    }

    .courier-topbar__actions {
      display: flex;
      align-items: center;
      gap: var(--space-3);
    }

    .courier-topbar__user {
      font-size: var(--font-size-sm);
      font-weight: var(--font-weight-medium);
      color: var(--text-secondary);
    }

    .courier-topbar__avatar {
      width: 32px; height: 32px;
      border-radius: var(--radius-full);
      object-fit: cover;
      border: 2px solid var(--border-color);
    }

    .courier-content {
      flex: 1;
      padding: var(--space-6);
    }

    @media (max-width: 767px) {
      .courier-sidebar { width: 72px; }
      .courier-main { margin-left: 72px; }
    }
  `],
})
export class CourierLayoutComponent {
  protected readonly auth = inject(AuthService);
  protected readonly sidebarCollapsed = signal(false);

  toggleSidebar(): void { this.sidebarCollapsed.update((v) => !v); }
}
