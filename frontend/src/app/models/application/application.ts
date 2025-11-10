import { Vacancy } from "../vacancy/vacancy";
import { User } from "../auth/user";

export interface Application {
  id: string;
  vacancyId: string;
  vacancy: Vacancy;
  userId: string;
  user: User;
  applicationDate: Date;
  status: string;
  resumeUrl: string;
  vacancyTitle: string;
  userFirstName: string;
  userLastName: string;
  userEmail: string;
  coverLetter: string;
}

export interface UpdateApplicationStatusRequest {
  status: string;
}

export interface AddReviewNoteRequest {
  note: string;
}