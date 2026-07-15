import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '@auth0/auth0-angular';
import { ProfileApiService } from '../../../../core/services/api/profile-api.service';
import { UserCardApiService } from '../../../../core/services/api/user-card-api.service';
import { ToastService } from '../../../../core/services/toast.service';
import { type UserCardResponseDto } from '../../../../core/models';

@Component({
  selector: 'app-profile-page',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './profile-page.component.html',
  styleUrl: './profile-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProfilePageComponent implements OnInit {
  private readonly auth = inject(AuthService);
  private readonly profileApi = inject(ProfileApiService);
  private readonly userCardApi = inject(UserCardApiService);
  private readonly toast = inject(ToastService);

  readonly user = signal<{ name: string; email: string; picture: string } | null>(null);
  readonly phone = signal('');
  readonly saving = signal(false);
  readonly loading = signal(true);
  readonly loadingCards = signal(false);
  readonly savedCards = signal<UserCardResponseDto[]>([]);

  ngOnInit() {
    this.auth.user$.subscribe(user => {
      if (user) {
        this.user.set({ name: user.name ?? '', email: user.email ?? '', picture: user.picture ?? '' });
      }
    });
    this.loadProfile();
    this.loadCards();
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

  private loadCards() {
    this.loadingCards.set(true);
    this.userCardApi.getAll().subscribe({
      next: (cards) => {
        this.savedCards.set(cards);
        this.loadingCards.set(false);
      },
      error: () => {
        this.loadingCards.set(false);
      },
    });
  }

  deleteCard(cardId: string) {
    if (confirm('Deseja remover este cartão salvo?')) {
      this.userCardApi.delete(cardId).subscribe({
        next: () => {
          this.toast.success('Cartão removido.');
          this.loadCards();
        },
        error: () => this.toast.error('Erro ao remover cartão.')
      });
    }
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
