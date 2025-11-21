export const extractErrorMessage = (error: unknown, fallback: string): string => {
  if (typeof error === 'string') {
    return error;
  }

  const err = error as any;
  return (
    err?.error?.message ||
    err?.error?.Message ||
    err?.error?.errors?.[0] ||
    err?.message ||
    fallback
  );
};

