import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { VacancyService } from '../../../services/vacancy/vacancy.service';
import { Vacancy } from '../../../models/vacancy/vacancy';
import { CommonModule } from '@angular/common';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { AuthService } from '../../../services/auth/auth.service';
import { User } from '../../../models/auth/user';
import { ApplicationService } from '../../../services/application/application.service';
import { NzModalService } from 'ng-zorro-antd/modal';
import { ApplyModalComponent } from '../apply-modal/apply-modal.component';

@Component({
  selector: 'app-vacancy-detail',
  standalone: true,
  imports: [CommonModule, NzCardModule, NzButtonModule],
  templateUrl: './vacancy-detail.component.html',
  styleUrls: ['./vacancy-detail.component.scss']
})
export class VacancyDetailComponent implements OnInit {
  vacancy: Vacancy | undefined;
  currentUser: User | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private vacancyService: VacancyService,
    private authService: AuthService,
    private applicationService: ApplicationService,
    private modalService: NzModalService
  ) {
    this.authService.currentUser.subscribe(user => {
      this.currentUser = user;
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.vacancyService.getVacancyById(id).subscribe(vacancy => {
        this.vacancy = vacancy;
      });
    }
  }

  apply(): void {
    if (!this.currentUser) {
      this.router.navigate(['/auth/login']);
      return;
    }

    if (this.vacancy) {
      const modal = this.modalService.create({
        nzTitle: `Apply for ${this.vacancy.title}`,
        nzContent: ApplyModalComponent,
        nzData: {
          vacancyId: this.vacancy.id
        },
        nzFooter: null
      });

      modal.afterClose.subscribe(result => {
        if (result) {
          // Handle successful application if needed
        }
      });
    }
  }
}
