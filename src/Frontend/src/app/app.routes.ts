import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';
import { roleGuard, Roles } from './core/auth/role.guard';
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
        loadComponent: () =>
          import('./features/cart/pages/cart-page/cart-page.component').then(
            (m) => m.CartPageComponent,
          ),
      },
      {
        path: 'orders',
        loadComponent: () =>
          import('./features/orders/pages/my-orders/my-orders.component').then(
            (m) => m.MyOrdersComponent,
          ),
      },
      {
        path: 'orders/:id',
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

  // Fallback
  { path: '**', redirectTo: '' },
];
