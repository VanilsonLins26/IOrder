import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '@auth0/auth0-angular';
import { ProfileApiService } from '../../../../core/services/api/profile-api.service';
import { ToastService } from '../../../../core/services/toast.service';

@Component({
  selector: 'app-profile-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './profile-page.component.html',
  styleUrl: './profile-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProfilePageComponent implements OnInit {
  private readonly auth = inject(AuthService);
  private readonly profileApi = inject(ProfileApiService);
  private readonly toast = inject(ToastService);

  readonly user = signal<{ name: string; email: string; picture: string } | null>(null);
  readonly phone = signal('');
  readonly saving = signal(false);
  readonly loading = signal(true);

  ngOnInit() {
    this.auth.user$.subscribe(user => {
      if (user) {
        this.user.set({ name: user.name ?? '', email: user.email ?? '', picture: user.picture ?? '' });
      }
    });
    this.loadProfile();
  }

  private loadProfile() {
    this.loading.set(true);
    this.profileApi.get().subscribe({
      next: (profile) => {
        this.phone.set(profile.phone);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
      },
    });
  }

  save() {
    this.saving.set(true);
    this.profileApi.update({ phone: this.phone() || null }).subscribe({
      next: () => {
        this.saving.set(false);
        this.toast.success('Perfil atualizado com sucesso.');
      },
      error: (err) => {
        this.saving.set(false);
        this.toast.error(err.error?.errors?.[0] || 'Erro ao salvar perfil.');
      },
    });
  }
}
