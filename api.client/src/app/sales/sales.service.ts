import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Sale } from './sales.model';

@Injectable({
  providedIn: 'root'
})
export class SalesService {
  private apiUrl = `${environment.apiUrl}/sales`;

  constructor(private http: HttpClient) {}

  getSales(): Observable<Sale[]> {
    return this.http.get<Sale[]>(this.apiUrl);
  }

  getSaleById(id: number): Observable<Sale> {
    return this.http.get<Sale>(`${this.apiUrl}/${id}`);
  }

  addSale(sale: Sale): Observable<Sale> {
    return this.http.post<Sale>(this.apiUrl, sale);
  }

  updateSale(sale: Sale): Observable<Sale> {
    if (sale.saleDate) {
      const dateUtc = new Date(sale.saleDate);
      sale.saleDate = dateUtc.toISOString().split('T')[0];
    }
  
    const saleWithSaleId = { ...sale, saleId: sale.id };
    return this.http.put<Sale>(`${this.apiUrl}/${sale.id}`, saleWithSaleId);
  }

  deleteSale(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
