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
import { TestListComponent } from './components/test/test-list/test-list.component';
import { TestFormComponent } from './components/test/test-form/test-form.component';
import { VerifyEmailComponent } from './components/auth/verify-email/verify-email.component';

export const routes: Routes = [
  {
    path: 'auth',
    children: [
      { path: 'login', component: LoginComponent },
      { path: 'register', component: RegisterComponent },
      { path: 'verify-email', component: VerifyEmailComponent },
    ]
  },
  {
    path: '',
    component: LayoutComponent,
    canActivate: [AuthGuard],
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'welcome' },
      { path: 'welcome', loadChildren: () => import('./pages/welcome/welcome.routes').then(m => m.WELCOME_ROUTES) },
      { path: 'organizations', component: OrganizationListComponent },
      { path: 'organizations/new', component: OrganizationFormComponent },
      { path: 'organizations/edit/:id', component: OrganizationFormComponent },
      { path: 'vacancies', component: VacancyListComponent },
      { path: 'vacancies/new', component: VacancyFormComponent },
      { path: 'vacancies/edit/:id', component: VacancyFormComponent },
      { path: 'applications', component: ApplicationListComponent },
      { path: 'applications/:id', component: ApplicationDetailComponent },
      { path: 'tests', component: TestListComponent },
      { path: 'tests/new', component: TestFormComponent },
      { path: 'tests/edit/:id', component: TestFormComponent },
    ]
  },
  { path: '**', redirectTo: '' } // Redirect any unmatched routes to the home page
]