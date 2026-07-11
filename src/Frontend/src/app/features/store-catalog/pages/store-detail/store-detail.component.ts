import { ChangeDetectionStrategy, Component, inject, input, computed, signal } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { rxResource } from '@angular/core/rxjs-interop';
import { StoreApiService } from '../../../../core/services/api/store-api.service';
import { ProductApiService } from '../../../../core/services/api/product-api.service';
import { CategoryApiService } from '../../../../core/services/api/category-api.service';
import { UploadApiService } from '../../../../core/services/api/upload-api.service';
import { CustomizationApiService } from '../../../../core/services/api/customization-api.service';
import { StoreInfoHeaderComponent } from '../../components/store-info-header/store-info-header';
import { ProductGridComponent } from '../../components/product-grid/product-grid';
import { LoadingSkeletonComponent } from '../../../../shared/components/loading-skeleton/loading-skeleton.component';
import { ModalComponent } from '../../../../shared/components/modal/modal.component';
import { CartStore } from '../../../cart/store/cart.store';
import type { StoreResponse } from '../../../../core/models/store.model';
import type { ProductResponse } from '../../../../core/models/product.model';
import type { CustomizationGroup } from '../../../../core/models/customization.model';

@Component({
  selector: 'app-store-detail',
  standalone: true,
  imports: [CurrencyPipe, FormsModule, StoreInfoHeaderComponent, ProductGridComponent, LoadingSkeletonComponent, ModalComponent],
  templateUrl: './store-detail.component.html',
  styleUrl: './store-detail.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class StoreDetailComponent {
  // Recebe o ':id' da rota
  id = input.required<string>();

  private readonly storeApi = inject(StoreApiService);
  private readonly productApi = inject(ProductApiService);
  private readonly categoryApi = inject(CategoryApiService);
  private readonly uploadApi = inject(UploadApiService);
  private readonly customizationApi = inject(CustomizationApiService);
  private readonly cartStore = inject(CartStore);

  readonly storeResource = rxResource({
    stream: () => this.storeApi.getById(this.id()),
  });

  readonly productsResource = rxResource({
    stream: () => this.productApi.getPaged({ storeId: this.id(), pageNumber: 1, pageSize: 50 }),
  });

  readonly categoriesResource = rxResource({
    stream: () => this.categoryApi.getByStoreId(this.id()),
  });

  readonly store = computed(() => this.storeResource.value());
  readonly products = computed(() => this.productsResource.value()?.items ?? []);
  readonly categories = computed(() => this.categoriesResource.value() ?? []);
  readonly loadingStore = computed(() => this.storeResource.isLoading());
  readonly loadingProducts = computed(() => this.productsResource.isLoading());
  readonly loadingCategories = computed(() => this.categoriesResource.isLoading());

  readonly groupedProducts = computed(() => {
    const products = this.products();
    const categories = this.categories().sort((a, b) => a.position - b.position);
    
    const grouped = categories.map(cat => ({
      id: cat.id,
      name: cat.name,
      products: products.filter(p => p.categoryId === cat.id)
    })).filter(cat => cat.products.length > 0);

    const unassignedProducts = products.filter(p => !p.categoryId);
    if (unassignedProducts.length > 0) {
       grouped.push({ id: '', name: 'Outros', products: unassignedProducts });
    }
    
    return grouped;
  });

  readonly showStoreDialog = signal(false);
  readonly showCustomizeDialog = signal(false);
  readonly customizeText = signal('');
  readonly customizeImageUrls = signal<string[]>([]);
  readonly uploadingCustomizeImage = signal(false);
  readonly customizationGroups = signal<CustomizationGroup[]>([]);
  readonly selectedOptionIds = signal<Set<string>>(new Set());
  readonly loadingCustomization = signal(false);
  private pendingProduct: ProductResponse | null = null;

  onAddToCart(product: ProductResponse) {
    const currentItems = this.cartStore.items();
    const currentStoreId = this.id();

    if (currentItems.length > 0 && currentItems[0].storeId && currentItems[0].storeId !== currentStoreId) {
      this.pendingProduct = product;
      this.showStoreDialog.set(true);
      return;
    }

    this.pendingProduct = product;
    this.customizeText.set('');
    this.selectedOptionIds.set(new Set());
    this.customizationGroups.set([]);

    this.loadingCustomization.set(true);
    this.customizationApi.getByProduct(product.id).subscribe({
      next: (groups) => {
        this.customizationGroups.set(groups);
        this.loadingCustomization.set(false);
        if (groups.length > 0 || product.customizable) {
          this.showCustomizeDialog.set(true);
        } else {
          this.addItemToCart(product);
        }
      },
      error: () => {
        this.loadingCustomization.set(false);
        if (product.customizable) {
          this.showCustomizeDialog.set(true);
        } else {
          this.addItemToCart(product);
        }
      },
    });
  }

  toggleOption(optionId: string) {
    this.selectedOptionIds.update(ids => {
      const next = new Set(ids);
      if (next.has(optionId)) {
        next.delete(optionId);
      } else {
        next.add(optionId);
      }
      return next;
    });
  }

  toggleSingleOption(optionId: string, group: CustomizationGroup) {
    this.selectedOptionIds.update(ids => {
      const next = new Set(ids);
      for (const opt of group.options) {
        next.delete(opt.id);
      }
      next.add(optionId);
      return next;
    });
  }

  isGroupValid(group: CustomizationGroup): boolean {
    const count = group.options.filter(o => this.selectedOptionIds().has(o.id)).length;
    return count >= group.minSelections && count <= group.maxSelections;
  }

  allGroupsValid(): boolean {
    return this.customizationGroups().every(g => !g.required || this.isGroupValid(g));
  }

  onConfirmCustomize() {
    const product = this.pendingProduct;
    const optionIds = Array.from(this.selectedOptionIds());
    this.pendingProduct = null;
    this.showCustomizeDialog.set(false);
    this.customizationGroups.set([]);
    if (!product) return;
    this.addItemToCart(product, this.customizeText(), this.customizeImageUrls(), optionIds);
    this.customizeImageUrls.set([]);
  }

  onSkipCustomize() {
    const product = this.pendingProduct;
    this.pendingProduct = null;
    this.showCustomizeDialog.set(false);
    this.customizationGroups.set([]);
    if (!product) return;
    this.addItemToCart(product);
    this.customizeImageUrls.set([]);
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;
    this.uploadingCustomizeImage.set(true);
    this.uploadApi.uploadImage(file).subscribe({
      next: (res) => {
        this.customizeImageUrls.update(urls => [...urls, res.imageUrl]);
        this.uploadingCustomizeImage.set(false);
        input.value = '';
      },
      error: () => {
        this.uploadingCustomizeImage.set(false);
        input.value = '';
      },
    });
  }

  removeCustomizeImage(index: number) {
    this.customizeImageUrls.update(urls => urls.filter((_, i) => i !== index));
  }

  onConfirmClearAndAdd() {
    const product = this.pendingProduct;
    this.pendingProduct = null;
    this.showStoreDialog.set(false);

    if (!product) return;

    this.cartStore.clearCart();

    this.customizeText.set('');
    this.customizeImageUrls.set([]);
    this.selectedOptionIds.set(new Set());
    this.customizationGroups.set([]);

    this.loadingCustomization.set(true);
    this.customizationApi.getByProduct(product.id).subscribe({
      next: (groups) => {
        this.customizationGroups.set(groups);
        this.loadingCustomization.set(false);
        if (groups.length > 0 || product.customizable) {
          this.showCustomizeDialog.set(true);
        } else {
          this.addItemToCart(product);
        }
      },
      error: () => {
        this.loadingCustomization.set(false);
        if (product.customizable) {
          this.showCustomizeDialog.set(true);
        } else {
          this.addItemToCart(product);
        }
      },
    });
  }

  onCancelStoreDialog() {
    this.pendingProduct = null;
    this.showStoreDialog.set(false);
  }

  onCancelCustomize() {
    this.pendingProduct = null;
    this.showCustomizeDialog.set(false);
    this.customizeImageUrls.set([]);
    this.customizationGroups.set([]);
  }

  private addItemToCart(product: ProductResponse, customize?: string, imageUrls?: string[], selectedOptionIds?: string[]) {
    const urls = imageUrls?.length
      ? imageUrls
      : (product.imageUrl ? [product.imageUrl] : []);
    this.cartStore.addItem({
      productId: product.id,
      quantity: 1,
      customize,
      imageUrls: urls,
      selectedOptionIds,
    });
  }
}
