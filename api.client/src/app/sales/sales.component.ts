import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms'; // Importando ReactiveFormsModule
import { SalesService } from './sales.service';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-sales',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, HttpClientModule],
  templateUrl: './sales.component.html',
  styleUrls: ['./sales.component.css'],
})
export class SalesComponent {
  sales: Sale[] = [];
  searchForm: FormGroup;
  editForm: FormGroup;
  isEditing = false;

  constructor(private salesService: SalesService, private fb: FormBuilder) {
    this.searchForm = this.fb.group({ term: '' });
    this.editForm = this.fb.group({
      id: null,
      saleNumber: '',
      date: '',
      customer: '',
      totalAmount: 0,
      branch: '',
    });

    this.salesService.sales$.subscribe((data) => {
      this.sales = data;
      console.log('Vendas carregadas:', this.sales); // Verificar se as vendas estão sendo recebidas
    });
  }

  search() {
    const term = this.searchForm.value.term;
    if (term) {
      this.salesService.searchSales(term);
    } else {
      this.salesService.loadAllSales();
    }
  }

  addSale() {
    const newSale = this.editForm.value as Sale;
    newSale.date = new Date(newSale.date);
    this.salesService.addSale(newSale).subscribe(() => {
      this.salesService.loadAllSales();
      this.resetForm();
    });
  }

  editSale(sale: Sale) {
    this.isEditing = true;
    this.editForm.patchValue({ ...sale, date: new Date(sale.date).toISOString().split('T')[0] });
  }

  updateSale() {
    const updatedSale = this.editForm.value as Sale;
    updatedSale.date = new Date(updatedSale.date);
    this.salesService.updateSale(updatedSale.id, updatedSale).subscribe(() => {
      this.salesService.loadAllSales();
      this.resetForm();
    });
  }

  deleteSale(saleId: number) {
    if (confirm('Are you sure you want to delete this sale?')) {
      this.salesService.deleteSale(saleId).subscribe(() => {
        this.salesService.loadAllSales();
      });
    }
  }

  resetForm() {
    this.isEditing = false;
    this.editForm.reset();
  }
}

export interface Sale {
  id: number;
  saleNumber: string;
  date: Date;
  customer: string;
  totalAmount: number;
  branch: string;
}
