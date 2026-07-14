import { Component, inject, signal, ChangeDetectionStrategy, OnInit } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { StoreApiService } from '../../../../core/services/api/store-api.service';
import { StoreCategoryApiService } from '../../../../core/services/api/store-category-api.service';
import { ToastService } from '../../../../core/services/toast.service';
import type { StoreCategoryResponse, StoreRequest } from '../../../../core/models';
import { finalize } from 'rxjs';
import { ImageUploadComponent } from '../../../../shared/components/image-upload/image-upload.component';

@Component({
  selector: 'app-setup-store',
  standalone: true,
  imports: [ReactiveFormsModule, ImageUploadComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './setup-store.component.html',
  styleUrl: './setup-store.component.scss',
})
export class SetupStoreComponent implements OnInit {
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly router = inject(Router);
  private readonly http = inject(HttpClient);
  private readonly storeApi = inject(StoreApiService);
  private readonly categoryApi = inject(StoreCategoryApiService);
  private readonly toast = inject(ToastService);
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
