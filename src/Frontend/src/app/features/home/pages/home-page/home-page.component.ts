import {
  Component,
  ChangeDetectionStrategy,
  inject,
  signal,
  computed,
  effect,
  ElementRef,
  ViewChild
} from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { StoreCategoryApiService } from '../../../../core/services/api/store-category-api.service';
import { StoreApiService } from '../../../../core/services/api/store-api.service';
import { AddressStore } from '../../../../core/stores/address.store';
import { AddressBarComponent } from '../../../../shared/components/address-bar/address-bar.component';
import { AddressDrawerComponent } from '../../../../shared/components/address-drawer/address-drawer.component';
import { LoadingSkeletonComponent } from '../../../../shared/components/loading-skeleton/loading-skeleton.component';
import { EmptyStateComponent } from '../../../../shared/components/empty-state/empty-state.component';
import { StoreCardComponent } from '../../../../shared/components/store-card/store-card';
import type { StoreCategoryResponse } from '../../../../core/models/store-category.model';
import type { StoreResponse } from '../../../../core/models/store.model';
import type { PagedList } from '../../../../core/models';

@Component({
  selector: 'app-home-page',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    RouterLink,
    LoadingSkeletonComponent,
    EmptyStateComponent,
    StoreCardComponent,
    AddressBarComponent,
    AddressDrawerComponent,
  ],
  templateUrl: './home-page.component.html',
  styleUrl: './home-page.component.scss',
})
export class HomePageComponent {
  private readonly storeCategoryApi = inject(StoreCategoryApiService);
  private readonly storeApi         = inject(StoreApiService);
  readonly addressStore             = inject(AddressStore);

  readonly drawerOpen = signal(false);
  readonly selectedCatId = signal<string | null>(null);
  readonly searchQuery   = signal('');

  private readonly categoriesResource = rxResource<StoreCategoryResponse[], void>({
    stream: () => this.storeCategoryApi.getAll(),
  });

  private readonly storesResource = rxResource<PagedList<StoreResponse>, string | null>({
    params: () => this.selectedCatId(),
    stream: (req) => {
      const lat = this.addressStore.latitude();
      const lon = this.addressStore.longitude();
      return this.storeApi.getPaged({
        pageNumber: 1,
        pageSize: 12,
        categoryId: req.params ?? undefined,
        ...(lat !== null && lon !== null ? { userLatitude: lat, userLongitude: lon } : {}),
      });
    },
  });

  readonly categories    = computed(() => this.categoriesResource.value() ?? []);
  readonly stores        = computed(() => this.storesResource.value()?.items ?? []);
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
    const query = this.searchQuery().toLowerCase().trim();
    if (!query) return this.stores();
    return this.stores().filter((s: StoreResponse) =>
      s.name.toLowerCase().includes(query)
    );
  });

  @ViewChild('scrollContainer') scrollContainer!: ElementRef<HTMLElement>;
  private isDragging = false;
  private startX = 0;
  private scrollLeft = 0;

  constructor() {
    this.addressStore.loadAddresses();

    effect(() => {
      const lat = this.addressStore.latitude();
      const lon = this.addressStore.longitude();
      if (lat !== null && lon !== null) {
        this.storesResource.reload();
      }
    });
  }

  selectCategory(id: string | null): void { this.selectedCatId.set(id); }

  onSearch(event: Event): void {
    this.searchQuery.set((event.target as HTMLInputElement).value);
  }

  openDrawer(): void { this.drawerOpen.set(true); }
  closeDrawer(): void { this.drawerOpen.set(false); }

  onMouseDown(e: MouseEvent) {
    this.isDragging = true;
    const el = this.scrollContainer.nativeElement;
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
    el.classList.add('categories__scroll--dragging');
    const x = e.pageX - el.offsetLeft;
    const walk = (x - this.startX) * 2;
    el.scrollLeft = this.scrollLeft - walk;
  }
}
