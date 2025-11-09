import { Component, OnInit } from '@angular/core';
import { OrganizationService } from '../../../services/organization/organization.service';
import { OrganizationFilterRequest, OrganizationList } from '../../../models/organization.model';
import { NzTableModule, NzTableQueryParams } from 'ng-zorro-antd/table';
import { NzModalService } from 'ng-zorro-antd/modal';
import { OrganizationFormComponent } from '../organization-form/organization-form.component';
import { CommonModule } from '@angular/common';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzPopconfirmModule } from 'ng-zorro-antd/popconfirm';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { FormsModule } from '@angular/forms';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { RouterLink } from '@angular/router';
import { NzTagModule } from 'ng-zorro-antd/tag';

@Component({
  selector: 'app-organization-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    NzTableModule,
    NzButtonModule,
    NzDividerModule,
    NzPopconfirmModule,
    OrganizationFormComponent,
    NzInputModule,
    NzSelectModule,
    FormsModule,
    NzTagModule
  ],
  templateUrl: './organization-list.component.html',
  styleUrls: ['./organization-list.component.scss']
})
export class OrganizationListComponent implements OnInit {
  organizations: OrganizationList[] = [];
  loading = true;
  total = 0;
  pageSize = 10;
  pageIndex = 1;
  filter: OrganizationFilterRequest = {
    page: this.pageIndex,
    pageSize: this.pageSize,
    sortBy: 'createdAt',
    sortOrder: 'desc'
  };

  constructor(
    private organizationService: OrganizationService,
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
    this.organizationService.getOrganizations(this.filter).subscribe(data => {
      this.loading = false;
      this.organizations = data.items;
      this.total = data.totalCount;
    },
    error => {
      this.loading = false;
      this.notification.error('Error', error.error.message || 'Failed to load organizations');
    });
  }

  onQueryParamsChange(params: NzTableQueryParams): void {
    const { pageSize, pageIndex, sort } = params;
    const currentSort = sort.find(item => item.value !== null);
    const sortField = (currentSort && currentSort.key) || 'createdAt';
    const sortOrder = (currentSort && currentSort.value) || 'desc';

    this.pageIndex = pageIndex;
    this.pageSize = pageSize;
    this.filter.sortBy = sortField;
    this.filter.sortOrder = sortOrder === 'ascend' ? 'asc' : 'desc';
    this.loadData();
  }

  openCreateModal(): void {
    const modal = this.modalService.create({
      nzTitle: 'Create Organization',
      nzContent: OrganizationFormComponent,
      nzWidth: '800px',
      nzFooter: null
    });
    modal.afterClose.subscribe(result => {
      if (result === 'created') {
        this.notification.success('Success', 'Organization created successfully.');
        this.loadData(true);
      }
    });
  }

  openEditModal(organization: OrganizationList): void {
    const modal = this.modalService.create({
      nzTitle: 'Edit Organization',
      nzContent: OrganizationFormComponent,
      nzWidth: '800px',
      nzData: {
        organizationId: organization.id
      },
      nzFooter: null
    });
    modal.afterClose.subscribe(result => {
      if (result === 'updated') {
        this.notification.success('Success', 'Organization updated successfully.');
        this.loadData();
      }
    });
  }

  deleteOrganization(id: number): void {
    this.organizationService.deleteOrganization(id).subscribe(() => {
      this.notification.success('Success', 'Organization deleted successfully.');
      this.loadData();
    },
    error => {
      this.notification.error('Error', error.error.message || 'Failed to delete organization.');
    });
  }
}
