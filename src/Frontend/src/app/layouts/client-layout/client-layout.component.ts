import { Component, ChangeDetectionStrategy, inject, OnInit } from '@angular/core';
import { RouterOutlet, Router } from '@angular/router';
import { NavbarComponent } from '../../shared/components/navbar/navbar.component';
import { FooterComponent } from '../../shared/components/footer/footer.component';
import { AuthService } from '../../core/auth/auth.service';
import { Roles } from '../../core/auth/role.guard';

@Component({
  selector: 'app-client-layout',
  standalone: true,
  imports: [RouterOutlet, NavbarComponent, FooterComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="client-layout">
      <app-navbar />
      <main class="client-layout__content">
        <router-outlet />
      </main>
      <app-footer />
    </div>
  `,
  styles: [`
    .client-layout {
      display: flex;
      flex-direction: column;
      min-height: 100vh;
      min-height: 100dvh;
    }

    .client-layout__content { flex: 1; }
  `],
})
export class ClientLayoutComponent implements OnInit {
  private authService = inject(AuthService);
  private router = inject(Router);

  ngOnInit(): void {
    this.authService.user$.subscribe(user => {
      if (user && user.roles && user.roles.includes(Roles.ShopKeeper)) {
        this.router.navigate(['/admin']);
      }
    });
  }
}
