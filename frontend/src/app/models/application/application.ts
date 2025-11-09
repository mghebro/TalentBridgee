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
    appliedAt: Date;
}

export interface CreateApplicationRequest {
    vacancyId: number;
    coverLetter?: string;
}

export interface UpdateApplicationStatusRequest {
    status: string;
}

export interface AddReviewNoteRequest {
    note: string;
}

export enum ApplicationStatus {
    Submitted = 'Submitted',
    UnderReview = 'UnderReview',
    Shortlisted = 'Shortlisted',
    Interview = 'Interview',
    TestAssigned = 'TestAssigned',
    Offered = 'Offered',
    Hired = 'Hired',
    Rejected = 'Rejected',
    Withdrawn = 'Withdrawn',
}
