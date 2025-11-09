export interface User {
    id: number;
    email: string;
    appleId?: string;
    googleId?: string;
    role: string;
    isVerified: boolean;
    authProvider: string;
    lastName: string;
    firstName: string;
    details?: UserDetails;
}

export interface UserDetails {
    userId: number;
    phoneNumber?: string;
    profilePictureUrl?: string;
    gender?: string;
    bio?: string;
    cvPdfUrl?: string;
    skills?: string;
    educations: any[]; // Replace with Education model
    experiences: any[]; // Replace with Experience model
}

export interface UpdateUserRequest {
    bio?: string;
    avatar?: string;
    gender?: string;
    lastName?: string;
    firstName?: string;
    phoneNumber?: string;
}

export interface UpdateUserResponse {
    user: User;
}
