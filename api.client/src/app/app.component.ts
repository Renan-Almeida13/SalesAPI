import { Component } from '@angular/core';
import { SalesComponent } from './sales/sales.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [SalesComponent],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'Sales App';
}
