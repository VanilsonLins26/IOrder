import { ChangeDetectionStrategy, Component, OnInit, inject, effect } from '@angular/core';
import { CatalogStore } from '../../store/catalog.store';
import { AddressStore } from '../../../../core/stores/address.store';
import { StoreFiltersComponent } from '../../components/store-filters/store-filters';
import { StoreCardComponent } from '../../../../shared/components/store-card/store-card';
import { EmptyStateComponent } from '../../../../shared/components/empty-state/empty-state.component';
import { LoadingSkeletonComponent } from '../../../../shared/components/loading-skeleton/loading-skeleton.component';

@Component({
  selector: 'app-stores-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    StoreFiltersComponent,
    StoreCardComponent,
    EmptyStateComponent,
    LoadingSkeletonComponent
  ],
  templateUrl: './stores-list.component.html',
  styleUrl: './stores-list.component.scss',
})
export class StoresListComponent implements OnInit {
  readonly catalogStore = inject(CatalogStore);
  private readonly addressStore = inject(AddressStore);

  constructor() {
    effect(() => {
      const hasLocation = this.addressStore.hasLocation();
      if (hasLocation) {
        this.catalogStore.loadStores({ pageNumber: 1, pageSize: 20 });
      }
    });
  }

  ngOnInit() {
    this.catalogStore.loadCategories();
    this.addressStore.loadAddresses();
  }

  onCategorySelect(categoryId: string | null) {
    this.catalogStore.setCategory(categoryId);
  }

  onSearch(query: string) {
    this.catalogStore.setSearchQuery(query);
  }
}
