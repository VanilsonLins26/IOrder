import { Component, ChangeDetectionStrategy } from '@angular/core';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `<div><h1>Dashboard</h1><p class="text-secondary">Em breve...</p></div>`,
})
export class DashboardComponent {}
