import { Component, OnInit } from '@angular/core';
import { TestService } from '../../../services/test/test.service';
import { Test } from '../../../models/test/test';
import { CommonModule } from '@angular/common';
import { NzListModule } from 'ng-zorro-antd/list';
import { NzCardModule } from 'ng-zorro-antd/card';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-test-list',
  standalone: true,
  imports: [CommonModule, NzListModule, NzCardModule, RouterLink],
  templateUrl: './test-list.component.html',
  styleUrls: ['./test-list.component.scss']
})
export class TestListComponent implements OnInit {
  tests: Test[] = [];

  constructor(private testService: TestService) { }

  ngOnInit(): void {
    this.testService.getMyAssignedTests().subscribe(tests => {
      this.tests = tests;
    });
  }
}