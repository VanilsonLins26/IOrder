import { Component, ChangeDetectionStrategy } from '@angular/core';

@Component({
  selector: 'app-category-management',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `<div><h1>Gerenciar Categorias</h1><p class="text-secondary">Em breve...</p></div>`,
})
export class CategoryManagementComponent {}
