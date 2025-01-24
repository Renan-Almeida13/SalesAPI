export interface Sale {
  id?: number;
  saleDate: string;
  customer: string;
  branch: string;
  totalSaleAmount: number;
  products: { saleId: null, productName: string; quantity: number; unitPrice: number; totalAmount: number }[];
}

export interface SaleItem {
  saleId?: number;
  productName: string;
  quantity: number;
  unitPrice: number;
  totalAmount: number;
}
