import { Component, ChangeDetectionStrategy, inject, OnInit, effect, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators, FormArray, FormGroup } from '@angular/forms';
import { CurrencyPipe } from '@angular/common';
import { AdminStore } from '../../store/admin.store';
import { StoreApiService } from '../../../../core/services/api/store-api.service';
import { LoadingSkeletonComponent } from '../../../../shared/components/loading-skeleton/loading-skeleton.component';
import { ImageUploadComponent } from '../../../../shared/components/image-upload/image-upload.component';
import { take } from 'rxjs';

@Component({
  selector: 'app-store-settings',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule, LoadingSkeletonComponent, ImageUploadComponent, CurrencyPipe],
  templateUrl: './store-settings.component.html',
  styleUrl: './store-settings.component.scss',
})
export class StoreSettingsComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  readonly adminStore = inject(AdminStore);
  private readonly storeApi = inject(StoreApiService);

  readonly activeTab = signal<'general' | 'address' | 'hours' | 'delivery'>('general');

  // Formulário Geral
  readonly generalForm = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    about: ['', [Validators.maxLength(500)]],
    imageUrl: ['', [Validators.required]],
  });

  // Formulário Endereço
  readonly addressForm = this.fb.nonNullable.group({
    zipCode: ['', [Validators.required]],
    street: ['', [Validators.required]],
    number: ['', [Validators.required]],
    complement: [''],
    neighborhood: ['', [Validators.required]],
    city: ['', [Validators.required]],
    state: ['', [Validators.required, Validators.maxLength(2)]],
  });

  // Formulário Horários
  readonly hoursForm = this.fb.nonNullable.group({
    openingHours: this.fb.array([] as FormGroup[])
  });

  // Formulário Entrega
  readonly deliveryForm = this.fb.nonNullable.group({
    deliveryPartner: [0, [Validators.required]],
    baseDeliveryFee: [5.0, [Validators.required, Validators.min(0), Validators.max(100)]],
    feePerKm: [1.5, [Validators.required, Validators.min(0), Validators.max(50)]],
    maxDeliveryDistanceKm: [15.0, [Validators.required, Validators.min(1), Validators.max(100)]],
    freeDeliveryRadiusKm: [0, [Validators.min(0), Validators.max(50)]],
  });

  readonly isSaving = signal(false);
  readonly saveSuccess = signal(false);
  readonly uploadingImage = signal(false);

  constructor() {
    effect(() => {
      const myStore = this.adminStore.myStore();
      if (myStore) {
        // Preencher Geral
        this.generalForm.patchValue({
          name: myStore.name,
          about: myStore.about,
          imageUrl: myStore.imageUrl,
        });

        // Preencher Endereço
        if (myStore.address) {
          this.addressForm.patchValue({
            zipCode: myStore.address.zipCode,
            street: myStore.address.street,
            number: myStore.address.number,
            complement: myStore.address.complement,
            neighborhood: myStore.address.neighborhood,
            city: myStore.address.city,
            state: myStore.address.state,
          });
        }

        // Preencher Horários
        this.hoursForm.controls.openingHours.clear();
        // Garante 7 dias
        for (let i = 0; i < 7; i++) {
          const existing = myStore.openingHours?.find(h => h.dayOfWeek === i);
          this.hoursForm.controls.openingHours.push(this.fb.group({
            dayOfWeek: [i],
            openHour: [existing?.openHour || '08:00', Validators.required],
            closeHour: [existing?.closeHour || '18:00', Validators.required],
            isClosed: [!existing] // Controle visual para dia fechado
          }));
        }

        // Preencher Entrega
        this.deliveryForm.patchValue({
          deliveryPartner: myStore.deliveryPartner ?? 0,
          baseDeliveryFee: myStore.baseDeliveryFee ?? 5.0,
          feePerKm: myStore.feePerKm ?? 1.5,
          maxDeliveryDistanceKm: myStore.maxDeliveryDistanceKm ?? 15.0,
          freeDeliveryRadiusKm: myStore.freeDeliveryRadiusKm ?? 0,
        });
      }
    }, { allowSignalWrites: true });
  }

  ngOnInit() {
    if (!this.adminStore.myStore() && !this.adminStore.loading()) {
      this.adminStore.loadAdminData();
    }
  }

  get hoursControls() {
    return this.hoursForm.controls.openingHours.controls;
  }

  getDayName(day: number): string {
    const days = ['Domingo', 'Segunda-feira', 'Terça-feira', 'Quarta-feira', 'Quinta-feira', 'Sexta-feira', 'Sábado'];
    return days[day];
  }

  setTab(tab: 'general' | 'address' | 'hours' | 'delivery') {
    this.activeTab.set(tab);
    this.saveSuccess.set(false);
  }

  saveGeneral() {
    if (this.generalForm.invalid) return;
    const storeId = this.adminStore.myStore()?.id;
    if (!storeId) return;

    this.isSaving.set(true);
    this.saveSuccess.set(false);
    this.storeApi.update(storeId, this.generalForm.getRawValue()).subscribe({
      next: (res) => {
        this.adminStore.updateStoreInfo(res);
        this.isSaving.set(false);
        this.saveSuccess.set(true);
      },
      error: () => this.isSaving.set(false)
    });
  }

  onStoreImageSelected(file: File) {
    this.uploadingImage.set(true);
    this.storeApi.updateImage(file).subscribe({
      next: (res) => {
        this.generalForm.patchValue({ imageUrl: res.imageUrl });
        this.uploadingImage.set(false);
      },
      error: () => this.uploadingImage.set(false),
    });
  }

  saveAddress() {
    if (this.addressForm.invalid) return;
    const storeId = this.adminStore.myStore()?.id;
    if (!storeId) return;

    this.isSaving.set(true);
    this.saveSuccess.set(false);
    this.storeApi.updateAddress(storeId, this.addressForm.getRawValue()).subscribe({
      next: (res) => {
        this.adminStore.updateStoreInfo(res);
        this.isSaving.set(false);
        this.saveSuccess.set(true);
      },
      error: () => this.isSaving.set(false)
    });
  }

  saveHours() {
    if (this.hoursForm.invalid) return;
    const storeId = this.adminStore.myStore()?.id;
    if (!storeId) return;

    this.isSaving.set(true);
    this.saveSuccess.set(false);

    // Filtrar apenas os dias que não estão fechados
    const openingHours = this.hoursForm.value.openingHours
      ?.filter((h: any) => !h.isClosed)
      .map((h: any) => ({
        dayOfWeek: h.dayOfWeek,
        openHour: h.openHour,
        closeHour: h.closeHour
      })) || [];

    this.storeApi.updateOpeningHours(storeId, { openingHours }).subscribe({
      next: (res) => {
        this.adminStore.updateStoreInfo(res);
        this.isSaving.set(false);
        this.saveSuccess.set(true);
      },
      error: () => this.isSaving.set(false)
    });
  }

  saveDelivery() {
    if (this.deliveryForm.invalid) return;
    const storeId = this.adminStore.myStore()?.id;
    if (!storeId) return;

    this.isSaving.set(true);
    this.saveSuccess.set(false);
    this.storeApi.update(storeId, {
      name: this.adminStore.myStore()!.name,
      about: this.adminStore.myStore()!.about,
      imageUrl: this.adminStore.myStore()!.imageUrl,
      deliveryPartner: this.deliveryForm.value.deliveryPartner!,
      baseDeliveryFee: this.deliveryForm.value.baseDeliveryFee!,
      feePerKm: this.deliveryForm.value.feePerKm!,
      maxDeliveryDistanceKm: this.deliveryForm.value.maxDeliveryDistanceKm!,
      freeDeliveryRadiusKm: this.deliveryForm.value.freeDeliveryRadiusKm!,
    }).subscribe({
      next: (res) => {
        this.adminStore.updateStoreInfo(res);
        this.isSaving.set(false);
        this.saveSuccess.set(true);
      },
      error: () => this.isSaving.set(false)
    });
  }
}
