export interface Sale {
  id: number;
  saleNumber: string;
  date: Date; // Use 'string' se estiver usando formato 'yyyy-MM-dd' no frontend
  customer: string;
  totalAmount: number;
  branch: string;
}
