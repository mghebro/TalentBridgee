import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { routes } from './app.routes';

import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { NzIconModule, NZ_ICONS } from 'ng-zorro-antd/icon';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { NzNotificationComponent } from 'ng-zorro-antd/notification';
import { NzPopconfirmModule } from 'ng-zorro-antd/popconfirm';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { NzPageHeaderModule } from 'ng-zorro-antd/page-header';
import { NzDescriptionsModule } from 'ng-zorro-antd/descriptions';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';

import { NzNotificationService } from 'ng-zorro-antd/notification';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { jwtInterceptor } from './services/jwt.interceptor';

import {
  MenuFoldOutline,
  MenuUnfoldOutline,
  DashboardOutline,
  BankOutline,
  ProfileOutline,
  SolutionOutline,
  FormOutline,
  LogoutOutline,
  UserOutline,
  LockOutline,
  MailOutline
} from '@ant-design/icons-angular/icons';

import { CookieService } from 'ngx-cookie-service';

import { LayoutComponent } from './components/layout/layout.component';
import { LoginComponent } from './components/auth/login/login.component';
import { RegisterComponent } from './components/auth/register/register.component';
import { OrganizationListComponent } from './components/organization/organization-list/organization-list.component';
import { OrganizationFormComponent } from './components/organization/organization-form/organization-form.component';
import { VacancyFormComponent } from './components/vacancy/vacancy-form/vacancy-form.component';
import { ApplicationListComponent } from './components/application/application-list/application-list.component';
import { ApplicationDetailComponent } from './components/application/application-detail/application-detail.component';

@NgModule({
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    RouterModule.forRoot(routes),
    NzLayoutModule,
    NzMenuModule,
    NzIconModule,
    NzButtonModule,
    NzModalModule,
    NzFormModule,
    NzInputModule,
    NzSelectModule,
    NzCheckboxModule,
    NzDatePickerModule,
    NzPopconfirmModule,
    NzTagModule,
    NzSpinModule,
    NzPageHeaderModule,
    NzDescriptionsModule,
    NzDropDownModule,
    NzNotificationComponent
  ],
  providers: [
    CookieService,
    NzNotificationService,
    { provide: HTTP_INTERCEPTORS, useValue: jwtInterceptor, multi: true },
    {
      provide: NZ_ICONS,
      useValue: [
        MenuFoldOutline,
        MenuUnfoldOutline,
        DashboardOutline,
        BankOutline,
        ProfileOutline,
        SolutionOutline,
        FormOutline,
        LogoutOutline,
        UserOutline,
        LockOutline,
        MailOutline
      ]
    }
  ]
})
export class AppModule { }