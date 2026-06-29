import { Component, ChangeDetectionStrategy, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CdkDragDrop, DragDropModule, moveItemInArray } from '@angular/cdk/drag-drop';
import { AdminStore } from '../../store/admin.store';
import { CategoryApiService } from '../../../../core/services/api/category-api.service';
import { ModalComponent } from '../../../../shared/components/modal/modal.component';
import { LoadingSkeletonComponent } from '../../../../shared/components/loading-skeleton/loading-skeleton.component';
import type { CategoryResponse, CategoryRequest, UpdateCategoryPositionsRequest } from '../../../../core/models';

@Component({
  selector: 'app-category-management',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, ReactiveFormsModule, DragDropModule, ModalComponent, LoadingSkeletonComponent],
  templateUrl: './category-management.component.html',
  styleUrl: './category-management.component.scss',
})
export class CategoryManagementComponent implements OnInit {
  readonly adminStore = inject(AdminStore);
  private readonly categoryApi = inject(CategoryApiService);
  private readonly fb = inject(FormBuilder);

  // Modal states
  readonly isModalOpen = signal(false);
  readonly isSaving = signal(false);
  readonly editingCategoryId = signal<string | null>(null);

  // Form
  readonly categoryForm = this.fb.nonNullable.group({
    name: ['', [Validators.required]],
    imageUrl: [''],
  });

  ngOnInit() {
    if (!this.adminStore.myStore() && !this.adminStore.loading()) {
      this.adminStore.loadAdminData();
    }
  }

  // --- Drag and Drop ---
  drop(event: CdkDragDrop<CategoryResponse[]>) {
    // We create a mutable copy to move items
    const categories = [...this.adminStore.categories()];
    moveItemInArray(categories, event.previousIndex, event.currentIndex);

    // Update the position properties and update local state
    categories.forEach((cat, idx) => {
      cat.position = idx;
    });
    this.adminStore.setCategories(categories);

    // Persist to backend
    const request: UpdateCategoryPositionsRequest = {
      positions: categories.map(cat => ({
        categoryId: cat.id,
        position: cat.position
      }))
    };
    
    this.categoryApi.updatePositions(request).subscribe({
      error: () => {
        // If it fails, reload to revert
        this.adminStore.loadAdminData();
      }
    });
  }

  // --- CRUD Modals ---
  openCreateModal() {
    this.editingCategoryId.set(null);
    this.categoryForm.reset({ name: '', imageUrl: '' });
    this.isModalOpen.set(true);
  }

  openEditModal(category: CategoryResponse) {
    this.editingCategoryId.set(category.id);
    this.categoryForm.patchValue({
      name: category.name,
      imageUrl: category.imageUrl
    });
    this.isModalOpen.set(true);
  }

  closeModal() {
    this.isModalOpen.set(false);
  }

  saveCategory() {
    if (this.categoryForm.invalid) return;

    this.isSaving.set(true);
    const formValue = this.categoryForm.getRawValue();
    const isEdit = this.editingCategoryId() !== null;

    if (isEdit) {
      // Update
      const req: CategoryRequest = {
        ...formValue,
        position: this.adminStore.categories().find(c => c.id === this.editingCategoryId())?.position || 0
      };

      this.categoryApi.update(this.editingCategoryId()!, req).subscribe({
        next: (res) => {
          this.adminStore.updateCategory(res);
          this.closeModal();
          this.isSaving.set(false);
        },
        error: () => this.isSaving.set(false)
      });
    } else {
      // Create
      const newPosition = this.adminStore.categories().length;
      const req: CategoryRequest = {
        ...formValue,
        position: newPosition
      };

      this.categoryApi.create(req).subscribe({
        next: (res) => {
          this.adminStore.addCategory(res);
          this.closeModal();
          this.isSaving.set(false);
        },
        error: () => this.isSaving.set(false)
      });
    }
  }

  deleteCategory(id: string) {
    if (confirm('Tem certeza que deseja excluir esta categoria? Produtos associados a ela poderão ficar sem categoria!')) {
      this.categoryApi.delete(id).subscribe({
        next: () => {
          this.adminStore.deleteCategory(id);
        }
      });
    }
  }
}
