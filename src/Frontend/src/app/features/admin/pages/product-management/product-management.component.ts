import { Component, ChangeDetectionStrategy } from '@angular/core';

@Component({
  selector: 'app-product-management',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `<div><h1>Gerenciar Produtos</h1><p class="text-secondary">Em breve...</p></div>`,
})
export class ProductManagementComponent {}
