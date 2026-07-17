import { Component, ChangeDetectionStrategy, inject, signal, OnInit } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators, FormArray } from '@angular/forms';
import { AdminStore } from '../../store/admin.store';
import { ProductApiService } from '../../../../core/services/api/product-api.service';
import { CategoryApiService } from '../../../../core/services/api/category-api.service';
import { CustomizationApiService } from '../../../../core/services/api/customization-api.service';
import { ModalComponent } from '../../../../shared/components/modal/modal.component';
import { ConfirmationModalComponent } from '../../../../shared/components/confirmation-modal/confirmation-modal.component';
import { LoadingSkeletonComponent } from '../../../../shared/components/loading-skeleton/loading-skeleton.component';
import { ImageUploadComponent } from '../../../../shared/components/image-upload/image-upload.component';
import { CurrencyInputDirective } from '../../../../shared/directives/currency-input.directive';
import { ProductResponse, ProductRequest, UpdateProductRequest, UnitOfMeasure, PromotionPriceRequest } from '../../../../core/models';
import type { CustomizationGroup } from '../../../../core/models/customization.model';

@Component({
  selector: 'app-product-management',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CurrencyPipe, ReactiveFormsModule, ModalComponent, ConfirmationModalComponent, LoadingSkeletonComponent, ImageUploadComponent, CurrencyInputDirective],
  templateUrl: './product-management.component.html',
  styleUrl: './product-management.component.scss',
})
export class ProductManagementComponent implements OnInit {
  readonly adminStore = inject(AdminStore);
  private readonly productApi = inject(ProductApiService);
  private readonly categoryApi = inject(CategoryApiService);
  private readonly customizationApi = inject(CustomizationApiService);
  private readonly fb = inject(FormBuilder);

  // Enum para template
  readonly UnitOfMeasure = UnitOfMeasure;

  // Modals state
  readonly isProductModalOpen = signal(false);
  readonly isPromoModalOpen = signal(false);
  readonly isCustomizationModalOpen = signal(false);
  readonly isSaving = signal(false);
  readonly isSavingCustomization = signal(false);
  readonly uploadingImage = signal(false);
  readonly editingProduct = signal<ProductResponse | null>(null);
  readonly customizationGroups = signal<CustomizationGroup[]>([]);
  readonly editingCustomizationGroup = signal<CustomizationGroup | null>(null);
  readonly loadingCustomization = signal(false);
  private readonly pendingImage = signal<File | null>(null);

  // Confirmation modals
  readonly showDeleteProductConfirm = signal(false);
  readonly productToDelete = signal<string | null>(null);
  readonly showDeleteCustomizationConfirm = signal(false);
  readonly customizationGroupToDelete = signal<string | null>(null);

  // Forms
  readonly productForm = this.fb.nonNullable.group({
    name: ['', [Validators.required]],
    description: [''],
    price: [0, [Validators.required, Validators.min(0)]],
    imageUrl: [''],
    unitOfMeasure: [0, [Validators.required]],
    customizable: [false],
    categoryId: [''] // Será usado para vincular pós-criação ou na edição
  });

  readonly customizationForm = this.fb.nonNullable.group({
    name: ['', [Validators.required]],
    type: ['SingleChoice' as 'SingleChoice' | 'MultipleChoice', [Validators.required]],
    minSelections: [1],
    maxSelections: [1],
    required: [true],
    position: [0],
    options: this.fb.array([
      this.createOptionFormGroup()
    ])
  });

  private createOptionFormGroup() {
    return this.fb.nonNullable.group({
      id: [''],
      name: ['', [Validators.required]],
      priceModifier: [0],
      position: [0],
    });
  }

  get optionsArray() {
    return this.customizationForm.get('options') as FormArray;
  }

  addOption() {
    this.optionsArray.push(this.createOptionFormGroup());
  }

  removeOption(index: number) {
    this.optionsArray.removeAt(index);
  }

  readonly promoForm = this.fb.nonNullable.group({
    price: [0, [Validators.required, Validators.min(0.01)]],
    initialTime: ['', [Validators.required]], // DateTime-local string
    finalTime: ['', [Validators.required]] // DateTime-local string
  });

  ngOnInit() {
    if (!this.adminStore.myStore() && !this.adminStore.loading()) {
      this.adminStore.loadAdminData();
    }
  }

  // --- CRUD Produto ---
  openCreateProduct() {
    this.editingProduct.set(null);
    this.productForm.reset({
      name: '',
      description: '',
      price: 0,
      imageUrl: '',
      unitOfMeasure: 0,
      customizable: false,
      categoryId: ''
    });
    this.isProductModalOpen.set(true);
  }

