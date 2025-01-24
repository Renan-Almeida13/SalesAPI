import { Component, OnInit } from '@angular/core';
import { SalesService } from './sales.service';
import { Sale } from './sales.model';

@Component({
  selector: 'app-sales',
  templateUrl: './sales.component.html',
  styleUrls: ['./sales.component.css'],
  standalone: false
})
export class SalesComponent implements OnInit {
  sales: Sale[] = [];
  searchId: number | undefined;
  searchResult: Sale | null = null;
  showModal: boolean = false;
  isEditMode: boolean = false;
  saleForm: any = {
    saleDate: '',
    customer: '',
    branch: '',
    products: [
      {
        productName: '',
        quantity: 0,
        unitPrice: 0,
        totalAmount: 0
      }
    ]
  };

  constructor(private salesService: SalesService) {}

  ngOnInit() {
    this.loadSales();
  }

  loadSales() {
    this.salesService.getSales().subscribe(data => {
      this.sales = data;
    });
  }

  searchSale() {
    if (this.searchId) {
      this.salesService.getSaleById(this.searchId).subscribe(result => {
        this.searchResult = result;
      });
    }
  }

  clearSearch() {
    this.searchId = undefined;
    this.searchResult = null;
  }

  openCreateModal() {
    this.isEditMode = false;
    this.saleForm = {
      saleDate: '',
      customer: '',
      branch: '',
      products: [
        {
          productName: '',
          quantity: 0,
          unitPrice: 0,
          totalAmount: 0
        }
      ]
    };
    this.showModal = true;
  }

  closeModal() {
    this.showModal = false;
  }

  submitForm() {
    if (this.isEditMode) {
      this.salesService.updateSale(this.saleForm).subscribe(() => {
        this.loadSales();
        this.closeModal();
      });
    } else {
      this.salesService.addSale(this.saleForm).subscribe(() => {
        this.loadSales();
        this.closeModal();
      });
    }
  }

  editSale(sale: Sale) {
    this.isEditMode = true;
    this.saleForm = { ...sale };
    this.showModal = true;
  }

  deleteSale(id: number) {
    if (id !== undefined && id !== null) {
    } else {
      console.error('ID is invalid');
    }
  }

  updateTotalAmount(product: any) {
    product.totalAmount = product.quantity * product.unitPrice;
  }
}