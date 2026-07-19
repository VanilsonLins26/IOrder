import { Component, ChangeDetectionStrategy, signal, inject } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AsyncPipe } from '@angular/common';
import { AuthService } from '@auth0/auth0-angular';
import { ChatNotificationService } from '../../core/services/chat-notification.service';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, AsyncPipe],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="admin-layout" [class.sidebar-collapsed]="sidebarCollapsed()">

      <!-- ======= SIDEBAR ======= -->
      <aside class="admin-sidebar" role="navigation" aria-label="Menu administrativo">

        <!-- Brand -->
        <div class="admin-sidebar__brand">
          <a routerLink="/" class="admin-sidebar__logo" [title]="sidebarCollapsed() ? 'IOrder' : ''">
            <div class="admin-sidebar__logo-icon">
              <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="white" stroke-width="2.5" stroke-linecap="round">
                <path d="M6 2L3 6v14a2 2 0 002 2h14a2 2 0 002-2V6l-3-4z"/>
                <line x1="3" y1="6" x2="21" y2="6"/>
                <path d="M16 10a4 4 0 01-8 0"/>
              </svg>
            </div>
            @if (!sidebarCollapsed()) {
              <span class="admin-sidebar__logo-text">IOrder</span>
            }
          </a>
          <button class="admin-sidebar__collapse-btn" (click)="toggleSidebar()"
                  [attr.aria-label]="sidebarCollapsed() ? 'Expandir menu' : 'Recolher menu'">
            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round">
              @if (sidebarCollapsed()) { <polyline points="9 18 15 12 9 6"></polyline> }
              @else { <polyline points="15 18 9 12 15 6"></polyline> }
            </svg>
          </button>
        </div>

        <!-- Section Label -->
        @if (!sidebarCollapsed()) {
          <div class="admin-sidebar__section-label">PAINEL</div>
        }

        <!-- Nav -->
        <nav class="admin-sidebar__nav">
          <a routerLink="/admin/dashboard" routerLinkActive="admin-sidebar__link--active"
             class="admin-sidebar__link" [title]="sidebarCollapsed() ? 'Dashboard' : ''">
            <span class="admin-sidebar__link-icon">
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round">
                <rect x="3" y="3" width="7" height="7"/><rect x="14" y="3" width="7" height="7"/>
                <rect x="14" y="14" width="7" height="7"/><rect x="3" y="14" width="7" height="7"/>
              </svg>
            </span>
            @if (!sidebarCollapsed()) { <span class="admin-sidebar__link-label">Dashboard</span> }
          </a>

          <a routerLink="/admin/orders" routerLinkActive="admin-sidebar__link--active"
             class="admin-sidebar__link" [title]="sidebarCollapsed() ? 'Pedidos' : ''">
            <span class="admin-sidebar__link-icon">
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round">
                <path d="M6 2L3 6v14a2 2 0 002 2h14a2 2 0 002-2V6l-3-4z"/><line x1="3" y1="6" x2="21" y2="6"/>
                <path d="M16 10a4 4 0 01-8 0"/>
              </svg>
            </span>
            @if (!sidebarCollapsed()) {
              <span class="admin-sidebar__link-label">Pedidos</span>
              @if (chatNotification.totalUnread() > 0) {
                <span class="admin-sidebar__badge">{{ chatNotification.totalUnread() > 9 ? '9+' : chatNotification.totalUnread() }}</span>
              }
            } @else if (chatNotification.totalUnread() > 0) {
              <span class="admin-sidebar__badge-dot"></span>
            }
          </a>

          <a routerLink="/admin/products" routerLinkActive="admin-sidebar__link--active"
             class="admin-sidebar__link" [title]="sidebarCollapsed() ? 'Produtos' : ''">
            <span class="admin-sidebar__link-icon">
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round">
                <path d="M21 16V8a2 2 0 00-1-1.73l-7-4a2 2 0 00-2 0l-7 4A2 2 0 003 8v8a2 2 0 001 1.73l7 4a2 2 0 002 0l7-4A2 2 0 0021 16z"/>
                <polyline points="3.27 6.96 12 12.01 20.73 6.96"/><line x1="12" y1="22.08" x2="12" y2="12"/>
              </svg>
            </span>
            @if (!sidebarCollapsed()) { <span class="admin-sidebar__link-label">Produtos</span> }
          </a>

          <a routerLink="/admin/categories" routerLinkActive="admin-sidebar__link--active"
             class="admin-sidebar__link" [title]="sidebarCollapsed() ? 'Categorias' : ''">
            <span class="admin-sidebar__link-icon">
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round">
                <path d="M20.59 13.41l-7.17 7.17a2 2 0 01-2.83 0L2 12V2h10l8.59 8.59a2 2 0 010 2.82z"/><line x1="7" y1="7" x2="7.01" y2="7"/>
              </svg>
            </span>
            @if (!sidebarCollapsed()) { <span class="admin-sidebar__link-label">Categorias</span> }
          </a>

          <a routerLink="/admin/store" routerLinkActive="admin-sidebar__link--active"
             class="admin-sidebar__link" [title]="sidebarCollapsed() ? 'Minha Loja' : ''">
            <span class="admin-sidebar__link-icon">
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round">
                <path d="M3 9l9-7 9 7v11a2 2 0 01-2 2H5a2 2 0 01-2-2z"/>
                <polyline points="9 22 9 12 15 12 15 22"/>
              </svg>
            </span>
            @if (!sidebarCollapsed()) { <span class="admin-sidebar__link-label">Minha Loja</span> }
          </a>
        </nav>

        <!-- Footer -->
        <div class="admin-sidebar__footer">
          @if (auth.user$ | async; as user) {
            <div class="admin-sidebar__user" [title]="sidebarCollapsed() ? (user.name || '') : ''">
              @if (user.picture) {
                <img [src]="user.picture" [alt]="user.name || 'Avatar'" class="admin-sidebar__avatar" />
              } @else {
                <div class="admin-sidebar__avatar admin-sidebar__avatar--placeholder">
                  {{ (user.name ?? 'U')[0] }}
                </div>
              }
              @if (!sidebarCollapsed()) {
                <div class="admin-sidebar__user-info">
                  <span class="admin-sidebar__user-name">{{ user.name }}</span>
                  <span class="admin-sidebar__user-role">Lojista</span>
                </div>
              }
            </div>
          }
          <a routerLink="/" class="admin-sidebar__link admin-sidebar__link--back" [title]="sidebarCollapsed() ? 'Voltar ao site' : ''">
            <span class="admin-sidebar__link-icon">
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round">
                <path d="M15 18l-6-6 6-6"/>
              </svg>
            </span>
            @if (!sidebarCollapsed()) { <span class="admin-sidebar__link-label">Voltar ao Site</span> }
          </a>
          <button class="admin-sidebar__link admin-sidebar__link--danger"
                  (click)="logout()" [title]="sidebarCollapsed() ? 'Sair' : ''">
            <span class="admin-sidebar__link-icon">
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round">
                <path d="M9 21H5a2 2 0 01-2-2V5a2 2 0 012-2h4"/>
                <polyline points="16 17 21 12 16 7"/><line x1="21" y1="12" x2="9" y2="12"/>
              </svg>
            </span>
            @if (!sidebarCollapsed()) { <span class="admin-sidebar__link-label">Sair</span> }
          </button>
        </div>
      </aside>

      <!-- ======= MAIN ======= -->
      <div class="admin-main">
        <header class="admin-topbar">
          <div class="admin-topbar__left">
            <button class="admin-topbar__mobile-menu" (click)="toggleSidebar()" aria-label="Abrir menu">
              <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round">
                <line x1="3" y1="6" x2="21" y2="6"/><line x1="3" y1="12" x2="21" y2="12"/><line x1="3" y1="18" x2="21" y2="18"/>
              </svg>
            </button>
            <span class="admin-topbar__title">Painel do Lojista</span>
          </div>
          <div class="admin-topbar__right">
            @if (auth.user$ | async; as user) {
              <div class="admin-topbar__user-chip">
                @if (user.picture) {
                  <img [src]="user.picture" [alt]="user.name || ''" class="admin-topbar__avatar" />
                }
                <span class="admin-topbar__user-name">{{ (user.name ?? '').split(' ')[0] }}</span>
              </div>
            }
          </div>
        </header>

        <main class="admin-content">
          <router-outlet />
        </main>
      </div>

      <!-- Mobile overlay -->
      @if (!sidebarCollapsed()) {
        <div class="admin-sidebar__overlay" (click)="toggleSidebar()"></div>
      }
    </div>
  `,
  styles: [`
    .admin-layout {
      display: flex;
      min-height: 100vh;
      min-height: 100dvh;
      background: var(--surface-primary);
    }

    /* ====== SIDEBAR ====== */
    .admin-sidebar {
      width: 260px;
      background: linear-gradient(180deg, #0f172a 0%, #1e293b 100%);
      display: flex;
      flex-direction: column;
      position: fixed;
      top: 0; left: 0; bottom: 0;
      z-index: var(--z-fixed);
      transition: width 250ms cubic-bezier(0.4,0,0.2,1);
      overflow: hidden;
      box-shadow: 4px 0 24px rgba(0,0,0,0.15);
    }

    .sidebar-collapsed .admin-sidebar { width: 72px; }

    /* Brand */
    .admin-sidebar__brand {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 0 16px;
      height: 64px;
      border-bottom: 1px solid rgba(255,255,255,0.06);
      flex-shrink: 0;
    }

    .admin-sidebar__logo {
      display: flex;
      align-items: center;
      gap: 10px;
      text-decoration: none;
      overflow: hidden;
    }

    .admin-sidebar__logo-icon {
      width: 36px; height: 36px;
      border-radius: 10px;
      background: linear-gradient(135deg, var(--color-primary-500), var(--color-primary-700));
      display: flex;
      align-items: center;
      justify-content: center;
      flex-shrink: 0;
      box-shadow: 0 4px 12px rgba(59,130,246,0.4);
    }

    .admin-sidebar__logo-text {
      font-size: 1.1875rem;
      font-weight: 800;
      color: white;
      white-space: nowrap;
      letter-spacing: -0.02em;
    }

    .admin-sidebar__collapse-btn {
      width: 28px; height: 28px;
      border-radius: 6px;
      color: rgba(255,255,255,0.4);
      display: flex;
      align-items: center;
      justify-content: center;
      transition: all 150ms ease;
      flex-shrink: 0;
      &:hover { background: rgba(255,255,255,0.08); color: rgba(255,255,255,0.8); }
    }

    .admin-sidebar__section-label {
      padding: 16px 16px 6px;
      font-size: 0.625rem;
      font-weight: 700;
      color: rgba(255,255,255,0.25);
      letter-spacing: 0.1em;
      flex-shrink: 0;
    }

    .admin-sidebar__nav {
      flex: 1;
      padding: 8px 10px;
      display: flex;
      flex-direction: column;
      gap: 2px;
      overflow-y: auto;
      overflow-x: hidden;
    }

    .admin-sidebar__link {
      display: flex;
      align-items: center;
      gap: 10px;
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
      border: none;
      background: transparent;
      width: 100%;
      cursor: pointer;
      text-align: left;

      &:hover {
        background: rgba(255,255,255,0.07);
        color: rgba(255,255,255,0.9);
      }

      &--active {
        background: rgba(59,130,246,0.15);
        color: #60a5fa;
        &::before {
          content: '';
          position: absolute;
          left: 0; top: 8px; bottom: 8px;
          width: 3px;
          border-radius: 0 3px 3px 0;
          background: var(--color-primary-400);
        }
      }

      &--back { color: rgba(255,255,255,0.35); &:hover { color: rgba(255,255,255,0.6); } }
      &--danger { &:hover { background: rgba(239,68,68,0.1); color: #fca5a5; } }
    }

    .admin-sidebar__link-icon {
      width: 20px; height: 20px;
      flex-shrink: 0;
      display: flex;
      align-items: center;
      justify-content: center;
    }

    .admin-sidebar__link-label {
      font-size: 0.875rem;
      font-weight: 500;
      white-space: nowrap;
    }

    .admin-sidebar__badge {
      margin-left: auto;
      display: inline-flex;
      align-items: center;
      justify-content: center;
      min-width: 20px; height: 20px;
      padding: 0 5px;
      background: #ef4444;
      color: white;
      font-size: 0.6875rem;
      font-weight: 700;
      border-radius: 999px;
    }

    .admin-sidebar__badge-dot {
      position: absolute;
      top: 6px; right: 6px;
      width: 8px; height: 8px;
      border-radius: 50%;
      background: #ef4444;
      border: 2px solid #1e293b;
    }

    /* Footer */
    .admin-sidebar__footer {
      padding: 10px;
      border-top: 1px solid rgba(255,255,255,0.06);
      display: flex;
      flex-direction: column;
      gap: 2px;
      flex-shrink: 0;
    }

    .admin-sidebar__user {
      display: flex;
      align-items: center;
      gap: 10px;
      padding: 10px 12px;
      border-radius: 10px;
      margin-bottom: 2px;
      overflow: hidden;
    }

    .admin-sidebar__avatar {
      width: 34px; height: 34px;
      border-radius: 50%;
      object-fit: cover;
      border: 2px solid rgba(59,130,246,0.4);
      flex-shrink: 0;
      &--placeholder {
        background: linear-gradient(135deg, var(--color-primary-400), var(--color-primary-700));
        display: flex;
        align-items: center;
        justify-content: center;
        font-size: 0.875rem;
        font-weight: 700;
        color: white;
      }
    }

    .admin-sidebar__user-info { display: flex; flex-direction: column; overflow: hidden; }

    .admin-sidebar__user-name {
      font-size: 0.8125rem;
      font-weight: 600;
      color: rgba(255,255,255,0.85);
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }

    .admin-sidebar__user-role {
      font-size: 0.6875rem;
      color: rgba(96,165,250,0.8);
      font-weight: 500;
      letter-spacing: 0.04em;
    }

    /* ====== MAIN ====== */
    .admin-main {
      flex: 1;
      margin-left: 260px;
      transition: margin-left 250ms cubic-bezier(0.4,0,0.2,1);
      display: flex;
      flex-direction: column;
      min-height: 100vh;
      min-height: 100dvh;
    }

    .sidebar-collapsed .admin-main { margin-left: 72px; }

    /* Topbar */
    .admin-topbar {
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

    .admin-topbar__left {
      display: flex;
      align-items: center;
      gap: 16px;
    }

    .admin-topbar__mobile-menu {
      display: none;
      width: 36px; height: 36px;
      border-radius: 8px;
      color: var(--text-secondary);
      align-items: center;
      justify-content: center;
      transition: background 150ms;
      &:hover { background: var(--surface-secondary); }
    }

    .admin-topbar__title {
      font-size: 0.9375rem;
      font-weight: 600;
      color: var(--text-primary);
    }

    .admin-topbar__right {
      display: flex;
      align-items: center;
      gap: 12px;
    }

    .admin-topbar__user-chip {
      display: flex;
      align-items: center;
      gap: 8px;
      padding: 4px 12px 4px 4px;
      border-radius: 999px;
      background: var(--surface-secondary);
      border: 1px solid var(--border-color);
    }

    .admin-topbar__avatar {
      width: 28px; height: 28px;
      border-radius: 50%;
      object-fit: cover;
    }

    .admin-topbar__user-name {
      font-size: 0.8125rem;
      font-weight: 500;
      color: var(--text-secondary);
    }

    .admin-content {
      flex: 1;
      background: var(--surface-secondary);
    }

    .admin-sidebar__overlay { display: none; }

    @media (max-width: 767px) {
      .admin-sidebar {
        transform: translateX(-100%);
        width: 260px !important;
        transition: transform 250ms cubic-bezier(0.4,0,0.2,1);
      }
      .sidebar-collapsed .admin-sidebar { transform: translateX(0); }
      .admin-sidebar__overlay {
        display: block;
        position: fixed;
        inset: 0;
        background: rgba(0,0,0,0.5);
        z-index: calc(var(--z-fixed) - 1);
        backdrop-filter: blur(2px);
      }
      .admin-main { margin-left: 0 !important; }
      .admin-topbar__mobile-menu { display: flex; }
      .sidebar-collapsed .admin-main { margin-left: 0 !important; }
    }
  `],
})
export class AdminLayoutComponent {
  protected readonly auth = inject(AuthService);
  protected readonly chatNotification = inject(ChatNotificationService);
  protected readonly sidebarCollapsed = signal(false);

  toggleSidebar(): void { this.sidebarCollapsed.update((v) => !v); }
  logout(): void { this.auth.logout({ logoutParams: { returnTo: window.location.origin } }); }
}
