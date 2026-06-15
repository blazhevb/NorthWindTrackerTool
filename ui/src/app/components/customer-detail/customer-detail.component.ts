import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { CustomerService } from '../../services/customer.service';
import { CustomerDetail } from '../../models/customer.model';

@Component({
  selector: 'app-customer-detail',
  imports: [CurrencyPipe, DatePipe],
  templateUrl: './customer-detail.component.html',
  styleUrl: './customer-detail.component.css'
})
export class CustomerDetailComponent implements OnInit {
  private readonly customerService = inject(CustomerService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  customer = signal<CustomerDetail | null>(null);
  loading = signal(false);
  error = signal<string | null>(null);
  location = computed(() => [this.customer()?.city, this.customer()?.country].filter(Boolean).join(', '));

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.loading.set(true);

    this.customerService.getById(id).subscribe({
      next: customer => {
        this.customer.set(customer);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Customer not found or failed to load.');
        this.loading.set(false);
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/customers']);
  }
}
