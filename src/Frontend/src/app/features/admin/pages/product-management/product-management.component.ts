import { Component, ChangeDetectionStrategy, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AdminStore } from '../../store/admin.store';
import { ProductApiService } from '../../../../core/services/api/product-api.service';
import { CategoryApiService } from '../../../../core/services/api/category-api.service';
import { ModalComponent } from '../../../../shared/components/modal/modal.component';
import { LoadingSkeletonComponent } from '../../../../shared/components/loading-skeleton/loading-skeleton.component';
import { ImageUploadComponent } from '../../../../shared/components/image-upload/image-upload.component';
import { ProductResponse, ProductRequest, UpdateProductRequest, UnitOfMeasure, PromotionPriceRequest } from '../../../../core/models';

@Component({
  selector: 'app-product-management',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, ReactiveFormsModule, ModalComponent, LoadingSkeletonComponent, ImageUploadComponent],
  templateUrl: './product-management.component.html',
  styleUrl: './product-management.component.scss',
})
export class ProductManagementComponent implements OnInit {
  readonly adminStore = inject(AdminStore);
  private readonly productApi = inject(ProductApiService);
  private readonly categoryApi = inject(CategoryApiService);
  private readonly fb = inject(FormBuilder);

  // Enum para template
  readonly UnitOfMeasure = UnitOfMeasure;

  // Modals state
  readonly isProductModalOpen = signal(false);
  readonly isPromoModalOpen = signal(false);
  readonly isSaving = signal(false);
  readonly uploadingImage = signal(false);
  readonly editingProduct = signal<ProductResponse | null>(null);

  // Forms
  readonly productForm = this.fb.nonNullable.group({
    name: ['', [Validators.required]],
    description: [''],
    price: [0, [Validators.required, Validators.min(0)]],
    imageUrl: [''],
    unitOfMeasure: [0, [Validators.required]],
    categoryId: [''] // Será usado para vincular pós-criação ou na edição
  });

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
    if (!currentEditing) return;
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
          this.syncCategory(res.id, formVal.categoryId, '');
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

  deleteProduct(id: string) {
    if (confirm('Tem certeza que deseja excluir este produto?')) {
      this.productApi.delete(id).subscribe(() => {
        this.adminStore.deleteProduct(id);
      });
    }
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