  openEditProduct(product: ProductResponse) {
    this.editingProduct.set(product);
    this.productForm.patchValue({
      name: product.name,
      description: product.description || '',
      price: product.price,
      imageUrl: product.imageUrl || '',
      unitOfMeasure: product.unitOfMeasure,
      customizable: product.customizable,
      // Para o categoryId teríamos que descobrir em qual categoria o produto está.
      // O admin.store tem categories, com a lista de products.
      categoryId: this.findCategoryForProduct(product.id, product)
    });
    this.isProductModalOpen.set(true);
  }

  private findCategoryForProduct(productId: string, product?: ProductResponse): string {
    if (product?.categoryId) {
      return product.categoryId;
    }
    
    // Fallback just in case
    const categories = this.adminStore.categories();
    for (const cat of categories) {
      if ((cat as any).products?.some((p: any) => p.id === productId)) {
        return cat.id;
      }
    }
    return '';
  }

  onProductImageSelected(file: File) {
    const currentEditing = this.editingProduct();
    if (!currentEditing) {
      this.pendingImage.set(file);
      return;
    }
    this.uploadingImage.set(true);
    this.productApi.updateImage(currentEditing.id, file).subscribe({
      next: (res) => {
        this.productForm.patchValue({ imageUrl: res.imageUrl });
        this.uploadingImage.set(false);
      },
      error: () => this.uploadingImage.set(false),
    });
  }

  closeProductModal() {
    this.isProductModalOpen.set(false);
  }

  saveProduct() {
    if (this.productForm.invalid) return;

    this.isSaving.set(true);
    const formVal = this.productForm.getRawValue();
    const productData = {
      name: formVal.name,
      description: formVal.description,
      price: formVal.price,
      imageUrl: formVal.imageUrl,
      unitOfMeasure: Number(formVal.unitOfMeasure),
      customizable: formVal.customizable,
    };

    const storeId = this.adminStore.myStore()?.id;
    if (!storeId) return;

    const currentEditing = this.editingProduct();
    if (currentEditing) {
      // Edit
      const req: UpdateProductRequest = {
        ...productData,
        active: currentEditing.active
      };

      this.productApi.update(currentEditing.id, req).subscribe({
        next: (res) => {
          this.adminStore.updateProduct(res);
          this.syncCategory(res.id, formVal.categoryId, this.findCategoryForProduct(currentEditing.id, currentEditing));
        },
        error: () => this.isSaving.set(false)
      });
    } else {
      // Create
      const req: ProductRequest = {
        ...productData,
        storeId
      };

      this.productApi.create(req).subscribe({
        next: (res) => {
          this.adminStore.addProduct(res);
          const file = this.pendingImage();
          if (file) {
            this.pendingImage.set(null);
            this.productApi.updateImage(res.id, file).subscribe({
              next: () => {
                this.adminStore.loadAdminData();
                this.syncCategory(res.id, formVal.categoryId, '');
              },
              error: () => this.syncCategory(res.id, formVal.categoryId, ''),
            });
          } else {
            this.syncCategory(res.id, formVal.categoryId, '');
          }
        },
        error: () => this.isSaving.set(false)
      });
    }
  }

  private syncCategory(productId: string, newCategoryId: string, oldCategoryId: string) {
    // Se não houver mudanças na categoria
    if (newCategoryId === oldCategoryId) {
      this.closeProductModal();
      this.isSaving.set(false);
      this.adminStore.loadAdminData(); // Reload for safety to update categories product list
      return;
    }

    // Se tiver que remover da antiga, teria que chamar endpoint.
    // Para simplificar, vou apenas chamar o addProducts na nova categoria (se houver), 
    // ou confiar no loadAdminData para puxar os dados frescos se for apenas adição.
    if (newCategoryId) {
      this.categoryApi.addProducts(newCategoryId, { productIds: [productId] }).subscribe({
        next: () => {
          this.closeProductModal();
          this.isSaving.set(false);
          this.adminStore.loadAdminData(); // Refresh global data
        },
        error: () => {
          this.closeProductModal();
          this.isSaving.set(false);
          this.adminStore.loadAdminData();
        }
      });
    } else {
      this.closeProductModal();
      this.isSaving.set(false);
      this.adminStore.loadAdminData();
    }
  }

  // --- Customization ---
  openCustomization(product: ProductResponse) {
    this.editingProduct.set(product);
    this.customizationGroups.set([]);
    this.openAddGroup();
    this.loadingCustomization.set(true);
    this.isCustomizationModalOpen.set(true);
    this.customizationApi.getByProduct(product.id).subscribe({
      next: (groups) => {
        this.customizationGroups.set(groups);
        this.loadingCustomization.set(false);
      },
      error: () => {
        this.loadingCustomization.set(false);
      },
    });
  }

