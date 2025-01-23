import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SalesComponent } from './sales/sales.component';

const routes: Routes = [
  { path: 'sales', loadComponent: () => import('./sales/sales.component').then((m) => m.SalesComponent) },
  { path: '', redirectTo: 'sales', pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule { }
