import api from './api';
import { 
  LoginRequest, 
  ShipperRegisterRequest, 
  TruckerRegisterRequest, 
  CompanyRegisterRequest,
  AuthResponse,
  TruckOwnerType,
  UserType
} from '../types/auth.types';

class AuthService {
  // Login method
  async login(loginData: LoginRequest): Promise<AuthResponse> {
    const response = await api.post<AuthResponse>('/auth/login', loginData);
    
    if (response.data.token) {
      // Parse the JWT token to get additional claims
      const tokenParts = response.data.token.split('.');
      let userData: any = {
        id: response.data.userId,
        username: response.data.username,
        roles: response.data.roles
      };
      
      // Try to extract additional claims from the token
      if (tokenParts.length === 3) {
        try {
          const tokenPayload = JSON.parse(atob(tokenParts[1]));
          
          // Extract company-specific claims if present
          if (response.data.roles.includes('Company')) {
            if (tokenPayload.CompanyName) {
              userData = {
                ...userData,
                companyName: tokenPayload.CompanyName
              };
            }
            
            if (tokenPayload.CompanyRegistrationNumber) {
              userData = {
                ...userData,
                companyRegistrationNumber: tokenPayload.CompanyRegistrationNumber
              };
            }
          }
        } catch (error) {
          console.error('Error parsing JWT token:', error);
        }
      }
      
      // Store tokens in localStorage
      localStorage.setItem('token', response.data.token);
      localStorage.setItem('refreshToken', response.data.refreshToken);
      localStorage.setItem('user', JSON.stringify(userData));
    }
    
    return response.data;
  }
  
  // Logout method
  logout(): void {
    const userId = this.getCurrentUserId();
    if (userId) {
      // Call the logout API
      api.post('/auth/logout', { userId }).catch(error => {
        console.error('Error during logout:', error);
      });
    }
    
    // Remove tokens from localStorage
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
  }
  
  // Register shipper method
  async registerShipper(registerData: ShipperRegisterRequest): Promise<AuthResponse> {
    // Map frontend field names to backend field names
    const backendData = {
      Username: registerData.email,
      Password: registerData.password,
      ConfirmPassword: registerData.confirmPassword || registerData.password, // Use confirmPassword if provided
      FirstName: registerData.firstName,
      LastName: registerData.lastName,
      UserType: 1 // Numeric value for Shipper (1)
    };
    
    const response = await api.post<AuthResponse>('/auth/register/shipper', backendData);
    
    if (response.data.token) {
      // Store tokens in localStorage
      localStorage.setItem('token', response.data.token);
      localStorage.setItem('refreshToken', response.data.refreshToken);
      localStorage.setItem('user', JSON.stringify({
        id: response.data.userId,
        username: response.data.username,
        roles: response.data.roles
      }));
    }
    
    return response.data;
  }
  
  // Register trucker method with enhanced driver information
  async registerTrucker(
    email: string,
    password: string,
    confirmPassword: string,
    firstName: string,
    lastName: string,
    truckOwnerType: TruckOwnerType,
    licenseNumber: string,
    licenseExpiryDate: string | Date,
    experience?: number | null
  ): Promise<AuthResponse> {
    // Convert TruckOwnerType enum to numeric value
    // Individual = 1, Company = 2
    const truckOwnerTypeValue = truckOwnerType === TruckOwnerType.Individual ? 1 : 2;
    
    // Create the DTO object matching what the API expects
    const data = {
      Username: email,
      Password: password,
      ConfirmPassword: confirmPassword,
      FirstName: firstName,
      LastName: lastName,
      UserType: 2, // Numeric value for Trucker (2)
      TruckOwnerType: truckOwnerTypeValue,
      LicenseNumber: licenseNumber,
      LicenseExpiryDate: licenseExpiryDate,
      Experience: experience
    };
    
    // Log the request data for debugging
    console.log('Sending trucker registration data:', data);
    
    const response = await api.post<AuthResponse>('/auth/register/trucker', data);
    
    if (response.data.token) {
      // Store tokens in localStorage
      localStorage.setItem('token', response.data.token);
      localStorage.setItem('refreshToken', response.data.refreshToken);
      localStorage.setItem('user', JSON.stringify({
        id: response.data.userId,
        username: response.data.username,
        roles: response.data.roles
      }));
    }
    
    return response.data;
  }
  
  // Register company method
  async registerCompany(registerData: CompanyRegisterRequest): Promise<AuthResponse> {
    // Map frontend field names to backend field names
    const backendData = {
      Username: registerData.email,
      Password: registerData.password,
      ConfirmPassword: registerData.confirmPassword || registerData.password, // Use confirmPassword if provided
      FirstName: registerData.firstName,
      LastName: registerData.lastName,
      UserType: 3, // Numeric value for Company (3)
      CompanyName: registerData.companyName,
      CompanyAddress: registerData.companyAddress,
      CompanyRegistrationNumber: registerData.companyRegistrationNumber,
      CompanyContact: registerData.companyContact
    };
    
    const response = await api.post<AuthResponse>('/auth/register/company', backendData);
    
    if (response.data.token) {
      // Store tokens in localStorage
      localStorage.setItem('token', response.data.token);
      localStorage.setItem('refreshToken', response.data.refreshToken);
      localStorage.setItem('user', JSON.stringify({
        id: response.data.userId,
        username: response.data.username,
        roles: response.data.roles,
        companyName: registerData.companyName,
        companyRegistrationNumber: registerData.companyRegistrationNumber
      }));
    }
    
    return response.data;
  }
  
  // Check if user is logged in
  isLoggedIn(): boolean {
    return !!localStorage.getItem('token');
  }
  
  // Get current user ID
  getCurrentUserId(): string | null {
    const userStr = localStorage.getItem('user');
    if (!userStr) return null;
    
    try {
      const user = JSON.parse(userStr);
      return user.id;
    } catch (error) {
      return null;
    }
  }
  
  // Get current user roles
  getCurrentUserRoles(): string[] {
    const userStr = localStorage.getItem('user');
    if (!userStr) return [];
    
    try {
      const user = JSON.parse(userStr);
      return user.roles || [];
    } catch (error) {
      return [];
    }
  }
  
  // Check if user has a specific role
  hasRole(role: string): boolean {
    const roles = this.getCurrentUserRoles();
    return roles.includes(role);
  }

  // Verify if the current token is valid
  async verifyToken(): Promise<boolean> {
    await api.get('/auth/verify');
    return true;
  }
}

export default new AuthService();