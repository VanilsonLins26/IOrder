import { Directive, ElementRef, HostListener, forwardRef, inject } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Directive({
  selector: '[appCurrencyInput]',
  standalone: true,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CurrencyInputDirective),
      multi: true
    }
  ]
})
export class CurrencyInputDirective implements ControlValueAccessor {
  private readonly el = inject(ElementRef<HTMLInputElement>);
  
  private onChange: (val: number | null) => void = () => {};
  private onTouched: () => void = () => {};

  @HostListener('focus')
  onFocus() {
    this.el.nativeElement.select();
  }

  @HostListener('click')
  onClick() {
    this.el.nativeElement.select();
  }

  @HostListener('input')
  onInput() {
    const value = this.el.nativeElement.value;
    const rawValue = value.replace(/\D/g, '');
    if (!rawValue) {
      this.onChange(null);
      this.el.nativeElement.value = '';
      return;
    }

    const numberValue = parseInt(rawValue, 10) / 100;
    this.el.nativeElement.value = this.formatCurrency(numberValue);
    this.onChange(numberValue);
  }

  @HostListener('blur')
  onBlur() {
    this.onTouched();
  }

  writeValue(value: number | null | undefined): void {
    if (value == null || isNaN(value)) {
      this.el.nativeElement.value = '';
    } else {
      this.el.nativeElement.value = this.formatCurrency(value);
    }
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState?(isDisabled: boolean): void {
    this.el.nativeElement.disabled = isDisabled;
  }

  private formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
    }).format(value);
  }
}
