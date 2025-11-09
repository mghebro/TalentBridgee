export interface VacancyFilter {
    search?: string;
    organizationId?: number;
    profession?: string;
    industry?: string;
    employmentType?: string;
    experienceLevel?: string;
    status?: string;
    location?: string;
    isRemote?: boolean;
    salaryMin?: number;
    salaryMax?: number;
    publishedFrom?: Date;
    publishedTo?: Date;
    page: number;
    pageSize: number;
    sortBy: string;
    sortOrder: string;
}
