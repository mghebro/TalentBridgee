import { QuestionOption } from "./question-option";

export interface Question {
  id: string;
  text: string;
  type: 'SINGLE_CHOICE' | 'MULTIPLE_CHOICE';
  options: QuestionOption[];
}
