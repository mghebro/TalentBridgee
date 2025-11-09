import { Component, OnInit } from '@angular/core';
import { TestService } from '../../../services/test/test.service';
import { Test } from '../../../models/test/test';
import { TestFilter } from '../../../models/test/test-filter';
import { NzTableModule, NzTableQueryParams } from 'ng-zorro-antd/table';
import { NzModalModule, NzModalService } from 'ng-zorro-antd/modal';
import { TestFormComponent } from '../test-form/test-form.component';
import { CommonModule } from '@angular/common';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzPopconfirmModule } from 'ng-zorro-antd/popconfirm';

@Component({
  selector: 'app-test-list',
  templateUrl: './test-list.component.html',
  styleUrls: ['./test-list.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    NzTableModule,
    NzDividerModule,
    NzModalModule,
    NzButtonModule,
    TestFormComponent,
    NzPopconfirmModule
  ]
})
export class TestListComponent implements OnInit {
  tests: Test[] = [];
  loading = true;
  total = 0;
  pageSize = 10;
  pageIndex = 1;
  filter: TestFilter = {
    page: this.pageIndex,
    pageSize: this.pageSize
  };

  constructor(
    private testService: TestService,
    private modalService: NzModalService,
    private notification: NzNotificationService
  ) { }

  ngOnInit(): void {
    this.loadData();
  }

  loadData(reset = false): void {
    if (reset) {
      this.pageIndex = 1;
    }
    this.loading = true;
    this.filter.page = this.pageIndex;
    this.filter.pageSize = this.pageSize;
    this.testService.getTests(this.filter).subscribe(
      data => {
        this.loading = false;
        this.tests = data.items;
        this.total = data.totalCount;
      },
      error => {
        this.notification.error('Error', error.error.message || 'Failed to load tests');
        this.loading = false;
      }
    );
  }

  onQueryParamsChange(params: NzTableQueryParams): void {
    const { pageSize, pageIndex, sort } = params;
    const currentSort = sort.find(item => item.value !== null);
    const sortField = (currentSort && currentSort.key) || undefined;
    const sortOrder = (currentSort && currentSort.value) || undefined;

    this.pageIndex = pageIndex;
    this.pageSize = pageSize;
    this.filter.sortBy = sortField;
    this.filter.sortOrder = sortOrder === 'ascend' ? 'asc' : 'desc';
    this.loadData();
  }

  openCreateModal(): void {
    const modal = this.modalService.create({
      nzTitle: 'Create Test',
      nzContent: TestFormComponent,
      nzFooter: null
    });
    modal.afterClose.subscribe(result => {
      if (result) {
        this.loadData(true);
      }
    });
  }

  openEditModal(test: Test): void {
    const modal = this.modalService.create({
      nzTitle: 'Edit Test',
      nzContent: TestFormComponent,
      nzData: {
        test: test
      },
      nzFooter: null
    });
    modal.afterClose.subscribe(result => {
      if (result) {
        this.loadData();
      }
    });
  }

  deleteTest(id: number): void {
    // The backend README does not specify a delete endpoint for tests.
    // Assuming a delete endpoint exists at /api/tests/{id}
    // If not, this will need to be adjusted based on actual backend implementation.
    this.testService.deleteTest(id).subscribe(
      () => {
        this.notification.success('Success', 'Test deleted successfully');
        this.loadData();
      },
      error => {
        this.notification.error('Error', error.error.message || 'Failed to delete test');
      }
    );
  }
}
