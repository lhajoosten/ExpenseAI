import { Directive, ElementRef, Input, OnInit, OnDestroy } from '@angular/core';

@Directive({
  selector: '[appAutoFocus]',
  standalone: true
})
export class AutoFocusDirective implements OnInit, OnDestroy {
  @Input() appAutoFocus: boolean | string = true;
  @Input() delay: number = 0;

  private timeoutId?: number;

  constructor(private el: ElementRef) {}

  ngOnInit() {
    if (this.shouldFocus()) {
      this.timeoutId = window.setTimeout(() => {
        this.el.nativeElement.focus();
      }, this.delay);
    }
  }

  ngOnDestroy() {
    if (this.timeoutId) {
      clearTimeout(this.timeoutId);
    }
  }

  private shouldFocus(): boolean {
    if (typeof this.appAutoFocus === 'string') {
      return this.appAutoFocus !== 'false';
    }
    return this.appAutoFocus;
  }
}
