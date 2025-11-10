import { Organization } from "../organization.model";

export interface Vacancy {
  id: string;
  title: string;
  description: string;
  requirements: string;
  location: string;
  employmentType: string;
  salary: number;
  vacancyStatus: string;
  postedAt: Date;
  expiresAt: Date;
  organizationId: string;
  organization: Organization;
}

export interface CreateVacancyRequest {
  title: string;
  organizationId: string;
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

export interface VacancyDetails extends Vacancy {
  responsibilities: string;
  profession: string;
  industry: string;
  experienceLevel: string;
  salaryMin?: number;
  salaryMax?: number;
  salaryCurrency: string;
  isRemote: boolean;
}