import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { ApplicationService } from '../services/application.service'; 

@Injectable({ providedIn: 'root' })
export class ApplicationOwnerGuard implements CanActivate {
  constructor(private applicationService: ApplicationService, private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot): Observable<boolean> {
    const applicationId = +route.params['id'];

    return this.applicationService.getApplicationById(applicationId).pipe(
      map(() => true),
      catchError(() => {
        this.router.navigate(['/unauthorized']); 
        return of(false);
      })
    );
  }
}
