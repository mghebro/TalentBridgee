export interface ApiResponse<T> {
  data: T | null;
  status: number;
  message?: string;
}

export interface ServiceResult<T> {
  success: boolean;
  data: T | null;
  message?: string;
  errors?: string[];
  timestamp?: string;
}

export interface PaginatedResult<T> {
  items: T[];
  totalItems: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

