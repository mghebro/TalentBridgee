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

export interface OrganizationDetails extends Organization {
  type: string;
  address: string;
  contactEmail: string;
  phoneNumber: string;
  description: string;
  website: string;
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
}

export enum OrganizationType {
  BUSINESS_COMPANY = 'BUSINESS_COMPANY',
  EDUCATION = 'EDUCATION',
  GOV = 'GOV',
  HEALTHCARE = 'HEALTHCARE',
  NON_GOV = 'NON_GOV',
  OTHERS_ASSOCIATIONS = 'OTHERS_ASSOCIATIONS',
}
