import { Routes } from '@angular/router';
import { LayoutComponent } from './components/layout/layout.component';
import { LoginComponent } from './components/auth/login/login.component';
import { RegisterComponent } from './components/auth/register/register.component';
import { AuthGuard } from './services/auth.guard';
import { OrganizationListComponent } from './components/organization/organization-list/organization-list.component';
import { OrganizationFormComponent } from './components/organization/organization-form/organization-form.component';
import { VacancyFormComponent } from './components/vacancy/vacancy-form/vacancy-form.component';
import { VacancyListComponent } from './components/vacancy/vacancy-list/vacancy-list.component';
import { ApplicationListComponent } from './components/application/application-list/application-list.component';
import { ApplicationDetailComponent } from './components/application/application-detail/application-detail.component';
import { MyApplicationsComponent } from './components/application/my-applications/my-applications.component';
import { TestListComponent } from './components/test/test-list/test-list.component';
import { TestFormComponent } from './components/test/test-form/test-form.component';
import { TestTakingComponent } from './components/test/test-taking/test-taking.component';
import { VerifyEmailComponent } from './components/auth/verify-email/verify-email.component';
import { VacancyDetailComponent } from './components/vacancy/vacancy-detail/vacancy-detail.component';
import { VacancyManagementComponent } from './components/vacancy/vacancy-management/vacancy-management.component';

export const routes: Routes = [
  {
    path: 'auth',
    children: [
      { path: 'login', component: LoginComponent },
      { path: 'register', component: RegisterComponent },
      { path: 'verify-email', component: VerifyEmailComponent },
    ],
  },
  {
    path: '',
    component: LayoutComponent,
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'vacancies' },

      { path: 'vacancies', pathMatch: 'full', component: VacancyListComponent },
      { path: 'vacancies/:id', component: VacancyDetailComponent },
      {
        path: 'my-applications',
        canActivate: [AuthGuard],
        component: MyApplicationsComponent,
      },
      {
        path: 'organizations',
        //canActivate: [AuthGuard],
        component: OrganizationListComponent,
      },
      {
        path: 'organizations/new',
        canActivate: [AuthGuard],
        component: OrganizationFormComponent,
      },
      {
        path: 'organizations/edit/:id',
        canActivate: [AuthGuard],
        component: OrganizationFormComponent,
      },
      {
        path: 'vacancies/new',
        canActivate: [AuthGuard],
        component: VacancyFormComponent,
      },
      {
        path: 'vacancies/edit/:id',
        canActivate: [AuthGuard],
        component: VacancyFormComponent,
      },
      {
        path: 'vacancy-management',
        canActivate: [AuthGuard],
        component: VacancyManagementComponent,
      },
      {
        path: 'vacancies/:vacancyId/applications',
        canActivate: [AuthGuard],
        component: ApplicationListComponent,
      },
      {
        path: 'applications',
        canActivate: [AuthGuard],
        component: ApplicationListComponent,
      },
      {
        path: 'applications/:id',
        canActivate: [AuthGuard],
        component: ApplicationDetailComponent,
      },
      {
        path: 'tests',
        canActivate: [AuthGuard],
        component: TestListComponent,
      },
      {
        path: 'tests/new',
        canActivate: [AuthGuard],
        component: TestFormComponent,
      },
      {
        path: 'tests/edit/:id',
        canActivate: [AuthGuard],
        component: TestFormComponent,
      },
      {
        path: 'tests/:id',
        canActivate: [AuthGuard],
        component: TestTakingComponent,
      },
    ],
  },
  { path: '**', redirectTo: '' }, // Redirect any unmatched routes to the home page
];
