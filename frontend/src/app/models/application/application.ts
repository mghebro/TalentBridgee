export interface Application {
  id: number;
  vacancyId: number;
  vacancyTitle: string;
  userId: number;
  userEmail: string;
  userFirstName: string;
  userLastName: string;
  status: string;
  coverLetter?: string;
  appliedAt: string;
}

export interface UpdateApplicationStatusRequest {
  status: string;
}

export interface AddReviewNoteRequest {
  note: string;
}