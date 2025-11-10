import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormArray, ReactiveFormsModule } from '@angular/forms';
import { TestService } from '../../../services/test/test.service';
import { CreateTestRequest, Test } from '../../../models/test/test';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { OrganizationService } from '../../../services/organization/organization.service';
import { OrganizationList} from '../../../models/organization.model';
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
import { ActivatedRoute, Router } from '@angular/router';

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
  @Input() testId?: string;
  form!: FormGroup;
  organizations: OrganizationList[] = [];
  isEditing = false;

  constructor(
    private fb: FormBuilder,
    private testService: TestService,
    private organizationService: OrganizationService,
    private notification: NzNotificationService,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadOrganizations();
    this.initForm();

    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.testId = id;
        this.isEditing = true;
        this.testService.getTestById(this.testId).subscribe(test => {
          this.form.patchValue(test);
          this.setQuestions(test.questions);
        });
      }
    });
  }

  initForm(): void {
    this.form = this.fb.group({
      title: [null, [Validators.required]],
      organizationId: [null, [Validators.required]],
      description: [null, [Validators.required]],
      profession: [null, [Validators.required]],
      durationMinutes: [null, [Validators.required]],
      passingScore: [null, [Validators.required]],
      difficulty: [null, [Validators.required]],
      questions: this.fb.array([])
    });
  }

  setQuestions(questions: any[]): void {
    const questionFGs = questions.map(q => {
      const questionForm = this.fb.group({
        questionText: [q.text, Validators.required],
        questionType: [q.type, Validators.required],
        points: [q.points || 0, Validators.required],
        options: this.fb.array(q.options.map((o: any) => this.newOption(o)))
      });
      return questionForm;
    });
    this.form.setControl('questions', this.fb.array(questionFGs));
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
      questionType: ['SINGLE_CHOICE', Validators.required],
      points: [0, Validators.required],
      options: this.fb.array([])
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
      text: [option?.text || '', Validators.required],
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
    this.organizationService.getMyOrganizations().subscribe({
      next: orgs => {
        this.organizations = orgs;
      },
      error: error => {
        this.notification.error('Error', error.error.message || 'Failed to load organizations');
      }
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
          error: (error) => {
            this.notification.error('Error', error.error.message || 'Failed to update test');
          }
        });
      } else {
        this.testService.createTest(request).subscribe({
          next: () => {
            this.notification.success('Success', 'Test created successfully');
            this.router.navigate(['/tests']);
          },
          error: (error) => {
            this.notification.error('Error', error.error.message || 'Failed to create test');
          }
        });
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

  cancel(): void {
    this.router.navigate(['/tests']);
  }
}
