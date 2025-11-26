export interface Profile {
  id: number;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
  isVerified: boolean;
  createdAt: Date;
  phoneNumber?: string;
  profilePictureUrl?: string;
  bio?: string;
  skills?: string;
  cvPdfUrl?: string;
  gender?: string;
  educations: Education[];
  experiences: Experience[];
}

export interface Education {
  id: number;
  institution: string;
  degree: string;
  fieldOfStudy: string;
  startDate: Date;
  endDate?: Date;
  isCurrent: boolean;
  description?: string;
}

export interface Experience {
  id: number;
  company: string;
  position: string;
  location?: string;
  startDate: Date;
  endDate?: Date;
  isCurrent: boolean;
  description?: string;
}

export interface UpdateProfileRequest {
  firstName?: string;
  lastName?: string;
  phoneNumber?: string;
  bio?: string;
  skills?: string;
  gender?: string;
}

export interface AddEducationRequest {
  institution: string;
  degree: string;
  fieldOfStudy: string;
  startDate: Date;
  endDate?: Date;
  isCurrent: boolean;
  description?: string;
}

export interface AddExperienceRequest {
  company: string;
  position: string;
  location?: string;
  startDate: Date;
  endDate?: Date;
  isCurrent: boolean;
  description?: string;
}

