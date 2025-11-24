import { Component, Input, OnInit } from '@angular/core';
import { NzModalRef } from 'ng-zorro-antd/modal';
import { TestService } from '../../../services/test/test.service';
import { Test } from '../../../models/test/test';
import { CommonModule } from '@angular/common';
import { NzListModule } from 'ng-zorro-antd/list';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { VacancyService } from '../../../services/vacancy/vacancy.service';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzCardModule } from 'ng-zorro-antd/card';

@Component({
  selector: 'app-assign-test-modal',
  standalone: true,
  imports: [CommonModule, NzListModule, NzButtonModule, NzCardModule],
  templateUrl: './assign-test-modal.component.html',
  styleUrls: ['./assign-test-modal.component.scss']
})
export class AssignTestModalComponent implements OnInit {
  @Input() vacancy: any;
  tests: Test[] = [];

  constructor(
    private modalRef: NzModalRef,
    private testService: TestService,
    private vacancyService: VacancyService,
    private notification: NzNotificationService
  ) {}

  ngOnInit(): void {
    if (this.vacancy) {
      this.testService.getTestsByOrganization(this.vacancy.organizationId).subscribe((tests: Test[]) => {
        this.tests = tests ?? [];
      });
    }
  }

  assignTest(testId: string): void {
    this.vacancyService.assignTestToVacancy(this.vacancy.id, testId).subscribe({
      next: () => {
        this.notification.success('Success', 'Test assigned successfully.');
        this.modalRef.close('assigned');
      },
      error: (err: any) => {
        this.notification.error('Error', err.message);
      }
    });
  }

  close(): void {
    this.modalRef.close();
  }
}
