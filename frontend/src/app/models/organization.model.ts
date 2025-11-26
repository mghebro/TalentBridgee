export interface Organization {
  id: string;
  name: string;
  logoUrl: string;
}

export interface OrganizationList {
  id: number;
  name: string;
  typeName: string;
  contactEmail: string;
  website: string;
  isActive: boolean;
  address: string;
  phoneNumber: string;
  logo?: string;
  logoUrl?: string;
}

export interface CreateOrganizationRequest {
  name: string;
  type: string;
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

export interface OrganizationStatistics {
  totalVacancies: number;
  activeVacancies: number;
  closedVacancies: number;
  totalApplications: number;
  pendingApplications: number;
  reviewedApplications: number;
  totalHires: number;
  lastVacancyPosted?: Date;
  lastApplicationReceived?: Date;
  averageTimeToHire?: number;
}

export interface OrganizationDetails extends Organization {
  type: string;
  typeName?: string;
  address: string;
  contactEmail: string;
  phoneNumber: string;
  description: string;
  website: string;
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
  logo?: string;
  statistics?: OrganizationStatistics;
}

export enum OrganizationType {
  BUSINESS_COMPANY = 'BUSINESS_COMPANY',
  EDUCATION = 'EDUCATION',
  GOV = 'GOV',
  HEALTHCARE = 'HEALTHCARE',
  NON_GOV = 'NON_GOV',
  OTHERS_ASSOCIATIONS = 'OTHERS_ASSOCIATIONS',
}
