import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';
import { CatalogStore } from '../../store/catalog.store';
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

  ngOnInit() {
    this.catalogStore.loadCategories();
    this.catalogStore.loadStores({ pageNumber: 1, pageSize: 20 });
  }

  onCategorySelect(categoryId: string | null) {
    this.catalogStore.setCategory(categoryId);
  }

  onSearch(query: string) {
    this.catalogStore.setSearchQuery(query);
  }
}
