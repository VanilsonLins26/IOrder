import { ChangeDetectionStrategy, Component, inject, input, computed, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { rxResource } from '@angular/core/rxjs-interop';
import { StoreApiService } from '../../../../core/services/api/store-api.service';
import { ProductApiService } from '../../../../core/services/api/product-api.service';
import { CategoryApiService } from '../../../../core/services/api/category-api.service';
import { StoreInfoHeaderComponent } from '../../components/store-info-header/store-info-header';
import { ProductGridComponent } from '../../components/product-grid/product-grid';
import { LoadingSkeletonComponent } from '../../../../shared/components/loading-skeleton/loading-skeleton.component';
import { ModalComponent } from '../../../../shared/components/modal/modal.component';
import { CartStore } from '../../../cart/store/cart.store';
import type { StoreResponse } from '../../../../core/models/store.model';
import type { ProductResponse } from '../../../../core/models/product.model';

@Component({
  selector: 'app-store-detail',
  standalone: true,
  imports: [CommonModule, StoreInfoHeaderComponent, ProductGridComponent, LoadingSkeletonComponent, ModalComponent],
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
  private pendingProduct: ProductResponse | null = null;

  onAddToCart(product: ProductResponse) {
    const currentItems = this.cartStore.items();
    const currentStoreId = this.id();

    if (currentItems.length > 0 && currentItems[0].storeId && currentItems[0].storeId !== currentStoreId) {
      this.pendingProduct = product;
      this.showStoreDialog.set(true);
      return;
    }

    this.addItemToCart(product);
  }

  onConfirmClearAndAdd() {
    const product = this.pendingProduct;
    this.pendingProduct = null;
    this.showStoreDialog.set(false);

    if (!product) return;

    this.cartStore.clearCart();
    this.addItemToCart(product);
  }

  onCancelStoreDialog() {
    this.pendingProduct = null;
    this.showStoreDialog.set(false);
  }

  private addItemToCart(product: ProductResponse) {
    this.cartStore.addItem({
      productId: product.id,
      quantity: 1,
      imageUrls: product.imageUrl ? [product.imageUrl] : [],
    });
  }
}
