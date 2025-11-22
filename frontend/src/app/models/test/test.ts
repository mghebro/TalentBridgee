import { Question } from "./question";

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