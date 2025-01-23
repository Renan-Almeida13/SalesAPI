import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Sale } from './sales.model';

@Injectable({
  providedIn: 'root',
})
export class SalesService {
  private salesSubject = new BehaviorSubject<Sale[]>([]);
  sales$ = this.salesSubject.asObservable();

  constructor(private http: HttpClient) { }

  loadAllSales() {
    this.http.get<Sale[]>('/api/sales').subscribe((sales) => {
      this.salesSubject.next(sales);
    });
  }

  searchSales(term: string) {
    this.http.get<Sale[]>(`/api/sales/search?term=${term}`).subscribe((sales) => {
      this.salesSubject.next(sales);
    });
  }

  addSale(sale: Sale) {
    return this.http.post('/api/sales', sale);
  }

  updateSale(id: number, sale: Sale) {
    return this.http.put(`/api/sales/${id}`, sale);
  }

  deleteSale(id: number) {
    return this.http.delete(`/api/sales/${id}`);
  }
}
