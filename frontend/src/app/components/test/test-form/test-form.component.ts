import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormArray, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { NzNotificationService } from 'ng-zorro-antd/notification';
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
import { NumericInputDirective } from '../../../directives/numeric-input.directive';

import { TestService } from '../../../services/test/test.service';
import { VacancyService } from '../../../services/vacancy/vacancy.service';
import { OrganizationService } from '../../../services/organization/organization.service';
import { CreateTestRequest } from '../../../models/test/test';
import { OrganizationList } from '../../../models/organization.model';
import { extractErrorMessage } from '../../../utils/api-error';
import { DECIMAL_PATTERN, INTEGER_PATTERN } from '../../../utils/validation-patterns';

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
    NzDividerModule,
    NumericInputDirective,
  ],
})
export class TestFormComponent implements OnInit {
  @Input() testId?: string;
  form!: FormGroup;
  organizations: OrganizationList[] = [];
  vacancies: any[] = [];
  isEditing = false;

  constructor(
    private fb: FormBuilder,
    private testService: TestService,
    private organizationService: OrganizationService,
    private vacancyService: VacancyService,
    private notification: NzNotificationService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadOrganizations();
    this.initForm();

    this.route.paramMap.subscribe((params) => {
      const id = params.get('id');
      if (id) {
        this.testId = id;
        this.isEditing = true;
        this.testService.getTestById(this.testId).subscribe((test) => {
          this.form.patchValue({
            ...test,
            organizationId: test.organizationId.toString(),
            vacancyId: test.vacancyId ? test.vacancyId.toString() : null,
          });
          this.setQuestions(test.questions);
          if (test.organizationId) {
            this.loadVacancies(test.organizationId);
          }
        });
      }
    });
  }

  initForm(): void {
    this.form = this.fb.group({
      title: [null, [Validators.required]],
      organizationId: [null, [Validators.required]],
      vacancyId: [null, [Validators.required]],
      description: [null, [Validators.required]],
      profession: [null, [Validators.required]],
      durationMinutes: [null, [Validators.required, Validators.pattern(INTEGER_PATTERN)]],
      passingScore: [
        null,
        [
          Validators.required,
          Validators.pattern(INTEGER_PATTERN),
          Validators.min(0),
          Validators.max(100),
        ],
      ],
      difficulty: [null, [Validators.required]],
      questions: this.fb.array([]),
    });

    this.form.get('organizationId')?.valueChanges.subscribe((orgId) => {
      if (orgId) {
        this.loadVacancies(orgId);
        this.form.get('vacancyId')?.setValue(null);
      } else {
        this.vacancies = [];
      }
    });
  }

  get questions(): FormArray {
    return this.form.get('questions') as FormArray;
  }

  getOptions(questionIndex: number): FormArray {
    return this.questions.at(questionIndex).get('options') as FormArray;
  }

  setQuestions(questions: any[]): void {
    const questionFGs = questions.map((q: any) =>
      this.fb.group({
        text: [q.text || q.questionText || q.QuestionText, Validators.required],
        type: [
          this.mapQuestionType(q.type || q.questionType || q.QuestionType) || 'SINGLE_CHOICE',
          Validators.required,
        ],
        points: [
          q.points || q.Points || 0,
          [Validators.required, Validators.pattern(DECIMAL_PATTERN)],
        ],
        timeLimitSeconds: [
          q.timeLimitSeconds !== undefined
            ? q.timeLimitSeconds
            : q.TimeLimitSeconds !== undefined
            ? q.TimeLimitSeconds
            : null,
          Validators.pattern(INTEGER_PATTERN),
        ],
        options: this.fb.array((q.options || q.Options || []).map((o: any) => this.newOption(o))),
      })
    );
    this.form.setControl('questions', this.fb.array(questionFGs));
  }

  newQuestion(): FormGroup {
    return this.fb.group({
      text: ['', Validators.required],
      type: ['SINGLE_CHOICE', Validators.required],
      points: [0, [Validators.required, Validators.pattern(DECIMAL_PATTERN)]],
      timeLimitSeconds: [null, Validators.pattern(INTEGER_PATTERN)],
      options: this.fb.array([]),
    });
  }

  addQuestion(): void {
    this.questions.push(this.newQuestion());
  }

  removeQuestion(i: number): void {
    this.questions.removeAt(i);
  }

  newOption(option?: any): FormGroup {
    return this.fb.group({
      text: [option?.text || option?.optionText || option?.OptionText || '', Validators.required],
      isCorrect: [option?.isCorrect !== undefined ? option.isCorrect : option?.IsCorrect || false],
    });
  }

  addOption(questionIndex: number): void {
    this.getOptions(questionIndex).push(this.newOption());
  }

  removeOption(questionIndex: number, optionIndex: number): void {
    this.getOptions(questionIndex).removeAt(optionIndex);
  }

  loadOrganizations(): void {
    this.organizationService.getMyOrganizations().subscribe({
      next: (orgs) => (this.organizations = orgs),
      error: (error) =>
        this.notification.error(
          'Error',
          extractErrorMessage(error, 'Failed to load organizations')
        ),
    });
  }

  loadVacancies(organizationId: number): void {
    this.vacancyService.getVacanciesByOrganization(organizationId).subscribe({
      next: (vacancies) => (this.vacancies = vacancies),
      error: (error) =>
        this.notification.error('Error', extractErrorMessage(error, 'Failed to load vacancies')),
    });
  }

  submitForm(): void {
    if (this.form.valid) {
      const request: CreateTestRequest = this.form.value;
      if (this.isEditing && this.testId) {
        this.testService.updateTest(this.testId, request).subscribe({
          next: () => {
            this.notification.success('Success', 'Test updated successfully');
            this.router.navigate(['/tests']);
          },
          error: (error) =>
            this.notification.error('Error', extractErrorMessage(error, 'Failed to update test')),
        });
      } else {
        this.testService.createTest(request).subscribe({
          next: () => {
            this.notification.success('Success', 'Test created successfully');
            this.router.navigate(['/tests']);
          },
          error: (error) =>
            this.notification.error('Error', extractErrorMessage(error, 'Failed to create test')),
        });
      }
    } else {
      Object.values(this.form.controls).forEach((control) => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true });
        }
      });
    }
  }

  cancel(): void {
    this.router.navigate(['/tests']);
  }

  private mapQuestionType(type: any): string {
    if (!type) return 'SINGLE_CHOICE';

    const typeStr = typeof type === 'string' ? type : String(type);

    const typeMap: Record<string, string> = {
      MultipleChoice: 'MULTIPLE_CHOICE',
      MULTIPLE_CHOICE: 'MULTIPLE_CHOICE',
      '0': 'MULTIPLE_CHOICE',
      SingleChoice: 'SINGLE_CHOICE',
      SINGLE_CHOICE: 'SINGLE_CHOICE',
      TrueFalse: 'SINGLE_CHOICE',
      ShortAnswer: 'SINGLE_CHOICE',
      Essay: 'SINGLE_CHOICE',
      Coding: 'SINGLE_CHOICE',
    };

    return typeMap[typeStr] || 'SINGLE_CHOICE';
  }
}
