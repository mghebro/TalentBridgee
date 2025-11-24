import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Subject, timer, Subscription } from 'rxjs';
import { takeUntil, finalize } from 'rxjs/operators';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzRadioModule } from 'ng-zorro-antd/radio';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { NzProgressModule } from 'ng-zorro-antd/progress';
import { NzSpinModule } from 'ng-zorro-antd/spin';

import { TestService } from '../../../services/test/test.service';
import { ApplicationService } from '../../../services/application/application.service';
import {
  TestForApplicationResponse,
  QuestionForUserResponse,
  SubmitAnswerRequest,
} from '../../../models/test/test';

interface QuestionWithState extends QuestionForUserResponse {
  isLocked: boolean;
  selectedOptionId?: number;
  answerText?: string;
  timeSpentSeconds?: number;
}

@Component({
  selector: 'app-test-player',
  standalone: true,
  imports: [
    NzRadioModule,
    FormsModule,
    CommonModule,
    ReactiveFormsModule,
    NzCardModule,
    NzButtonModule,
    NzRadioModule,
    NzCheckboxModule,
    NzInputModule,
    NzProgressModule,
    NzSpinModule,
  ],
  templateUrl: './test-player.component.html',
  styleUrls: ['./test-player.component.scss'],
})
export class TestPlayerComponent implements OnInit, OnDestroy {
  isLoading = true;
  isSubmitting = false;
  test?: TestForApplicationResponse;
  questions: QuestionWithState[] = [];
  currentQuestionIndex = 0;

  private timerSubscription?: Subscription;
  private destroy$ = new Subject<void>();
  timeLeft = 0;
  timePercentage = 100;

  private assignmentId!: number;
  private submissionId?: number;
  private applicationId!: string;
  private isFinishing = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private testService: TestService,
    private applicationService: ApplicationService,
    private notification: NzNotificationService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    const applicationId = this.route.snapshot.paramMap.get('id');
    if (!applicationId) {
      this.notification.error('Error', 'Application ID not found.');
      this.router.navigate(['/']);
      return;
    }
    this.applicationId = applicationId;

