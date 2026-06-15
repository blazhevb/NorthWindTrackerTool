import { Component, inject, signal, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CustomerService } from '../../services/customer.service';
import { CustomerSummary } from '../../models/customer.model';

@Component({
  selector: 'app-customer-list',
  imports: [FormsModule],
  templateUrl: './customer-list.component.html',
  styleUrl: './customer-list.component.css'
})
export class CustomerListComponent implements OnInit {
  private readonly customerService = inject(CustomerService);
  private readonly router = inject(Router);

  customers = signal<CustomerSummary[]>([]);
  nameFilter = signal('');
  loading = signal(false);
  error = signal<string | null>(null);

  ngOnInit(): void {
    this.loadCustomers();
  }

  loadCustomers(): void {
    this.loading.set(true);
    this.error.set(null);

    this.customerService.getAll(this.nameFilter() || undefined).subscribe({
      next: customers => {
        this.customers.set(customers);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load customers. Please try again.');
        this.loading.set(false);
      }
    });
  }

  viewDetail(customerId: string): void {
    this.router.navigate(['/customers', customerId]);
  }
}
