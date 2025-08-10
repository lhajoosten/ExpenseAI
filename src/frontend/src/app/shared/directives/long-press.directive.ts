import { Directive, ElementRef, HostListener, Input, Renderer2 } from '@angular/core';

@Directive({
  selector: '[appLongPress]',
  standalone: true
})
export class LongPressDirective {
  @Input() appLongPress: number = 500; // milliseconds

  private isPressed = false;
  private timeoutId?: number;

  constructor(
    private el: ElementRef,
    private renderer: Renderer2
  ) {}

  @HostListener('mousedown', ['$event'])
  @HostListener('touchstart', ['$event'])
  onPressStart(event: Event) {
    this.isPressed = true;
    this.timeoutId = window.setTimeout(() => {
      if (this.isPressed) {
        this.onLongPress();
      }
    }, this.appLongPress);
  }

  @HostListener('mouseup', ['$event'])
  @HostListener('mouseleave', ['$event'])
  @HostListener('touchend', ['$event'])
  @HostListener('touchcancel', ['$event'])
  onPressEnd(event: Event) {
    this.isPressed = false;
    if (this.timeoutId) {
      clearTimeout(this.timeoutId);
    }
  }

  private onLongPress() {
    // Dispatch a custom event
    const longPressEvent = new CustomEvent('longpress', {
      bubbles: true,
      detail: { target: this.el.nativeElement }
    });
    this.el.nativeElement.dispatchEvent(longPressEvent);
  }
}
