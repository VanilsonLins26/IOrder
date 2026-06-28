import { Component, ChangeDetectionStrategy } from '@angular/core';

@Component({
  selector: 'app-store-settings',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `<div><h1>Configurações da Loja</h1><p class="text-secondary">Em breve...</p></div>`,
})
export class StoreSettingsComponent {}
