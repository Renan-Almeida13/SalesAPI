import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Sale } from './sales.model';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-sales',
  templateUrl: './sales.component.html',
  styleUrls: ['./sales.component.css'],
  standalone: false
})
export class SalesComponent implements OnInit {
  sales: Sale[] = [];
  showModal = false;
  isEditMode = false;
  saleForm = {
    saleDate: '',
    customer: '',
    branch: '',
    products: [{ productName: '', quantity: 1, unitPrice: 0, totalAmount: 0 }]
  };
  saleToEdit: Sale | null = null;

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.loadSales();
  }

  loadSales() {
    this.http.get<Sale[]>('api/sales').subscribe(data => {
      this.sales = data;
    });
  }

  openCreateModal() {
    this.saleForm = { saleDate: '', customer: '', branch: '', products: [{ productName: '', quantity: 1, unitPrice: 0, totalAmount: 0 }] };
    this.isEditMode = false;
    this.showModal = true;
  }

  editSale(sale: Sale) {
    this.saleForm = { ...sale };
    this.isEditMode = true;
    this.saleToEdit = sale;
    this.showModal = true;
  }

  closeModal() {
    this.showModal = false;
  }

  saveSale() {
    const url = this.isEditMode
      ? `${environment.apiUrl}/api/sales/${this.saleToEdit?.id}` // Para edição
      : `${environment.apiUrl}/api/sales`; // Para criação
  
    const method = this.isEditMode ? 'put' : 'post';

    if (this.saleForm?.saleDate) {
      this.saleForm.saleDate = new Date(this.saleForm.saleDate).toISOString();
  }
  
    this.http[method](url, this.saleForm).subscribe(() => {
      this.loadSales();
      this.closeModal();
    });
  }

  deleteSale(saleId: number) {
    this.http.delete(`api/sales/${saleId}`).subscribe(() => {
      this.loadSales();
    });
  }
}
