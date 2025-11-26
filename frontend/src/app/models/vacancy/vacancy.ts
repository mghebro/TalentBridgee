export interface Vacancy {
  id: number;
  organizationId: number;
  organizationName: string | null;
  organizationLogo: string | null;
  title: string;
  description: string;
  profession: string;
  industry: string;
  employmentType: string;
  employmentTypeName: string;
  experienceLevel: string;
  experienceLevelName: string;
  salaryMin?: number;
  salaryMax?: number;
  salaryCurrency?: string;
  salaryRange?: string;
  location: string;
  isRemote: boolean;
  status: string;
  statusName: string;
  applicationDeadline: string;
  publishedAt?: string | null;
  viewCount: number;
  applicationCount: number;
  createdAt: string;
  testId?: number | null;
}

export interface VacancyDetails extends Vacancy {
  requirements: string;
  responsibilities: string;
  organizationWebsite?: string | null;
  createdBy: number;
  createdByName: string;
  updatedAt?: string | null;
  statistics?: VacancyStatistics;
  tests: VacancyTest[];
  hasApplied?: boolean;
}

export interface VacancyStatistics {
  viewCount: number;
  totalApplications: number;
  pendingApplications: number;
  reviewedApplications: number;
  shortlistedApplications: number;
  rejectedApplications: number;
  testsAssigned: number;
  testsCompleted: number;
  averageTestScore: number;
  interviewsScheduled: number;
  interviewsCompleted: number;
  daysActive: number;
  daysRemaining: number;
}

export interface VacancyTest {
  id: number;
  title: string;
  duration: number;
  questionCount: number;
  isActive: boolean;
}

export interface CreateVacancyRequest {
  title: string;
  organizationId: number;
  description: string;
  requirements: string;
  responsibilities: string;
  profession: string;
  industry: string;
  employmentType: string;
  experienceLevel: string;
  salaryMin?: number;
  salaryMax?: number;
  salaryCurrency: string;
  location: string;
  isRemote: boolean;
  status: string;
  applicationDeadline: Date;
}

export interface UpdateVacancyRequest extends CreateVacancyRequest {}
