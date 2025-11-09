import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormArray, ReactiveFormsModule } from '@angular/forms';
import { TestService } from '../../../services/test/test.service';
import { CreateTestRequest, Test } from '../../../models/test/test';
import { NzModalRef } from 'ng-zorro-antd/modal';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { OrganizationService } from '../../../services/organization/organization.service';
import { OrganizationList} from '../../../models/organization.model'; // Corrected import path
import { CommonModule } from '@angular/common';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { NzDividerModule } from 'ng-zorro-antd/divider';

@Component({
  selector: 'app-test-form',
  templateUrl: './test-form.component.html',
  styleUrls: ['./test-form.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    NzFormModule,
    NzInputModule,
    NzButtonModule,
    NzSelectModule,
    NzInputNumberModule,
    NzCardModule,
    NzGridModule,
    NzIconModule,
    NzCheckboxModule,
    NzDividerModule
  ]
})
export class TestFormComponent implements OnInit {
  @Input() test?: Test;
  form!: FormGroup;
  organizations: OrganizationList[] = [];

  constructor(
    private fb: FormBuilder,
    private testService: TestService,
    private organizationService: OrganizationService,
    private modalRef: NzModalRef,
    private notification: NzNotificationService
  ) { }

  ngOnInit(): void {
    this.loadOrganizations();
    this.form = this.fb.group({
      title: [this.test?.title, [Validators.required]],
      organizationId: [this.test?.organizationId, [Validators.required]],
      description: [this.test?.description, [Validators.required]],
      profession: [this.test?.profession, [Validators.required]],
      durationMinutes: [this.test?.durationMinutes, [Validators.required]],
      passingScore: [this.test?.passingScore, [Validators.required]],
      difficulty: [this.test?.difficulty, [Validators.required]],
      questions: this.fb.array([])
    });

    if (this.test?.questions) {
      this.test.questions.forEach(q => this.addQuestion(q));
    }
  }

  get questions(): FormArray {
    return this.form.get('questions') as FormArray;
  }

  getOptions(questionIndex: number): FormArray {
    return this.questions.at(questionIndex).get('options') as FormArray;
  }

  newQuestion(): FormGroup {
    return this.fb.group({
      questionText: ['', Validators.required],
      questionType: ['MultipleChoice', Validators.required],
      points: [0, Validators.required],
      options: this.fb.array([])
    });
  }

  addQuestion(question?: any): void {
    const questionForm = this.newQuestion();
    if (question) {
      questionForm.patchValue(question);
      question.options.forEach((o: any) => {
        (questionForm.get('options') as FormArray).push(this.newOption(o));
      });
    }
    this.questions.push(questionForm);
  }

  removeQuestion(i: number): void {
    this.questions.removeAt(i);
  }

  newOption(option?: any): FormGroup {
    return this.fb.group({
      optionText: [option?.optionText || '', Validators.required],
      isCorrect: [option?.isCorrect || false]
    });
  }

  addOption(questionIndex: number): void {
    this.getOptions(questionIndex).push(this.newOption());
  }

  removeOption(questionIndex: number, optionIndex: number): void {
    this.getOptions(questionIndex).removeAt(optionIndex);
  }

  loadOrganizations(): void {
    this.organizationService.getOrganizationsForCurrentUser().subscribe(
      orgs => {
        this.organizations = orgs.items; // Access items from PaginatedResult
      },
      error => {
        this.notification.error('Error', error.error.message || 'Failed to load organizations');
      }
    );
  }

  submitForm(): void {
    if (this.form.valid) {
      if (this.test) {
        // Assuming there's an updateTest method in TestService
        // The backend README does not explicitly define an UpdateTestRequest DTO,
        // so I'll use the form value directly, assuming it matches the backend's expected structure.
        this.testService.updateTest(this.test.id, this.form.value).subscribe(
          () => {
            this.notification.success('Success', 'Test updated successfully');
            this.modalRef.close(true);
          },
          (error) => {
            this.notification.error('Error', error.error.message || 'Failed to update test');
          }
        );
      } else {
        const createRequest: CreateTestRequest = this.form.value;
        this.testService.createTest(createRequest).subscribe(
          () => {
            this.notification.success('Success', 'Test created successfully');
            this.modalRef.close(true);
          },
          (error) => {
            this.notification.error('Error', error.error.message || 'Failed to create test');
          }
        );
      }
    } else {
      Object.values(this.form.controls).forEach(control => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true });
        }
      });
    }
  }
}
