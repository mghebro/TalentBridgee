export interface User {
  id: number;
  email: string;
  role: string;
  isVerified: boolean;
  firstName: string;
  lastName: string;
  token?: string;
}
