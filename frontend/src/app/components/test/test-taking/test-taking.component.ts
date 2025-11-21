import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TestService } from '../../../services/test/test.service';
import { Test } from '../../../models/test/test';
import { FormBuilder, FormGroup, FormArray, FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzRadioModule } from 'ng-zorro-antd/radio';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { extractErrorMessage } from '../../../utils/api-error';

@Component({
  selector: 'app-test-taking',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    NzCardModule,
    NzButtonModule,
    NzRadioModule,
    NzCheckboxModule
  ],
  templateUrl: './test-taking.component.html',
  styleUrls: ['./test-taking.component.scss']
})
export class TestTakingComponent implements OnInit {
  test: Test | undefined;
  testForm!: FormGroup;
  isLoading = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private testService: TestService,
    private fb: FormBuilder,
    private notification: NzNotificationService
  ) { }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.testService.getTestById(id).subscribe(test => {
        this.test = test;
        this.buildForm();
      });
    }
  }

  buildForm(): void {
    const questions = this.test?.questions.map(q => {
      if (q.type === 'MULTIPLE_CHOICE') {
        const options = q.options.map(() => new FormControl(false));
        return this.fb.group({
          questionId: q.id,
          options: new FormArray(options)
        });
      } else {
        return this.fb.group({
          questionId: q.id,
          optionId: ['', Validators.required]
        });
      }
    }) || [];
    this.testForm = this.fb.group({
      answers: this.fb.array(questions)
    });
  }

  get answers(): FormArray {
    return this.testForm.get('answers') as FormArray;
  }

  submitForm(): void {
    if (this.testForm.valid) {
      this.isLoading = true;
      const answers = this.testForm.value.answers.map((a: any, i: number) => {
        if (this.test?.questions[i].type === 'MULTIPLE_CHOICE') {
          const selectedOptionIds = a.options
            .map((checked: boolean, j: number) => checked ? this.test?.questions[i].options[j].id : null)
            .filter((id: string | null) => id !== null);
          return {
            questionId: a.questionId,
            selectedOptionIds: selectedOptionIds
          };
        } else {
          return {
            questionId: a.questionId,
            selectedOptionIds: [a.optionId]
          };
        }
      });

      this.testService.submitTest(this.test!.id, { answers }).subscribe({
        next: () => {
          this.notification.success('Success', 'Test submitted successfully');
          this.router.navigate(['/tests']);
          this.isLoading = false;
        },
        error: (error : any) => {
          this.notification.error('Error', extractErrorMessage(error, 'Failed to submit test'));
          this.isLoading = false;
        }
      });
    }
  }
}
