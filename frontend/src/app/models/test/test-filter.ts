export interface TestFilter {
    page: number;
    pageSize: number;
    search?: string;
    organizationId?: number;
    difficulty?: string;
    sortBy?: string;
    sortOrder?: string;
}
