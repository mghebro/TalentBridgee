export interface VacancyList {
    id: number;
    organizationId: number;
    organizationName: string;
    organizationLogo: string;
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
    salaryCurrency: string;
    salaryRange: string;
    location: string;
    isRemote: boolean;
    status: string;
    statusName: string;
    applicationDeadline: Date;
    publishedAt?: Date;
    viewCount: number;
    applicationCount: number;
    createdAt: Date;
}

export interface VacancyDetails {
    id: number;
    organizationId: number;
    organizationName: string;
    organizationLogo: string;
    organizationWebsite: string;
    createdBy: number;
    createdByName: string;
    title: string;
    description: string;
    requirements: string;
    responsibilities: string;
    profession: string;
    industry: string;
    employmentType: string;
    employmentTypeName: string;
    experienceLevel: string;
    experienceLevelName: string;
    salaryMin?: number;
    salaryMax?: number;
    salaryCurrency: string;
    location: string;
    isRemote: boolean;
    status: string;
    statusName: string;
    applicationDeadline: Date;
    publishedAt?: Date;
    createdAt: Date;
    updatedAt?: Date;
    statistics: VacancyStatistics;
    tests: VacancyTest[];
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
    organizationId: number;
    title: string;
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
    applicationDeadline: Date;
}

export interface UpdateVacancyRequest {
    title?: string;
    description?: string;
    requirements?: string;
    responsibilities?: string;
    profession?: string;
    industry?: string;
    employmentType?: string;
    experienceLevel?: string;
    salaryMin?: number;
    salaryMax?: number;
    salaryCurrency?: string;
    location?: string;
    isRemote?: boolean;
    status?: string;
    applicationDeadline?: Date;
}
