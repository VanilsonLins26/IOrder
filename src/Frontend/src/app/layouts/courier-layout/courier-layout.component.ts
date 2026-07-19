import { Component, ChangeDetectionStrategy, signal, inject, computed } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AsyncPipe } from '@angular/common';
import { AuthService } from '@auth0/auth0-angular';

@Component({
  selector: 'app-courier-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, AsyncPipe],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="courier-layout" [class.sidebar-collapsed]="sidebarCollapsed()">

      <!-- ======= SIDEBAR ======= -->
      <aside class="courier-sidebar" role="navigation" aria-label="Menu entregador">

        <!-- Logo -->
        <div class="courier-sidebar__brand">
          <a routerLink="/" class="courier-sidebar__logo" [title]="sidebarCollapsed() ? 'IOrder' : ''">
            <div class="courier-sidebar__logo-icon">
              <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
                <circle cx="5.5" cy="17.5" r="2.5"/><circle cx="17.5" cy="17.5" r="2.5"/>
                <path d="M8 17.5h7M2 12l3-7h12l2 5H2zm14 0v5"/>
                <path d="M10 7v5"/>
              </svg>
            </div>
            @if (!sidebarCollapsed()) {
              <span class="courier-sidebar__logo-text">IOrder</span>
            }
          </a>
          <button class="courier-sidebar__collapse-btn" (click)="toggleSidebar()" [attr.aria-label]="sidebarCollapsed() ? 'Expandir menu' : 'Recolher menu'">
            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round">
              @if (sidebarCollapsed()) {
                <polyline points="9 18 15 12 9 6"></polyline>
              } @else {
                <polyline points="15 18 9 12 15 6"></polyline>
              }
            </svg>
          </button>
        </div>

        <!-- Nav -->
        <nav class="courier-sidebar__nav">
          <a routerLink="/courier/dashboard" routerLinkActive="courier-sidebar__link--active"
             class="courier-sidebar__link" [title]="sidebarCollapsed() ? 'Dashboard' : ''">
            <span class="courier-sidebar__link-icon">
              <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <rect x="3" y="3" width="7" height="7"/><rect x="14" y="3" width="7" height="7"/>
                <rect x="14" y="14" width="7" height="7"/><rect x="3" y="14" width="7" height="7"/>
              </svg>
            </span>
            @if (!sidebarCollapsed()) { <span class="courier-sidebar__link-label">Dashboard</span> }
          </a>

          <a routerLink="/courier/deliveries" routerLinkActive="courier-sidebar__link--active"
             class="courier-sidebar__link" [title]="sidebarCollapsed() ? 'Minhas Entregas' : ''">
            <span class="courier-sidebar__link-icon">
              <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0118 0z"/><circle cx="12" cy="10" r="3"/>
              </svg>
            </span>
            @if (!sidebarCollapsed()) { <span class="courier-sidebar__link-label">Minhas Entregas</span> }
          </a>
        </nav>

        <!-- Footer -->
        <div class="courier-sidebar__footer">
          @if (auth.user$ | async; as user) {
            <div class="courier-sidebar__user" [title]="sidebarCollapsed() ? (user.name || '') : ''">
              @if (user.picture) {
                <img [src]="user.picture" [alt]="user.name || 'Avatar'" class="courier-sidebar__avatar" />
              } @else {
                <div class="courier-sidebar__avatar courier-sidebar__avatar--placeholder">
                  {{ (user.name || 'U')[0].toUpperCase() }}
                </div>
              }
              @if (!sidebarCollapsed()) {
                <div class="courier-sidebar__user-info">
                  <span class="courier-sidebar__user-name">{{ user.name }}</span>
                  <span class="courier-sidebar__user-role">Entregador</span>
                </div>
              }
            </div>
          }
          <a routerLink="/" class="courier-sidebar__link courier-sidebar__link--back" [title]="sidebarCollapsed() ? 'Voltar ao site' : ''">
            <span class="courier-sidebar__link-icon">
              <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M3 9l9-7 9 7v11a2 2 0 01-2 2H5a2 2 0 01-2-2z"/><polyline points="9 22 9 12 15 12 15 22"/>
              </svg>
            </span>
            @if (!sidebarCollapsed()) { <span class="courier-sidebar__link-label">Voltar ao Site</span> }
          </a>
        </div>
      </aside>

      <!-- ======= MAIN AREA ======= -->
      <div class="courier-main">

        <!-- Topbar -->
        <header class="courier-topbar">
          <div class="courier-topbar__left">
            <button class="courier-topbar__mobile-menu" (click)="toggleSidebar()" aria-label="Abrir menu">
              <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round">
                <line x1="3" y1="6" x2="21" y2="6"/><line x1="3" y1="12" x2="21" y2="12"/><line x1="3" y1="18" x2="21" y2="18"/>
              </svg>
            </button>
            <div class="courier-topbar__breadcrumb">
              <span class="courier-topbar__app-name">Painel do Entregador</span>
            </div>
          </div>

          <div class="courier-topbar__right">
            <div class="courier-topbar__status-pill">
              <span class="courier-topbar__status-dot"></span>
              <span>Online</span>
            </div>
            @if (auth.user$ | async; as user) {
              <div class="courier-topbar__user-chip">
                @if (user.picture) {
                  <img [src]="user.picture" [alt]="user.name || ''" class="courier-topbar__user-avatar" />
                }
                <span class="courier-topbar__user-name">{{ (user.name ?? '').split(' ')[0] }}</span>
              </div>
            }
          </div>
        </header>

        <!-- Content -->
        <main class="courier-content">
          <router-outlet />
        </main>
      </div>

      <!-- Mobile Overlay -->
      @if (sidebarCollapsed() === false) {
        <div class="courier-sidebar__overlay" (click)="toggleSidebar()"></div>
      }
    </div>
  `,
  styles: [`
    /* =========================================
       COURIER LAYOUT — PREMIUM DESIGN
    ========================================= */

    .courier-layout {
      display: flex;
      min-height: 100vh;
      min-height: 100dvh;
      background: var(--surface-primary);
    }

    /* ====== SIDEBAR ====== */
    .courier-sidebar {
      width: 260px;
      background: linear-gradient(180deg, #0f172a 0%, #1e293b 100%);
      display: flex;
      flex-direction: column;
      position: fixed;
      top: 0; left: 0; bottom: 0;
      z-index: var(--z-fixed);
      transition: width 250ms cubic-bezier(0.4, 0, 0.2, 1);
      overflow: hidden;
      box-shadow: 4px 0 24px rgba(0,0,0,0.15);
    }

    .sidebar-collapsed .courier-sidebar { width: 72px; }

    /* Brand */
    .courier-sidebar__brand {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 0 16px;
      height: 64px;
      border-bottom: 1px solid rgba(255,255,255,0.06);
      flex-shrink: 0;
    }

    .courier-sidebar__logo {
      display: flex;
      align-items: center;
      gap: 10px;
      text-decoration: none;
      overflow: hidden;
    }

    .courier-sidebar__logo-icon {
      width: 38px;
      height: 38px;
      border-radius: 10px;
      background: linear-gradient(135deg, #f97316, #ea580c);
      display: flex;
      align-items: center;
      justify-content: center;
      color: white;
      flex-shrink: 0;
      box-shadow: 0 4px 12px rgba(249,115,22,0.4);
    }

    .courier-sidebar__logo-text {
      font-size: 1.2rem;
      font-weight: 800;
      color: white;
      white-space: nowrap;
      letter-spacing: -0.02em;
    }

    .courier-sidebar__collapse-btn {
      width: 28px;
      height: 28px;
      border-radius: 6px;
      color: rgba(255,255,255,0.4);
      display: flex;
      align-items: center;
      justify-content: center;
      transition: all 150ms ease;
      flex-shrink: 0;

      &:hover {
        background: rgba(255,255,255,0.08);
        color: rgba(255,255,255,0.8);
      }
    }

    /* Nav */
    .courier-sidebar__nav {
      flex: 1;
      padding: 16px 10px;
      display: flex;
      flex-direction: column;
      gap: 4px;
      overflow-y: auto;
      overflow-x: hidden;
    }

    .courier-sidebar__link {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 10px 12px;
      border-radius: 10px;
      text-decoration: none;
      color: rgba(255,255,255,0.55);
      font-size: 0.875rem;
      font-weight: 500;
      transition: all 150ms ease;
      white-space: nowrap;
      overflow: hidden;
      position: relative;

      &:hover {
        background: rgba(255,255,255,0.07);
        color: rgba(255,255,255,0.9);
      }

      &--active {
        background: rgba(249,115,22,0.15);
        color: #fb923c;

        &::before {
          content: '';
          position: absolute;
          left: 0; top: 8px; bottom: 8px;
          width: 3px;
          border-radius: 0 3px 3px 0;
          background: #f97316;
        }
      }

      &--back {
        color: rgba(255,255,255,0.35);
        &:hover { color: rgba(255,255,255,0.6); }
      }
    }

    .courier-sidebar__link-icon {
      width: 20px;
      height: 20px;
      flex-shrink: 0;
      display: flex;
      align-items: center;
      justify-content: center;
    }

    .courier-sidebar__link-label {
      font-size: 0.875rem;
      font-weight: 500;
      white-space: nowrap;
    }

    /* Footer / User */
    .courier-sidebar__footer {
      padding: 12px 10px 16px;
      border-top: 1px solid rgba(255,255,255,0.06);
      display: flex;
      flex-direction: column;
      gap: 4px;
      flex-shrink: 0;
    }

    .courier-sidebar__user {
      display: flex;
      align-items: center;
      gap: 10px;
      padding: 10px 12px;
      border-radius: 10px;
      margin-bottom: 4px;
      overflow: hidden;
    }

    .courier-sidebar__avatar {
      width: 36px;
      height: 36px;
      border-radius: 50%;
      object-fit: cover;
      border: 2px solid rgba(249,115,22,0.5);
      flex-shrink: 0;

      &--placeholder {
        background: linear-gradient(135deg, #f97316, #ea580c);
        display: flex;
        align-items: center;
        justify-content: center;
        font-size: 0.875rem;
        font-weight: 700;
        color: white;
      }
    }

    .courier-sidebar__user-info {
      display: flex;
      flex-direction: column;
      overflow: hidden;
    }

    .courier-sidebar__user-name {
      font-size: 0.8125rem;
      font-weight: 600;
      color: rgba(255,255,255,0.85);
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }

    .courier-sidebar__user-role {
      font-size: 0.6875rem;
      color: rgba(249,115,22,0.8);
      font-weight: 500;
      letter-spacing: 0.04em;
    }

    /* ====== MAIN ====== */
    .courier-main {
      flex: 1;
      margin-left: 260px;
      transition: margin-left 250ms cubic-bezier(0.4, 0, 0.2, 1);
      display: flex;
      flex-direction: column;
      min-height: 100vh;
      min-height: 100dvh;
    }

    .sidebar-collapsed .courier-main { margin-left: 72px; }

    /* Topbar */
    .courier-topbar {
      display: flex;
      align-items: center;
      justify-content: space-between;
      height: 64px;
      padding: 0 24px;
      background: var(--surface-primary);
      border-bottom: 1px solid var(--border-color);
      position: sticky;
      top: 0;
      z-index: var(--z-sticky);
      backdrop-filter: blur(8px);
    }

    .courier-topbar__left {
      display: flex;
      align-items: center;
      gap: 16px;
    }

    .courier-topbar__mobile-menu {
      display: none;
      width: 36px;
      height: 36px;
      border-radius: 8px;
      color: var(--text-secondary);
      align-items: center;
      justify-content: center;
      transition: background 150ms;

      &:hover { background: var(--surface-secondary); }
    }

    .courier-topbar__app-name {
      font-size: 0.9375rem;
      font-weight: 600;
      color: var(--text-primary);
    }

    .courier-topbar__right {
      display: flex;
      align-items: center;
      gap: 12px;
    }

    .courier-topbar__status-pill {
      display: flex;
      align-items: center;
      gap: 6px;
      padding: 5px 12px;
      background: rgba(34,197,94,0.1);
      border: 1px solid rgba(34,197,94,0.2);
      border-radius: 999px;
      font-size: 0.75rem;
      font-weight: 600;
      color: #16a34a;
    }

    .courier-topbar__status-dot {
      width: 7px;
      height: 7px;
      border-radius: 50%;
      background: #22c55e;
      animation: statusPulse 2s ease-in-out infinite;
    }

    @keyframes statusPulse {
      0%, 100% { box-shadow: 0 0 0 0 rgba(34,197,94,0.4); }
      50% { box-shadow: 0 0 0 4px rgba(34,197,94,0); }
    }

    .courier-topbar__user-chip {
      display: flex;
      align-items: center;
      gap: 8px;
      padding: 4px 12px 4px 4px;
      border-radius: 999px;
      background: var(--surface-secondary);
      border: 1px solid var(--border-color);
    }

    .courier-topbar__user-avatar {
      width: 28px;
      height: 28px;
      border-radius: 50%;
      object-fit: cover;
    }

    .courier-topbar__user-name {
      font-size: 0.8125rem;
      font-weight: 500;
      color: var(--text-secondary);
    }

    /* Content */
    .courier-content {
      flex: 1;
      background: var(--surface-secondary);
    }

    /* Mobile overlay */
    .courier-sidebar__overlay {
      display: none;
    }

    /* ====== RESPONSIVE ====== */
    @media (max-width: 767px) {
      .courier-sidebar {
        transform: translateX(-100%);
        width: 260px !important;
        transition: transform 250ms cubic-bezier(0.4, 0, 0.2, 1);
      }

      .sidebar-collapsed .courier-sidebar {
        transform: translateX(0);
      }

      .courier-sidebar__overlay {
        display: block;
        position: fixed;
        inset: 0;
        background: rgba(0,0,0,0.5);
        z-index: calc(var(--z-fixed) - 1);
        backdrop-filter: blur(2px);
      }

      .courier-main {
        margin-left: 0 !important;
      }

      .courier-topbar__mobile-menu {
        display: flex;
      }

      .sidebar-collapsed .courier-main {
        margin-left: 0 !important;
      }
    }
  `],
})
export class CourierLayoutComponent {
  protected readonly auth = inject(AuthService);
  protected readonly sidebarCollapsed = signal(false);

  toggleSidebar(): void { this.sidebarCollapsed.update((v) => !v); }
}
