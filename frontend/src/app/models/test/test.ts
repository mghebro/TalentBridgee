import { Question } from './question';

export interface Test {
  id: string;
  title: string;
  description: string;
  organizationId: number;
  profession: string;
  durationMinutes: number;
  passingScore: number;
  difficulty: string;
  questions: Question[];
  questionCount?: number;
}

export interface TestForApplicationResponse {
  id: number;
  title: string;
  description: string;
  durationMinutes: number;
  questions: QuestionForUserResponse[];
  assignmentId: number;
  submissionId?: number;
}

export interface QuestionForUserResponse {
  id: number;
  questionText: string;
  questionType: string;
  points: number;
  orderNumber: number;
  timeLimitSeconds?: number;
  options: QuestionOptionForUserResponse[];
  submittedAnswer?: SubmissionAnswer;
}

export interface QuestionOptionForUserResponse {
  id: number;
  optionText: string;
}

export interface SubmitTestRequest {
  answers: SubmitAnswerRequest[];
}

export interface SubmitAnswerRequest {
  questionId: number;
  selectedOptionIds?: number[];
  answerText?: string;
  timeSpentSeconds: number;
}

export interface SubmissionAnswer {
  id: number;
  testSubmissionId: number;
  questionId: number;
  selectedOptionId?: number;
  answerText?: string;
  pointsAwarded?: number;
  isCorrect?: boolean;
  isTimedOut: boolean;
  answeredAt: Date;
  timeSpentSeconds: number;
}

export interface TestSubmissionResponse {
  id: number;
  testAssignmentId: number;
  submittedAt: Date;
  totalScore: number;
}

export interface CreateTestRequest {
  title: string;
  description: string;
  organizationId: string;
  profession: string;
  durationMinutes: number;
  passingScore: number;
  difficulty: string;
  questions: Question[];
}
