import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { provideRouter } from '@angular/router';
import { routes } from './app/app.routes';
import { provideNzIcons } from 'ng-zorro-antd/icon';
import { en_US, provideNzI18n } from 'ng-zorro-antd/i18n';
import { registerLocaleData } from '@angular/common';
import en from '@angular/common/locales/en';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { icons } from './app/icons-provider';
import { jwtInterceptor } from './app/services/jwt.interceptor';

registerLocaleData(en);

bootstrapApplication(AppComponent, {
  providers: [
    provideRouter(routes),
    provideNzIcons(icons),
    provideNzI18n(en_US),
    provideAnimationsAsync(),
    provideHttpClient(withInterceptors([jwtInterceptor]))
  ]
});
