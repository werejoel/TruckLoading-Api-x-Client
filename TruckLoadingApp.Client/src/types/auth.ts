export type UserRole = 'Admin' | 'Shipper' | 'Trucker' | 'Company';

export interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  userType: UserRole;
  roles: UserRole[];
} 