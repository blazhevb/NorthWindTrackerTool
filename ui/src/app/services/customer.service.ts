import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CustomerSummary, CustomerDetail } from '../models/customer.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class CustomerService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiBaseUrl;

  getAll(nameFilter?: string): Observable<CustomerSummary[]> {
    const url = nameFilter
      ? `${this.baseUrl}/customers?name=${encodeURIComponent(nameFilter)}`
      : `${this.baseUrl}/customers`;
    return this.http.get<CustomerSummary[]>(url);
  }

  getById(customerId: string): Observable<CustomerDetail> {
    return this.http.get<CustomerDetail>(`${this.baseUrl}/customers/${customerId}`);
  }
}
