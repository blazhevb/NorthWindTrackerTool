export interface CustomerSummary {
  customerId: string;
  companyName: string;
  orderCount: number;
}

export interface OrderSummary {
  orderId: number;
  orderDate: string | null;
  totalValue: number;
  productCount: number;
}

export interface CustomerDetail {
  customerId: string;
  companyName: string;
  contactName: string | null;
  city: string | null;
  country: string | null;
  orders: OrderSummary[];
}
