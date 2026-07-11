import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import type { ProductResponse } from '../../../core/models/product.model';

@Component({
  selector: 'app-product-card',
  standalone: true,
  imports: [CurrencyPipe],
  templateUrl: './product-card.html',
  styleUrl: './product-card.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProductCardComponent {
  product = input.required<ProductResponse>();
  addToCart = output<ProductResponse>();

  get hasPromotion(): boolean {
    const promoPrice = this.product().currentPromotionalPrice;
    return promoPrice != null && promoPrice > 0;
  }
}
