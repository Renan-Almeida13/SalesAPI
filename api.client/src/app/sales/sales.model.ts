export interface Sale {
  id: number;
  saleDate: string;
  customer: string;
  branch: string;
  products: { productName: string; quantity: number; unitPrice: number; totalAmount: number }[];
}

export interface SaleItem {
  productName: string;
  quantity: number;
  unitPrice: number;
  totalAmount: number;
}
