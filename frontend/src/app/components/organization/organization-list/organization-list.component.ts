import { Component, DOCUMENT, Inject, OnInit } from '@angular/core';
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
import { extractErrorMessage } from '../../../utils/api-error';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { map, Observable } from 'rxjs';
import { User } from '../../../models/user.model';
import { AuthService } from '../../../services/auth/auth.service';

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
    NzModalModule,
    OrganizationFormComponent,
    NzInputModule,
    NzSelectModule,
    FormsModule,
    NzTagModule,
  ],
  templateUrl: './organization-list.component.html',
  styleUrls: ['./organization-list.component.scss'],
})
export class OrganizationListComponent implements OnInit {
  organizations: OrganizationList[] = [];
  loading = true;
  filter: OrganizationFilter = {};
  organizationTypes = Object.values(OrganizationType);
  currentUser$: Observable<User | null>;

  constructor(
    private authService: AuthService,
    private organizationService: OrganizationService,
    private modalService: NzModalService,
    private notification: NzNotificationService,
    @Inject(DOCUMENT) private document: Document
  ) {
    this.currentUser$ = this.authService.currentUser as Observable<User | null>;
  }

  ngOnInit(): void {
    this.loadData();
    if (!this.authService.currentUserValue && this.authService.hasToken()) {
      this.authService.getCurrentUser(true).subscribe();
    }
  }
  userCanManage(): Observable<boolean> {
    return this.currentUser$.pipe(
      map((user) => {
        if (!user) {
          return false;
        }
        return user.role !== 'USER';
      })
    );
  }
  loadData(): void {
    this.loading = true;
    this.organizationService.getOrganizations(this.filter).subscribe({
      next: (data) => {
        this.loading = false;
        this.organizations = data;
      },
      error: (error) => {
        this.loading = false;
        this.notification.error(
          'Error',
          extractErrorMessage(error, 'Failed to load organizations')
        );
      },
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
      nzFooter: null,
    });
    modal.afterClose.subscribe((result) => {
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
        organizationId: organization.id,
      },
      nzFooter: null,
    });
    modal.afterClose.subscribe((result) => {
      if (result === 'updated') {
        this.notification.success('Success', 'Organization updated successfully.');
        this.loadData();
      }
    });
  }

  deleteOrganization(id: string): void {
    this.organizationService.deleteOrganization(id).subscribe(
      () => {
        this.notification.success('Success', 'Organization deleted successfully.');
        this.loadData();
      },
      (error) => {
        this.notification.error(
          'Error',
          extractErrorMessage(error, 'Failed to delete organization.')
        );
      }
    );
  }
}
