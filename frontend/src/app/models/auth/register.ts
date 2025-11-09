import { CreateOrganizationRequest } from '../organization.model';

export interface Register {
    email: string;
    password?: string;
    lastName: string;
    firstName: string;
    desiredRole: string;
    organizationId?: number;
    organizationDetails?: CreateOrganizationRequest;
}