  closeCustomizationModal() {
    this.isCustomizationModalOpen.set(false);
  }

  openAddGroup() {
    this.editingCustomizationGroup.set(null);
    this.customizationForm.reset({
      name: '',
      type: 'SingleChoice',
      minSelections: 1,
      maxSelections: 1,
      required: true,
      position: this.customizationGroups().length,
    });
    this.optionsArray.clear();
    this.addOption();
  }

  openEditGroup(group: CustomizationGroup) {
    this.editingCustomizationGroup.set(group);
    this.optionsArray.clear();
    for (const opt of group.options) {
      const fg = this.createOptionFormGroup();
      fg.patchValue({
        id: opt.id,
        name: opt.name,
        priceModifier: opt.priceModifier,
        position: opt.position,
      });
      this.optionsArray.push(fg);
    }
    this.customizationForm.patchValue({
      name: group.name,
      type: group.type,
      minSelections: group.minSelections,
      maxSelections: group.maxSelections,
      required: group.required,
      position: group.position,
    });
  }

  cancelEditGroup() {
    this.openAddGroup();
  }

  saveCustomizationGroup() {
    if (this.customizationForm.invalid) return;
    const product = this.editingProduct();
    if (!product) return;

    this.isSavingCustomization.set(true);
    const formVal = this.customizationForm.getRawValue();
    const dto = {
      id: this.editingCustomizationGroup()?.id,
      name: formVal.name,
      type: formVal.type,
      minSelections: formVal.minSelections,
      maxSelections: formVal.maxSelections,
      required: formVal.required,
      position: formVal.position,
      options: formVal.options.map((o: any) => ({
        id: o.id || undefined,
        name: o.name,
        priceModifier: o.priceModifier,
        position: o.position,
      })),
    };

    this.customizationApi.save(product.id, dto).subscribe({
      next: (saved) => {
        this.isSavingCustomization.set(false);
        this.openAddGroup();
        // Reload groups
        this.customizationApi.getByProduct(product.id).subscribe({
          next: (groups) => this.customizationGroups.set(groups),
        });
      },
      error: () => {
        this.isSavingCustomization.set(false);
      },
    });
  }

  deleteCustomizationGroup(id: string) {
    this.customizationGroupToDelete.set(id);
    this.showDeleteCustomizationConfirm.set(true);
  }

  confirmDeleteCustomizationGroup() {
    const id = this.customizationGroupToDelete();
    if (!id) return;
    this.customizationApi.delete(id).subscribe({
      next: () => {
        this.customizationGroups.update(groups => groups.filter(g => g.id !== id));
        this.showDeleteCustomizationConfirm.set(false);
        this.customizationGroupToDelete.set(null);
      },
    });
  }

  deleteProduct(id: string) {
    this.productToDelete.set(id);
    this.showDeleteProductConfirm.set(true);
  }

  confirmDeleteProduct() {
    const id = this.productToDelete();
    if (!id) return;
    this.productApi.delete(id).subscribe(() => {
      this.adminStore.deleteProduct(id);
      this.showDeleteProductConfirm.set(false);
      this.productToDelete.set(null);
    });
  }

  // --- Promoção ---
  openPromoModal(product: ProductResponse) {
    this.editingProduct.set(product);
    
    // Default dates
    const now = new Date();
    const tomorrow = new Date(now);
    tomorrow.setDate(tomorrow.getDate() + 1);

    this.promoForm.reset({
      price: product.price, // Sugere o preço atual
      initialTime: now.toISOString().slice(0, 16), // 'YYYY-MM-DDTHH:mm'
      finalTime: tomorrow.toISOString().slice(0, 16)
    });
    
    this.isPromoModalOpen.set(true);
  }

  closePromoModal() {
    this.isPromoModalOpen.set(false);
  }

  savePromo() {
    if (this.promoForm.invalid) return;
    const prod = this.editingProduct();
    if (!prod) return;

    this.isSaving.set(true);
    const req: PromotionPriceRequest = {
      productId: prod.id,
      price: this.promoForm.getRawValue().price,
      initialTime: new Date(this.promoForm.getRawValue().initialTime).toISOString(),
      finalTime: new Date(this.promoForm.getRawValue().finalTime).toISOString(),
    };

    this.productApi.createPromotion(req).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.closePromoModal();
        this.adminStore.loadAdminData(); // Reload to get updated product with promotion
      },
      error: () => this.isSaving.set(false)
    });
  }
}
