import { Component, inject, signal, ChangeDetectionStrategy, OnInit } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { StoreApiService } from '../../../../core/services/api/store-api.service';
import { StoreCategoryApiService } from '../../../../core/services/api/store-category-api.service';
import { ToastService } from '../../../../core/services/toast.service';
import { ThemeService } from '../../../../core/services/theme.service';
import type { StoreCategoryResponse, StoreRequest } from '../../../../core/models';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-setup-store',
  standalone: true,
  imports: [ReactiveFormsModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="setup-layout">
      <div class="setup-card">
        <header class="setup-header">
          <div class="setup-header__icon">🏪</div>
          <h1 class="setup-header__title">Crie sua Loja</h1>
          <p class="setup-header__subtitle">
            Falta pouco! Preencha as informações da sua loja para acessar o painel.
          </p>
        </header>

        <form [formGroup]="form" (ngSubmit)="onSubmit()" class="setup-form">
          <div class="form-row">
            <div class="form-group">
              <label class="form-label" for="name">Nome da Loja</label>
              <input id="name" type="text" formControlName="name" class="form-control" 
                     placeholder="Ex: Minha Loja Incrível" />
            </div>

            <div class="form-group">
              <label class="form-label" for="categoryId">Categoria</label>
              <select id="categoryId" formControlName="categoryId" class="form-control">
                <option value="" disabled>Selecione uma categoria...</option>
                @for (cat of storeCategories(); track cat.id) {
                  <option [value]="cat.id">{{ cat.name }}</option>
                }
              </select>
            </div>
          </div>

          <div class="form-group">
            <label class="form-label" for="about">Sobre a Loja</label>
            <textarea id="about" formControlName="about" class="form-control" rows="3"
                      placeholder="Conte um pouco sobre a sua loja..."></textarea>
          </div>

          <div class="form-group">
            <label class="form-label" for="imageUrl">URL da Logo (Opcional)</label>
            <input id="imageUrl" type="text" formControlName="imageUrl" class="form-control" 
                   placeholder="https://..." />
          </div>

          <div class="form-divider">Endereço</div>

          <div class="form-row form-row--cep">
            <div class="form-group">
              <label class="form-label" for="zipCode">CEP</label>
              <div class="input-with-button">
                <input id="zipCode" type="text" formControlName="zipCode" class="form-control" 
                       placeholder="00000-000" (blur)="buscarCep()" />
                @if (loadingCep()) {
                  <span class="spinner"></span>
                }
              </div>
            </div>
            
            <div class="form-group" style="flex: 2;">
              <label class="form-label" for="street">Rua</label>
              <input id="street" type="text" formControlName="street" class="form-control" />
            </div>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label class="form-label" for="number">Número</label>
              <input id="number" type="text" formControlName="number" class="form-control" />
            </div>
            <div class="form-group">
              <label class="form-label" for="complement">Complemento</label>
              <input id="complement" type="text" formControlName="complement" class="form-control" />
            </div>
            <div class="form-group">
              <label class="form-label" for="neighborhood">Bairro</label>
              <input id="neighborhood" type="text" formControlName="neighborhood" class="form-control" />
            </div>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label class="form-label" for="city">Cidade</label>
              <input id="city" type="text" formControlName="city" class="form-control" />
            </div>
            <div class="form-group">
              <label class="form-label" for="state">Estado (UF)</label>
              <input id="state" type="text" formControlName="state" class="form-control" />
            </div>
          </div>

          <button type="submit" class="btn-submit" [disabled]="form.invalid || loading()">
            @if (loading()) {
              Criando...
            } @else {
              Criar Loja
            }
          </button>
        </form>
        
        <button class="theme-toggle" (click)="themeService.toggle()" aria-label="Alternar tema">
          {{ themeService.isDark() ? '☀️' : '🌙' }}
        </button>
      </div>
    </div>
  `,
  styles: [`
    .setup-layout {
      min-height: 100vh;
      display: flex;
      align-items: center;
      justify-content: center;
      background: linear-gradient(135deg, var(--color-primary-50), var(--color-primary-100));
      padding: var(--space-4);
      position: relative;
      
      :host-context([data-theme='dark']) & {
        background: linear-gradient(135deg, var(--color-neutral-900), var(--color-neutral-800));
      }
    }

    .setup-card {
      background: var(--surface-primary);
      width: 100%;
      max-width: 700px;
      border-radius: var(--radius-xl);
      padding: var(--space-8);
      box-shadow: 0 20px 40px rgba(0,0,0,0.08);
      position: relative;
      border: 1px solid rgba(255, 255, 255, 0.4);
      backdrop-filter: blur(20px);
      
      :host-context([data-theme='dark']) & {
        border-color: rgba(255, 255, 255, 0.05);
        box-shadow: 0 20px 40px rgba(0,0,0,0.3);
      }
    }

    .setup-header {
      text-align: center;
      margin-bottom: var(--space-6);
    }
    
    .setup-header__icon {
      font-size: 3rem;
      margin-bottom: var(--space-2);
      animation: bounceIn 0.8s cubic-bezier(0.175, 0.885, 0.32, 1.275);
    }

    .setup-header__title {
      font-size: var(--font-size-2xl);
      font-weight: var(--font-weight-bold);
      color: var(--text-primary);
      margin-bottom: var(--space-2);
    }

    .setup-header__subtitle {
      color: var(--text-secondary);
      font-size: var(--font-size-sm);
    }

    .setup-form {
      display: flex;
      flex-direction: column;
      gap: var(--space-4);
    }
    
    .form-row {
      display: flex;
      flex-wrap: wrap;
      gap: var(--space-4);
      > * { flex: 1; min-width: 150px; }
    }
    
    .form-divider {
      margin: var(--space-2) 0;
      color: var(--text-primary);
      font-weight: var(--font-weight-semibold);
      font-size: var(--font-size-sm);
      text-transform: uppercase;
      letter-spacing: 0.05em;
      border-bottom: 1px solid var(--border-color);
      padding-bottom: var(--space-2);
    }

    .form-group {
      display: flex;
      flex-direction: column;
      gap: var(--space-2);
    }

    .form-label {
      font-size: var(--font-size-sm);
      font-weight: var(--font-weight-medium);
      color: var(--text-secondary);
    }

    .form-control {
      padding: var(--space-3) var(--space-4);
      background: var(--surface-secondary);
      border: 1px solid var(--border-color);
      border-radius: var(--radius-lg);
      color: var(--text-primary);
      font-size: var(--font-size-sm);
      transition: all var(--transition-fast);

      &:focus {
        outline: none;
        border-color: var(--color-primary-500);
        box-shadow: 0 0 0 3px rgba(var(--color-primary-500-rgb), 0.15);
      }
      
      &:disabled { opacity: 0.7; cursor: not-allowed; }
    }
    
    .input-with-button {
      position: relative;
      display: flex;
      align-items: center;
      
      .form-control { width: 100%; }
      .spinner {
        position: absolute;
        right: 12px;
        width: 16px; height: 16px;
        border: 2px solid var(--color-primary-500);
        border-right-color: transparent;
        border-radius: 50%;
        animation: spin 0.75s linear infinite;
      }
    }

    .btn-submit {
      margin-top: var(--space-4);
      padding: var(--space-4);
      background: var(--color-primary-500);
      color: white;
      border: none;
      border-radius: var(--radius-lg);
      font-size: var(--font-size-base);
      font-weight: var(--font-weight-semibold);
      cursor: pointer;
      transition: all var(--transition-fast);
      box-shadow: 0 4px 12px rgba(var(--color-primary-500-rgb), 0.3);

      &:hover:not(:disabled) {
        background: var(--color-primary-600);
        transform: translateY(-2px);
      }

      &:disabled {
        opacity: 0.7;
        cursor: not-allowed;
        transform: none;
      }
    }
    
    .theme-toggle {
      position: absolute;
      top: var(--space-4);
      right: var(--space-4);
      background: var(--surface-secondary);
      border: 1px solid var(--border-color);
      width: 40px; height: 40px;
      border-radius: var(--radius-full);
      display: flex; align-items: center; justify-content: center;
      cursor: pointer;
      font-size: 1.2rem;
      transition: background 0.2s;
      
      &:hover { background: var(--border-color); }
    }

    @keyframes bounceIn {
      0% { transform: scale(0.3); opacity: 0; }
      50% { transform: scale(1.05); opacity: 1; }
      70% { transform: scale(0.9); }
      100% { transform: scale(1); }
    }
    
    @keyframes spin {
      100% { transform: rotate(360deg); }
    }
  `],
})
export class SetupStoreComponent implements OnInit {
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly router = inject(Router);
  private readonly http = inject(HttpClient);
  private readonly storeApi = inject(StoreApiService);
  private readonly categoryApi = inject(StoreCategoryApiService);
  private readonly toast = inject(ToastService);
  protected readonly themeService = inject(ThemeService);

  readonly loading = signal(false);
  readonly loadingCep = signal(false);
  readonly storeCategories = signal<StoreCategoryResponse[]>([]);

  readonly form = this.fb.group({
    name: ['', Validators.required],
    about: ['', Validators.required],
    imageUrl: [''],
    categoryId: ['', Validators.required],
    zipCode: ['', [Validators.required, Validators.pattern(/^[0-9]{5}-?[0-9]{3}$/)]],
    street: ['', Validators.required],
    number: ['', Validators.required],
    complement: [''],
    neighborhood: ['', Validators.required],
    city: ['', Validators.required],
    state: ['', Validators.required]
  });

  ngOnInit(): void {
    this.loadCategories();
  }

  private loadCategories(): void {
    this.categoryApi.getAll().subscribe({
      next: (categories) => this.storeCategories.set(categories),
      error: () => this.toast.error('Erro ao carregar categorias de loja')
    });
  }

  buscarCep(): void {
    let cep = this.form.get('zipCode')?.value || '';
    cep = cep.replace(/\D/g, '');

    if (cep.length === 8) {
      this.loadingCep.set(true);
      this.http.get<any>(`https://viacep.com.br/ws/${cep}/json/`)
        .pipe(finalize(() => this.loadingCep.set(false)))
        .subscribe({
          next: (data) => {
            if (!data.erro) {
              this.form.patchValue({
                street: data.logradouro,
                neighborhood: data.bairro,
                city: data.localidade,
                state: data.uf
              });
            } else {
              this.toast.error('CEP não encontrado.');
            }
          },
          error: () => this.toast.error('Erro ao buscar o CEP.')
        });
    }
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.toast.error('Por favor, preencha todos os campos obrigatórios.');
      return;
    }

    this.loading.set(true);
    
    // Create default opening hours
    const openingHours = Array.from({length: 7}).map((_, i) => ({
      dayOfWeek: i,
      openHour: "09:00",
      closeHour: "18:00"
    }));

    const val = this.form.getRawValue();
    const req: StoreRequest = {
      name: val.name,
      about: val.about,
      imageUrl: val.imageUrl || 'https://via.placeholder.com/150',
      categoryId: val.categoryId,
      openingHours: openingHours,
      address: {
        zipCode: val.zipCode,
        street: val.street,
        number: val.number,
        complement: val.complement,
        neighborhood: val.neighborhood,
        city: val.city,
        state: val.state
      }
    };

    this.storeApi.create(req).subscribe({
      next: () => {
        this.toast.success('Loja criada com sucesso! Bem-vindo.');
        this.router.navigate(['/admin/dashboard']);
      },
      error: (err: any) => {
        this.toast.error(err.error?.message || 'Erro ao criar a loja.');
        this.loading.set(false);
      }
    });
  }
}
