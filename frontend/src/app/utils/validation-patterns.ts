export const EMAIL_PATTERN = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;

export const PASSWORD_PATTERN =
  /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&._-])[A-Za-z\d@$!%*?&._-]{8,}$/;

export const NAME_PATTERN = /^[a-zA-ZÀ-ÿ' -]{2,}$/;

export const PHONE_PATTERN = /^[+()0-9\s-]{7,20}$/;

export const WEBSITE_PATTERN = /^(https?:\/\/)?([\w-]+\.)+[\w-]{2,}(\/[\w-./?%&=]*)?$/;

export const INTEGER_PATTERN = /^\d+$/;

export const DECIMAL_PATTERN = /^\d+(\.\d+)?$/;
