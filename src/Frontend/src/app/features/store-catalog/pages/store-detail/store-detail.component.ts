import { ChangeDetectionStrategy, Component, inject, input, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { rxResource } from '@angular/core/rxjs-interop';
import { StoreApiService } from '../../../../core/services/api/store-api.service';
import { ProductApiService } from '../../../../core/services/api/product-api.service';
import { CategoryApiService } from '../../../../core/services/api/category-api.service';
import { StoreInfoHeaderComponent } from '../../components/store-info-header/store-info-header';
import { ProductGridComponent } from '../../components/product-grid/product-grid';
import { LoadingSkeletonComponent } from '../../../../shared/components/loading-skeleton/loading-skeleton.component';
import { ToastService } from '../../../../core/services/toast.service';
import type { StoreResponse } from '../../../../core/models/store.model';
import type { ProductResponse } from '../../../../core/models/product.model';

@Component({
  selector: 'app-store-detail',
  standalone: true,
  imports: [CommonModule, StoreInfoHeaderComponent, ProductGridComponent, LoadingSkeletonComponent],
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
  private readonly toast = inject(ToastService);

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
  readonly products = computed(() => this.productsResource.value() ?? []);
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

  onAddToCart(product: ProductResponse) {
    this.toast.success(`Adicionado: ${product.name}`);
  }
}
