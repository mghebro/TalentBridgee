import { Component, Inject, OnInit } from '@angular/core';
import { NzModalRef, NZ_MODAL_DATA } from 'ng-zorro-antd/modal';
import { TestService } from '../../../services/test/test.service';
import { VacancyService } from '../../../services/vacancy/vacancy.service';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { FormBuilder, FormGroup, Validators, FormArray, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzDescriptionsModule } from 'ng-zorro-antd/descriptions';
import { CreateTestRequest, Test } from '../../../models/test/test';
import { extractErrorMessage } from '../../../utils/api-error';
import { Router } from '@angular/router';

@Component({
  selector: 'app-assign-test-modal',
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
    NzIconModule,
    NzCheckboxModule,
    NzDividerModule,
    NzSpinModule,
    NzTagModule,
    NzDescriptionsModule,
  ],
  templateUrl: './assign-test-modal.component.html',
  styleUrls: ['./assign-test-modal.component.scss'],
})
export class AssignTestModalComponent implements OnInit {
  form!: FormGroup;
  isLoading = false;
  existingTest: Test | null = null;
  isEditMode = false;
  hasTest = false;

  constructor(
    private modalRef: NzModalRef,
    private testService: TestService,
    private vacancyService: VacancyService,
    private notification: NzNotificationService,
    private fb: FormBuilder,
    private router: Router,
    @Inject(NZ_MODAL_DATA) public data: { vacancy: any }
  ) {}

  ngOnInit(): void {
    this.checkExistingTest();
  }

  checkExistingTest(): void {
    if (this.data?.vacancy?.testId) {
      this.hasTest = true;
      this.isLoading = true;
      this.testService.getTestById(this.data.vacancy.testId.toString()).subscribe({
        next: (test) => {
          this.existingTest = test;
          this.isLoading = false;
        },
        error: (error) => {
          this.notification.error('Error', extractErrorMessage(error, 'Failed to load test'));
          this.isLoading = false;
        },
      });
    } else {
      this.initForm();
      if (this.data && this.data.vacancy) {
        this.form.patchValue({
          organizationId: this.data.vacancy.organizationId.toString(),
          vacancyId: this.data.vacancy.id.toString(),
          profession: this.data.vacancy.profession,
        });
      }
    }
  }

  initForm(): void {
    this.form = this.fb.group({
      title: [null, [Validators.required]],
      organizationId: [null, [Validators.required]],
      vacancyId: [null, [Validators.required]],
      description: [null, [Validators.required]],
      profession: [null, [Validators.required]],
      durationMinutes: [null, [Validators.required]],
      passingScore: [null, [Validators.required]],
      difficulty: [null, [Validators.required]],
      questions: this.fb.array([]),
    });
  }

  initFormWithTest(test: Test): void {
    this.form = this.fb.group({
      title: [test.title, [Validators.required]],
      organizationId: [test.organizationId.toString(), [Validators.required]],
      vacancyId: [this.data.vacancy.id.toString(), [Validators.required]],
      description: [test.description, [Validators.required]],
      profession: [test.profession, [Validators.required]],
      durationMinutes: [test.durationMinutes, [Validators.required]],
      passingScore: [test.passingScore, [Validators.required]],
      difficulty: [test.difficulty, [Validators.required]],
      questions: this.fb.array([]),
    });

    if (test.questions && test.questions.length > 0) {
      const questionFGs = test.questions.map((q) =>
        this.fb.group({
          text: [q.text, Validators.required],
          type: [q.type, Validators.required],
          points: [q.points || 0, Validators.required],
          timeLimitSeconds: [q.timeLimitSeconds || null],
          options: this.fb.array(
            (q.options || []).map((o: any) =>
              this.fb.group({
                text: [o.text, Validators.required],
                isCorrect: [o.isCorrect || false],
              })
            )
          ),
        })
      );
      this.form.setControl('questions', this.fb.array(questionFGs));
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
      text: ['', Validators.required],
      type: ['SINGLE_CHOICE', Validators.required],
      points: [0, Validators.required],
      timeLimitSeconds: [null],
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
      text: [option?.text || '', Validators.required],
      isCorrect: [option?.isCorrect || false],
    });
  }

  addOption(questionIndex: number): void {
    this.getOptions(questionIndex).push(this.newOption());
  }

  removeOption(questionIndex: number, optionIndex: number): void {
    this.getOptions(questionIndex).removeAt(optionIndex);
  }

  enableEditMode(): void {
    if (this.existingTest) {
      this.isEditMode = true;
      this.initFormWithTest(this.existingTest);
    }
  }

  cancelEdit(): void {
    this.isEditMode = false;
    this.existingTest = null;
    this.checkExistingTest();
  }

  submitForm(): void {
    if (this.form.valid) {
      this.isLoading = true;
      const request: CreateTestRequest = this.form.value;

      if (this.isEditMode && this.existingTest) {
        this.testService.updateTest(this.existingTest.id.toString(), request).subscribe({
          next: () => {
            this.notification.success('Success', 'Test updated successfully.');
            this.modalRef.close('updated');
            this.isLoading = false;
          },
          error: (error) => {
            this.notification.error('Error', extractErrorMessage(error, 'Failed to update test'));
            this.isLoading = false;
          },
        });
      } else {
        this.testService.createTest(request).subscribe({
          next: (test) => {
            const testId = typeof test.id === 'string' ? parseInt(test.id, 10) : test.id;
            this.vacancyService.assignTestToVacancy(this.data.vacancy.id, testId).subscribe({
              next: () => {
                this.notification.success('Success', 'Test created and assigned successfully.');
                this.modalRef.close('assigned');
                this.isLoading = false;
              },
              error: (error) => {
                this.notification.error(
                  'Error',
                  extractErrorMessage(error, 'Test created but failed to assign to vacancy')
                );
                this.isLoading = false;
              },
            });
          },
          error: (error) => {
            this.notification.error('Error', extractErrorMessage(error, 'Failed to create test'));
            this.isLoading = false;
          },
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

  getDifficultyColor(difficulty: string): string {
    const colors: Record<string, string> = {
      Easy: 'green',
      Medium: 'orange',
      Hard: 'red',
    };
    return colors[difficulty] || 'default';
  }

  getTotalPoints(): number {
    if (!this.existingTest?.questions) return 0;
    return this.existingTest.questions.reduce((sum, q) => sum + (q.points || 0), 0);
  }

  close(): void {
    this.modalRef.close();
  }
}
