import { Directive, HostListener, Input } from '@angular/core';

type NumericMode = 'integer' | 'decimal';

@Directive({
  selector: '[tbNumericInput]',
  standalone: true,
})
export class NumericInputDirective {
  @Input('tbNumericInput') mode: NumericMode = 'integer';

  private readonly controlKeys = new Set([
    'Backspace',
    'Tab',
    'ArrowLeft',
    'ArrowRight',
    'Delete',
    'Home',
    'End',
  ]);

  @HostListener('keydown', ['$event'])
  handleKeyDown(event: KeyboardEvent): void {
    if (this.isModifierKey(event)) {
      return;
    }

    if (this.controlKeys.has(event.key)) {
      return;
    }

    if (this.isDigit(event.key)) {
      return;
    }

    if (
      this.mode === 'decimal' &&
      event.key === '.' &&
      this.canInsertDecimal(event.target as HTMLInputElement)
    ) {
      return;
    }

    event.preventDefault();
  }

  @HostListener('paste', ['$event'])
  handlePaste(event: ClipboardEvent): void {
    const input = event.target as HTMLInputElement;
    if (!input) {
      return;
    }

    const pasted = event.clipboardData?.getData('text') ?? '';
    const sanitized = this.sanitize(pasted);

    if (!sanitized.length) {
      event.preventDefault();
      return;
    }

    event.preventDefault();
    const selectionStart = input.selectionStart ?? input.value.length;
    const selectionEnd = input.selectionEnd ?? input.value.length;

    const newValue =
      input.value.substring(0, selectionStart) + sanitized + input.value.substring(selectionEnd);

    input.value = newValue;

    const caretPosition = selectionStart + sanitized.length;
    input.setSelectionRange(caretPosition, caretPosition);
    input.dispatchEvent(new Event('input', { bubbles: true }));
  }

  private sanitize(value: string): string {
    const regex = this.mode === 'decimal' ? /[^0-9.]/g : /[^0-9]/g;
    let sanitized = value.replace(regex, '');

    if (this.mode === 'decimal') {
      const parts = sanitized.split('.');
      sanitized = parts.shift() ?? '';
      if (parts.length) {
        sanitized += '.' + parts.join('');
      }
    }

    return sanitized;
  }

  private isDigit(key: string): boolean {
    return /^[0-9]$/.test(key);
  }

  private isModifierKey(event: KeyboardEvent): boolean {
    return event.ctrlKey || event.metaKey;
  }

  private canInsertDecimal(input: HTMLInputElement): boolean {
    const value = input.value ?? '';
    const hasDecimal = value.includes('.');

    if (!hasDecimal) {
      return true;
    }

    const selectionStart = input.selectionStart ?? value.length;
    const selectionEnd = input.selectionEnd ?? value.length;

    return value.substring(selectionStart, selectionEnd).includes('.');
  }
}
