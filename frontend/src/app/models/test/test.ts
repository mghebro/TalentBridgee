export interface Test {
    id: number;
    organizationId: number;
    organizationName: string;
    title: string;
    description: string;
    profession: string;
    durationMinutes: number;
    passingScore: number;
    totalPoints: number;
    difficulty: string;
    createdAt: Date;
    questions: Question[];
}

export interface Question {
    id: number;
    questionText: string;
    questionType: string;
    points: number;
    orderNumber: number;
    options: QuestionOption[];
}

export interface QuestionOption {
    id: number;
    optionText: string;
    isCorrect: boolean;
}

export interface CreateTestRequest {
    organizationId: number;
    title: string;
    description: string;
    profession: string;
    durationMinutes: number;
    passingScore: number;
    difficulty: string;
    questions: CreateQuestionRequest[];
}

export interface CreateQuestionRequest {
    questionText: string;
    questionType: string;
    points: number;
    orderNumber: number;
    options: CreateQuestionOptionRequest[];
}

export interface CreateQuestionOptionRequest {
    optionText: string;
    isCorrect: boolean;
}

export interface AddQuestionRequest {
    questionText: string;
    questionType: string;
    points: number;
    orderNumber: number;
    options: CreateQuestionOptionRequest[];
}

export interface AssignTestRequest {
    applicationId: number;
    testId: number;
    dueDate: Date;
}

export interface SubmitTestRequest {
    answers: SubmissionAnswerRequest[];
}

export interface SubmissionAnswerRequest {
    questionId: number;
    selectedOptionIds?: number[]; // For multiple choice
    answerText?: string; // For short answer/essay
    fileUrl?: string; // For file upload
}

export interface GradeAnswerRequest {
    grade: number;
    feedback?: string;
}
