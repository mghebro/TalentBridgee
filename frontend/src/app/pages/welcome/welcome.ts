import { Component } from '@angular/core';
import { VacancyListComponent } from '../../components/vacancy/vacancy-list/vacancy-list.component';

@Component({
  selector: 'app-welcome',
  standalone: true,
  imports: [VacancyListComponent],
  templateUrl: './welcome.html',
  styleUrl: './welcome.scss'
})
export class Welcome {}
