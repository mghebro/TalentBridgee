export interface OrganizationFilter {
    search?: string;
    type?: string;
    location?: string;
    isActive?: boolean;
    page: number;
    pageSize: number;
    sortBy: string;
    sortOrder: string;
}
