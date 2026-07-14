import { ChangeDetectionStrategy, Component, input, output, signal, effect, untracked } from '@angular/core';

@Component({
  selector: 'app-image-upload',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './image-upload.component.html',
  styleUrl: './image-upload.component.scss',
})
export class ImageUploadComponent {
  readonly currentUrl = input<string | null | undefined>(null);
  readonly loading = input(false);

  readonly fileSelected = output<File>();

  readonly previewUrl = signal<string | null>(null);

  constructor() {
    effect(() => {
      this.currentUrl(); // track changes to currentUrl
      untracked(() => {
        this.previewUrl.set(null);
      });
    });
  }

  onFilePicked(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    const reader = new FileReader();
    reader.onload = () => {
      this.previewUrl.set(reader.result as string);
    };
    reader.readAsDataURL(file);

    this.fileSelected.emit(file);
  }

  removeFile() {
    this.previewUrl.set(null);
  }
}
