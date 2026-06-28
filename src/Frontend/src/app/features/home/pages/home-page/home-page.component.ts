import {
  Component,
  ChangeDetectionStrategy,
  inject,
  signal,
  computed,
  ElementRef,
  ViewChild
} from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { StoreCategoryApiService } from '../../../../core/services/api/store-category-api.service';
import { StoreApiService } from '../../../../core/services/api/store-api.service';
import { LoadingSkeletonComponent } from '../../../../shared/components/loading-skeleton/loading-skeleton.component';
import { EmptyStateComponent } from '../../../../shared/components/empty-state/empty-state.component';
import { StoreCardComponent } from '../../../../shared/components/store-card/store-card';
import type { StoreCategoryResponse } from '../../../../core/models/store-category.model';
import type { StoreResponse } from '../../../../core/models/store.model';

@Component({
  selector: 'app-home-page',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink, LoadingSkeletonComponent, EmptyStateComponent, StoreCardComponent],
  templateUrl: './home-page.component.html',
  styleUrl: './home-page.component.scss',
})
export class HomePageComponent {
  private readonly storeCategoryApi = inject(StoreCategoryApiService);
  private readonly storeApi         = inject(StoreApiService);

  // ---- Filters (user-driven signals) ----
  readonly selectedCatId = signal<string | null>(null);
  readonly searchQuery   = signal('');

  // ---- Data via rxResource — correct Angular 20 zoneless pattern ----
  // rxResource uses `stream` (Observable-based) and bridges RxJS into the signal graph,
  // so templates re-render automatically when data arrives, no Zone.js needed.

  private readonly categoriesResource = rxResource<StoreCategoryResponse[], void>({
    stream: () => this.storeCategoryApi.getAll(),
  });

  private readonly storesResource = rxResource<StoreResponse[], void>({
    stream: () => this.storeApi.getPaged({ pageNumber: 1, pageSize: 12 }),
  });

  // ---- Derived signals ----
  readonly categories    = computed(() => this.categoriesResource.value() ?? []);
  readonly stores        = computed(() => this.storesResource.value()    ?? []);
  readonly loadingCats   = computed(() => this.categoriesResource.isLoading());
  readonly loadingStores = computed(() => this.storesResource.isLoading());

  readonly selectedCategoryName = computed(() => {
    const catId = this.selectedCatId();
    if (!catId) return 'Lojas em destaque';
    return (
      this.categories().find((c: StoreCategoryResponse) => c.id === catId)?.name ??
      'Lojas em destaque'
    );
  });

  readonly filteredStores = computed(() => {
    const catId = this.selectedCatId();
    const query = this.searchQuery().toLowerCase().trim();

    return this.stores().filter((s: StoreResponse) => {
      const matchesCat   = !catId || s.categoryId === catId;
      const matchesQuery = !query || s.name.toLowerCase().includes(query);
      return matchesCat && matchesQuery;
    });
  });

  // ---- Drag to Scroll State ----
  @ViewChild('scrollContainer') scrollContainer!: ElementRef<HTMLElement>;
  private isDragging = false;
  private startX = 0;
  private scrollLeft = 0;

  // ---- User actions ----
  selectCategory(id: string | null): void { this.selectedCatId.set(id); }

  onSearch(event: Event): void {
    this.searchQuery.set((event.target as HTMLInputElement).value);
  }

  onMouseDown(e: MouseEvent) {
    this.isDragging = true;
    const el = this.scrollContainer.nativeElement;
    el.classList.add('categories__scroll--dragging');
    this.startX = e.pageX - el.offsetLeft;
    this.scrollLeft = el.scrollLeft;
  }

  onMouseLeave() {
    this.isDragging = false;
    this.scrollContainer?.nativeElement.classList.remove('categories__scroll--dragging');
  }

  onMouseUp() {
    this.isDragging = false;
    this.scrollContainer?.nativeElement.classList.remove('categories__scroll--dragging');
  }

  onMouseMove(e: MouseEvent) {
    if (!this.isDragging) return;
    e.preventDefault();
    const el = this.scrollContainer.nativeElement;
    const x = e.pageX - el.offsetLeft;
    const walk = (x - this.startX) * 2; // Scroll-fast
    el.scrollLeft = this.scrollLeft - walk;
  }
}
