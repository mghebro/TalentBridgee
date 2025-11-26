import { Component, OnInit } from '@angular/core';
import { TestService } from '../../../services/test/test.service';
import { Test } from '../../../models/test/test';
import { CommonModule } from '@angular/common';
import { NzListModule } from 'ng-zorro-antd/list';
import { NzCardModule } from 'ng-zorro-antd/card';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../services/auth/auth.service';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzEmptyModule } from 'ng-zorro-antd/empty';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzSpinModule } from 'ng-zorro-antd/spin';

@Component({
  selector: 'app-test-list',
  standalone: true,
  imports: [
    CommonModule,
    NzListModule,
    NzCardModule,
    RouterLink,
    NzButtonModule,
    NzIconModule,
    NzEmptyModule,
    NzTagModule,
    NzSpinModule,
  ],
  templateUrl: './test-list.component.html',
  styleUrls: ['./test-list.component.scss'],
})
export class TestListComponent implements OnInit {
  tests: Test[] = [];
  isOrgAdmin = false;
  isLoading = true;

  constructor(private testService: TestService, private authService: AuthService) {}

  ngOnInit(): void {
    const user = this.authService.currentUserValue;
    this.isOrgAdmin = user?.role === 'ORGANIZATION_ADMIN' || user?.role === 'HR_MANAGER';

    this.isLoading = true;
    if (this.isOrgAdmin) {
      this.testService.getMyCreatedTests().subscribe({
        next: (tests) => {
          this.tests = tests;
          this.isLoading = false;
        },
        error: () => (this.isLoading = false),
      });
    } else {
      this.testService.getMyAssignedTests().subscribe({
        next: (tests) => {
          this.tests = tests;
          this.isLoading = false;
        },
        error: () => (this.isLoading = false),
      });
    }
  }

  getDifficultyColor(difficulty: string): string {
    const colors: Record<string, string> = {
      Easy: 'green',
      Medium: 'orange',
      Hard: 'red',
    };
    return colors[difficulty] || 'default';
  }
}