    this.applicationService.getTestForApplication(applicationId).subscribe({
      next: (response) => {
        if (!response || !response.assignmentId) {
          this.notification.error('Error', 'Test data is invalid or missing assignment ID.');
          this.router.navigate(['/my-applications']);
          return;
        }
        const test = response;
        this.test = test;
        this.assignmentId = test.assignmentId;
        this.submissionId = test.submissionId ?? undefined;
        this.questions = test.questions.map((q) => ({
          ...q,
          isLocked: !!q.submittedAnswer,
          selectedOptionId: q.submittedAnswer?.selectedOptionId,
          answerText: q.submittedAnswer?.answerText,
          timeSpentSeconds: 0,
        }));
        this.isLoading = false;

        if (this.questions.length > 0) {
          this.loadQuestion(0);
        } else {
          this.notification.info('Info', 'This test has no questions.');
          this.finishTest();
        }
      },
      error: (err) => {
        this.isLoading = false;
        this.notification.error(
          'Error loading test',
          err?.error?.message || 'Could not load the test.'
        );
        this.router.navigate(['/my-applications']);
      },
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    if (this.timerSubscription) {
      this.timerSubscription.unsubscribe();
    }
  }

  get currentQuestion(): QuestionWithState {
    return this.questions[this.currentQuestionIndex];
  }

  private loadQuestion(index: number): void {
    if (index >= this.questions.length) {
      this.finishTest();
      return;
    }
    this.currentQuestionIndex = index;
    if (this.currentQuestion.isLocked) {
      this.loadQuestion(index + 1);
    } else {
      this.startTimerForCurrentQuestion();
    }
  }

  private startTimerForCurrentQuestion(): void {
    if (this.timerSubscription) {
      this.timerSubscription.unsubscribe();
    }

    const timeLimit = this.currentQuestion.timeLimitSeconds ?? 60;
    this.timeLeft = timeLimit;

    this.timerSubscription = timer(0, 1000)
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => {
          if (!this.currentQuestion.isLocked) {
            this.handleTimeout();
          }
        })
      )
      .subscribe(() => {
        this.timePercentage = (this.timeLeft / timeLimit) * 100;
        if (this.timeLeft > 0) {
          this.timeLeft--;
        } else {
          if (this.timerSubscription) {
            this.timerSubscription.unsubscribe();
          }
        }
        this.cdr.detectChanges();
      });
  }

  private handleTimeout(): void {
    if (this.currentQuestion.isLocked) return; 

    this.notification.warning('Time Up!', `The time for this question has expired.`);
    this.currentQuestion.isLocked = true;
    this.isSubmitting = true;
    const elapsed = this.currentQuestion.timeLimitSeconds ?? 60;
    this.currentQuestion.timeSpentSeconds = elapsed;

    this.testService
      .handleQuestionTimeout(this.assignmentId, this.currentQuestion.id)
      .pipe(
        finalize(() => {
          this.isSubmitting = false;
          this.loadQuestion(this.currentQuestionIndex + 1);
        })
      )
      .subscribe({
        next: (res) => {
          this.notification.success(
            'Auto-Submitted',
            'Question marked as incorrect due to timeout.'
          );
          this.currentQuestion.submittedAnswer = res.data ?? undefined;
        },
        error: (err) => {
          this.notification.error('Error', err?.error?.message || 'Failed to record timeout.');
        },
      });
  }

  submitAnswer(): void {
    if (this.currentQuestion.isLocked) return;

    this.isSubmitting = true;
    this.currentQuestion.isLocked = true;
    if (this.timerSubscription) {
      this.timerSubscription.unsubscribe();
    }

    const timeLimit = this.currentQuestion.timeLimitSeconds ?? 60;
    const timeSpent = Math.min(timeLimit, Math.max(0, timeLimit - this.timeLeft));
    this.currentQuestion.timeSpentSeconds = timeSpent;

    const selectedOptionIds =
      this.currentQuestion.selectedOptionId !== undefined
        ? [this.currentQuestion.selectedOptionId]
        : [];

    const answerRequest: SubmitAnswerRequest = {
      questionId: this.currentQuestion.id,
      selectedOptionIds,
      answerText: this.currentQuestion.answerText,
      timeSpentSeconds: timeSpent,
    };

    this.testService
      .submitAnswer(this.assignmentId, this.currentQuestion.id, answerRequest)
      .pipe(
        finalize(() => {
          this.isSubmitting = false;
          this.loadQuestion(this.currentQuestionIndex + 1);
        })
      )
      .subscribe({
        next: (res) => {
          this.notification.success('Submitted', 'Your answer has been recorded.');
          this.currentQuestion.submittedAnswer = res.data ?? undefined;
        },
        error: (err) => {
          this.notification.error('Error', err?.error?.message || 'Failed to submit answer.');
        },
      });
  }

  onOptionChange(optionId: number | null): void {
    if (!this.currentQuestion.isLocked) {
      this.currentQuestion.selectedOptionId = optionId ?? undefined;
    }
  }

  private buildFinalAnswers(): SubmitAnswerRequest[] {
    return this.questions.map((q) => {
      const selectedId = q.selectedOptionId ?? q.submittedAnswer?.selectedOptionId ?? undefined;
      const selectedOptionIds = selectedId !== undefined ? [selectedId] : [];
      const answerText = q.answerText ?? q.submittedAnswer?.answerText;
      const timeSpentSeconds = q.timeSpentSeconds ?? q.submittedAnswer?.timeSpentSeconds ?? 0;

      return {
        questionId: q.id,
        selectedOptionIds,
        answerText,
        timeSpentSeconds,
      };
    });
  }

  private finishTest(): void {
    if (this.isFinishing) {
      return;
    }
    this.isFinishing = true;

    if (this.timerSubscription) {
      this.timerSubscription.unsubscribe();
    }

    const answers = this.buildFinalAnswers();

    this.applicationService
      .submitTest(this.applicationId, { answers })
      .pipe(
        finalize(() => {
          this.router.navigate(['/my-applications']);
        })
      )
      .subscribe({
        next: () => {
          this.notification.success('Test Complete', 'Thank you for completing the test.');
        },
        error: (err) => {
          this.notification.error(
            'Unable to finalize test',
            err?.error?.message || 'We saved your answers but could not finalize the test.'
          );
        },
      });
  }
}
