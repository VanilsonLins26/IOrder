import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';
import { roleGuard, Roles } from './core/auth/role.guard';
import { roleRedirectGuard } from './core/auth/role-redirect.guard';
import { storeGuard } from './core/guards/store.guard';

export const routes: Routes = [

  // Public routes — with Navbar + Footer
  {
    path: '',
    loadComponent: () =>
      import('./layouts/client-layout/client-layout.component').then(
        (m) => m.ClientLayoutComponent,
      ),
    children: [
      {
        path: '',
        canActivate: [roleRedirectGuard],
        loadComponent: () =>
          import('./features/home/pages/home-page/home-page.component').then(
            (m) => m.HomePageComponent,
          ),
      },
      {
        path: 'stores',
        loadComponent: () =>
          import('./features/store-catalog/pages/stores-list/stores-list.component').then(
            (m) => m.StoresListComponent,
          ),
      },
      {
        path: 'stores/:id',
        loadComponent: () =>
          import('./features/store-catalog/pages/store-detail/store-detail.component').then(
            (m) => m.StoreDetailComponent,
          ),
      },
      {
        path: 'cart',
        canActivate: [authGuard],
        loadComponent: () =>
          import('./features/cart/pages/cart-page/cart-page.component').then(
            (m) => m.CartPageComponent,
          ),
      },
      {
        path: 'orders',
        canActivate: [authGuard],
        loadComponent: () =>
          import('./features/orders/pages/my-orders/my-orders.component').then(
            (m) => m.MyOrdersComponent,
          ),
      },
      {
        path: 'orders/:id',
        canActivate: [authGuard],
        loadComponent: () =>
          import('./features/orders/pages/order-detail/order-detail.component').then(
            (m) => m.OrderDetailComponent,
          ),
      },
      {
        path: 'profile',
        loadComponent: () =>
          import('./features/profile/pages/profile-page/profile-page.component').then(
            (m) => m.ProfilePageComponent,
          ),
      },
      {
        path: 'profile/addresses',
        canActivate: [authGuard],
        loadComponent: () =>
          import('./features/profile/pages/addresses-page/addresses-page.component').then(
            (m) => m.AddressesPageComponent,
          ),
      },
    ],
  },

  {
    path: 'admin',
    canActivate: [authGuard, roleGuard(Roles.ShopKeeper)],
    children: [
      {
        path: 'setup-store',
        loadComponent: () => import('./features/admin/pages/setup-store/setup-store.component').then(m => m.SetupStoreComponent)
      },
      {
        path: '',
        loadComponent: () => import('./layouts/admin-layout/admin-layout.component').then(m => m.AdminLayoutComponent),
        canActivate: [storeGuard],
        children: [
          { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
          {
            path: 'dashboard',
            loadComponent: () =>
              import('./features/admin/pages/dashboard/dashboard.component').then(
                (m) => m.DashboardComponent,
              ),
          },
      {
        path: 'products',
        loadComponent: () =>
          import('./features/admin/pages/product-management/product-management.component').then(
            (m) => m.ProductManagementComponent,
          ),
      },
      {
        path: 'categories',
        loadComponent: () =>
          import('./features/admin/pages/category-management/category-management.component').then(
            (m) => m.CategoryManagementComponent,
          ),
      },
      {
        path: 'coupons',
        loadComponent: () =>
          import('./features/admin/pages/coupon-management/coupon-management.component').then(
            (m) => m.CouponManagementComponent,
          ),
      },
          {
            path: 'orders',
            loadComponent: () =>
              import('./features/admin/pages/store-orders/store-orders.component').then(
                (m) => m.StoreOrdersComponent,
              ),
          },
          {
            path: 'orders/:id',
            loadComponent: () =>
              import('./features/admin/pages/store-order-detail/store-order-detail.component').then(
                (m) => m.StoreOrderDetailComponent,
              ),
          },
          {
            path: 'store',
            loadComponent: () =>
              import('./features/admin/pages/store-settings/store-settings.component').then(
                (m) => m.StoreSettingsComponent,
              ),
          },
        ],
      }
    ]
  },

  // Courier routes — Delivery role
  {
    path: 'courier',
    canActivate: [authGuard, roleGuard(Roles.Delivery)],
    loadComponent: () =>
      import('./layouts/courier-layout/courier-layout.component').then(
        (m) => m.CourierLayoutComponent,
      ),
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/courier/pages/courier-dashboard/courier-dashboard.component').then(
            (m) => m.CourierDashboardComponent,
          ),
      },
      {
        path: 'deliveries',
        loadComponent: () =>
          import('./features/courier/pages/my-deliveries/my-deliveries.component').then(
            (m) => m.MyDeliveriesComponent,
          ),
      },
      {
        path: 'deliveries/:id',
        loadComponent: () =>
          import('./features/courier/pages/delivery-detail/delivery-detail.component').then(
            (m) => m.DeliveryDetailComponent,
          ),
      },
    ],
  },

  // Fallback
  { path: '**', redirectTo: '' },
];
