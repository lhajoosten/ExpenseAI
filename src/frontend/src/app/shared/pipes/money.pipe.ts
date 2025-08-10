import { Pipe, PipeTransform } from '@angular/core';
import { Money } from '../models/expense.models';

@Pipe({
  name: 'money',
  standalone: true
})
export class MoneyPipe implements PipeTransform {
  transform(value: Money | null | undefined, showCurrency = true): string {
    if (!value || value.amount === null || value.amount === undefined) {
      return showCurrency ? '$0.00' : '0.00';
    }

    const formatter = new Intl.NumberFormat('en-US', {
      style: showCurrency ? 'currency' : 'decimal',
      currency: showCurrency ? value.currency : undefined,
      minimumFractionDigits: 2,
      maximumFractionDigits: 2
    });

    return formatter.format(value.amount);
  }
}
