import { Component, OnInit } from '@angular/core';
import { VacancyService } from '../../../services/vacancy/vacancy.service';
import { Vacancy } from '../../../models/vacancy/vacancy';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzListModule } from 'ng-zorro-antd/list';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { VacancyFilter } from '../../../models/vacancy/vacancy-filter';
import { EmploymentType } from '../../../models/enums';

@Component({
  selector: 'app-vacancy-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterLink,
    NzCardModule,
    NzListModule,
    NzFormModule,
    NzInputModule,
    NzButtonModule,
    NzSelectModule
  ],
  templateUrl: './vacancy-list.component.html',
  styleUrls: ['./vacancy-list.component.scss']
})
export class VacancyListComponent implements OnInit {
  vacancies: Vacancy[] = [];
  filter: VacancyFilter = {};
  employmentTypes = Object.values(EmploymentType);

  constructor(private vacancyService: VacancyService) { }

  ngOnInit(): void {
    this.loadVacancies();
  }

  loadVacancies(): void {
    this.vacancyService.getVacancies(this.filter).subscribe(vacancies => {
      this.vacancies = vacancies;
    });
  }

  search(): void {
    this.loadVacancies();
  }
}