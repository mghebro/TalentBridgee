export interface OrganizationList {
  id: number;
  name: string;
  address: string;
  contactEmail: string;
  phoneNumber: string;
  logo: string;
  website: string;
  type: OrganizationType;
  typeName: string;
  exactTypeValue: number;
  exactTypeName: string;
  activeVacancies: number;
  totalApplications: number;
  createdAt: Date;
  isActive: boolean;
}

export interface OrganizationDetails {
  id: number;
  name: string;
  address: string;
  contactEmail: string;
  phoneNumber: string;
  description: string;
  logo: string;
  website: string;
  type: OrganizationType;
  typeName: string;
  exactTypeValue: number;
  exactTypeName: string;
  statistics: OrganizationStatistics;
  createdDate: Date;
  updatedDate?: Date;
  isActive: boolean;
}

export interface OrganizationStatistics {
  totalVacancies: number;
  activeVacancies: number;
  closedVacancies: number;
  totalApplications: number;
  pendingApplications: number;
  reviewedApplications: number;
  totalTests: number;
  completedTests: number;
  averageTestScore: number;
  scheduledInterviews: number;
  completedInterviews: number;
  totalHires: number;
  averageTimeToHire: number;
  lastVacancyPosted?: Date;
  lastApplicationReceived?: Date;
}

export enum OrganizationType {
  BUSINESS_COMPANY = 'BUSINESS_COMPANY',
  EDUCATION = 'EDUCATION',
  HEALTHCARE = 'HEALTHCARE',
  NON_GOV = 'NON_GOV',
  GOV = 'GOV',
  OTHERS_ASSOCIATIONS = 'OTHERS_ASSOCIATIONS',
}

export interface CreateOrganizationRequest {
  name: string;
  type: OrganizationType;
  address: string;
  contactEmail: string;
  phoneNumber?: string;
  description?: string;
  website?: string;
}

export interface UpdateOrganizationRequest {
  name?: string;
  address?: string;
  contactEmail?: string;
  phoneNumber?: string;
  description?: string;
  website?: string;
  deleteLogo?: boolean;
}

export interface OrganizationFilterRequest {
  search?: string;
  type?: OrganizationType;
  location?: string;
  isActive?: boolean;
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortOrder?: string;
}
