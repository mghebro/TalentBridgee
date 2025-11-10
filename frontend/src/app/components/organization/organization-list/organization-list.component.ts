import { Component, OnInit } from '@angular/core';
import { OrganizationService } from '../../../services/organization/organization.service';
import { OrganizationList } from '../../../models/organization.model';
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
import { OrganizationFilter } from '../../../models/organization/organization-filter';
import { OrganizationType } from '../../../models/organization.model';

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
  filter: OrganizationFilter = {};
  organizationTypes = Object.values(OrganizationType);

  constructor(
    private organizationService: OrganizationService,
    private modalService: NzModalService,
    private notification: NzNotificationService
  ) { }

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.loading = true;
    this.organizationService.getOrganizations(this.filter).subscribe({
      next: data => {
        this.loading = false;
        this.organizations = data;
      },
      error: error => {
        this.loading = false;
        this.notification.error('Error', error.error.message || 'Failed to load organizations');
      }
    });
  }

  onQueryParamsChange(params: NzTableQueryParams): void {
    // No longer handling pagination and sorting via query params in this component
    // as the OrganizationFilter model has changed.
    // If pagination/sorting is needed, OrganizationFilter needs to be updated.
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
        this.loadData();
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

  deleteOrganization(id: string): void {
    this.organizationService.deleteOrganization(id).subscribe(() => {
      this.notification.success('Success', 'Organization deleted successfully.');
      this.loadData();
    },
    error => {
      this.notification.error('Error', error.error.message || 'Failed to delete organization.');
    });
  }
}
