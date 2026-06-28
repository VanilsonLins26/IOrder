import { Injectable, signal, effect, PLATFORM_ID, inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

export type Theme = 'light' | 'dark';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly platformId = inject(PLATFORM_ID);
  private readonly _theme = signal<Theme>(this.getInitialTheme());

  readonly theme = this._theme.asReadonly();
  readonly isDark = () => this._theme() === 'dark';

  constructor() {
    effect(() => {
      const theme = this._theme();
      if (isPlatformBrowser(this.platformId)) {
        document.documentElement.setAttribute('data-theme', theme);
        localStorage.setItem('iorder-theme', theme);
      }
    });
  }

  toggle(): void {
    this._theme.update((current) => (current === 'light' ? 'dark' : 'light'));
  }

  setTheme(theme: Theme): void {
    this._theme.set(theme);
  }

  private getInitialTheme(): Theme {
    if (!isPlatformBrowser(this.platformId)) {
      return 'light';
    }

    const stored = localStorage.getItem('iorder-theme') as Theme | null;
    if (stored) {
      return stored;
    }

    const prefersDark = window.matchMedia(
      '(prefers-color-scheme: dark)',
    ).matches;
    return prefersDark ? 'dark' : 'light';
  }
}
