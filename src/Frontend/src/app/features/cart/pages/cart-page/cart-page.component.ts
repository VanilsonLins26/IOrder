import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CartStore } from '../../store/cart.store';
import { EmptyStateComponent } from '../../../../shared/components/empty-state/empty-state.component';
import { LoadingSkeletonComponent } from '../../../../shared/components/loading-skeleton/loading-skeleton.component';

@Component({
  selector: 'app-cart-page',
  standalone: true,
  imports: [CommonModule, FormsModule, EmptyStateComponent, LoadingSkeletonComponent],
  templateUrl: './cart-page.component.html',
  styleUrl: './cart-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CartPageComponent implements OnInit {
  readonly cartStore = inject(CartStore);
  private readonly router = inject(Router);

  readonly couponInput = signal('');

  ngOnInit() {
    this.cartStore.loadCart();
  }

  removeItem(itemId: string) {
    this.cartStore.removeItem(itemId);
  }

  changeQuantity(itemId: string, quantity: number) {
    if (quantity < 1) return;
    this.cartStore.changeQuantity({ cartItemId: itemId, newQuantity: quantity });
  }

  applyCoupon() {
    const code = this.couponInput().trim();
    if (!code) return;
    this.cartStore.applyCoupon(code);
    this.couponInput.set('');
  }

  clearCart() {
    this.cartStore.clearCart();
  }

  goToStores() {
    this.router.navigate(['/stores']);
  }

  protected readonly trackByItemId = (_: number, item: { id: string }) => item.id;
}
