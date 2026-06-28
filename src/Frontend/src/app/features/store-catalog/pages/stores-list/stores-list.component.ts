import { Component, ChangeDetectionStrategy } from '@angular/core';

@Component({
  selector: 'app-stores-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="page-container">
      <h1>Lojas</h1>
      <p class="text-secondary">Em breve...</p>
    </div>
  `,
  styles: [`.page-container { max-width: var(--container-xl); margin: 0 auto; padding: var(--space-8) var(--space-4); }`],
})
export class StoresListComponent {}
