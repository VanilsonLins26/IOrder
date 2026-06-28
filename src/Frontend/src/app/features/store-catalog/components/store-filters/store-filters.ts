import { ChangeDetectionStrategy, Component, ElementRef, ViewChild, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import type { StoreCategoryResponse } from '../../../../core/models/store-category.model';
import { LoadingSkeletonComponent } from '../../../../shared/components/loading-skeleton/loading-skeleton.component';

@Component({
  selector: 'app-store-filters',
  standalone: true,
  imports: [CommonModule, LoadingSkeletonComponent],
  templateUrl: './store-filters.html',
  styleUrl: './store-filters.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class StoreFiltersComponent {
  @ViewChild('scrollContainer') scrollContainer!: ElementRef<HTMLElement>;

  categories = input.required<StoreCategoryResponse[]>();
  loadingCategories = input<boolean>(false);
  
  selectedCategoryId = input<string | null>(null);
  searchQuery = input<string>('');

  categorySelect = output<string | null>();
  searchChange = output<string>();

  private isDragging = false;
  private startX = 0;
  private scrollLeft = 0;

  onSearch(event: Event) {
    const value = (event.target as HTMLInputElement).value;
    this.searchChange.emit(value);
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
    this.scrollContainer.nativeElement.classList.remove('categories__scroll--dragging');
  }

  onMouseUp() {
    this.isDragging = false;
    this.scrollContainer.nativeElement.classList.remove('categories__scroll--dragging');
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
