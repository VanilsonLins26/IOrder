import { Component, ChangeDetectionStrategy, inject, signal, OnInit } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AsyncPipe } from '@angular/common';
import { AuthService } from '@auth0/auth0-angular';
import { CartStore } from '../../../features/cart/store/cart.store';
import { ChatNotificationService } from '../../../core/services/chat-notification.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, AsyncPipe],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss',
})
export class NavbarComponent {
  protected readonly auth      = inject(AuthService);
  protected readonly cartStore = inject(CartStore);
  protected readonly chatNotification = inject(ChatNotificationService);
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
