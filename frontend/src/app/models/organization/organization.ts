export interface Organization {
    id: number;
    name: string;
    address: string;
    contactEmail: string;
    phoneNumber: string;
    description: string;
    logo: string;
    website: string;
    type: string;
    typeName: string;
    exactTypeValue: number;
    exactTypeName: string;
    statistics: OrganizationStatistics;
    createdDate: Date;
    updatedDate?: Date;
    isActive: boolean;
    activeVacancies: number;
    totalApplications: number;
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
