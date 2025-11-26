import { Component, OnInit } from '@angular/core';
import { VacancyService } from '../../../services/vacancy/vacancy.service';
import { Vacancy } from '../../../models/vacancy/vacancy';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzListModule } from 'ng-zorro-antd/list';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzEmptyModule } from 'ng-zorro-antd/empty';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { VacancyFilter } from '../../../models/vacancy/vacancy-filter';
import { EmploymentType } from '../../../models/enums';
import { Router } from '@angular/router';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-vacancy-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    NzCardModule,
    NzListModule,
    NzFormModule,
    NzInputModule,
    NzButtonModule,
    NzSelectModule,
    NzTagModule,
    NzAvatarModule,
    NzEmptyModule,
    NzIconModule,
  ],
  templateUrl: './vacancy-list.component.html',
  styleUrls: ['./vacancy-list.component.scss'],
})
export class VacancyListComponent implements OnInit {
  vacancies: Vacancy[] = [];
  filter: VacancyFilter = {};
  employmentTypes = Object.values(EmploymentType);
  viewMode: 'grid' | 'list' = 'grid';

  constructor(private vacancyService: VacancyService, private router: Router) {}

  ngOnInit(): void {
    this.loadVacancies();
  }

  loadVacancies(): void {
    this.vacancyService.getVacancies(this.filter).subscribe((vacancies) => {
      this.vacancies = vacancies;
    });
  }

  search(): void {
    this.loadVacancies();
  }

  viewVacancy(vacancy: Vacancy): void {
    this.router.navigate(['/vacancies', vacancy.id]);
  }

  getLogoUrl(relativePath: string | null | undefined): string {
    if (!relativePath) return '';
    const baseUrl = environment.apiUrl.replace('/api', '');
    return `${baseUrl}${relativePath}`;
  }
}
