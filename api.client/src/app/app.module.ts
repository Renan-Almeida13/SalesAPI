import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppComponent } from './app.component';
import { HttpClientModule } from '@angular/common/http';
import { SalesComponent } from './sales/sales.component';

@NgModule({
  declarations: [
    AppComponent,
    SalesComponent// Apenas componentes não standalone devem ser declarados aqui
  ],
  imports: [
    HttpClientModule,
    BrowserModule, // Outros módulos necessários
  ],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule { }
