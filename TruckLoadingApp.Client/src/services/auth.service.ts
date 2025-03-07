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
    try {
      console.log('Logging in user:', loginData.username);
      const response = await api.post<AuthResponse>('/auth/login', loginData);
      
      if (!response.data.success) {
        console.error('Login failed:', response.data.message);
        throw new Error(response.data.message || 'Login failed');
      }
      
      if (!response.data.token || !response.data.refreshToken) {
        console.error('Login response missing tokens');
        throw new Error('Invalid login response: missing tokens');
      }
      
      console.log('Login successful, storing tokens and user data');
      
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
      
      return response.data;
    } catch (error) {
      console.error('Error in login method:', error);
      throw error;
    }
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
    try {
      console.log('Verifying token...');
      // Use a simple endpoint that requires authentication
      await api.get('/auth/verify');
      console.log('Token verification successful');
      return true;
    } catch (error) {
      console.error('Token verification failed:', error);
      throw error;
    }
  }

  // Refresh token method
  async refreshToken(token: string, refreshToken: string): Promise<AuthResponse> {
    try {
      console.log('Auth service refreshToken called with token and refresh token');
      
      // Use axios directly to avoid circular dependencies with api.ts
      const axios = (await import('axios')).default;
      const baseURL = 'https://localhost:7021/api';
      
      const response = await axios.post<AuthResponse>(`${baseURL}/auth/refresh-token`, {
        token,
        refreshToken
      });
      
      console.log('Refresh token response received:', { 
        success: response.data.success,
        hasToken: !!response.data.token,
        hasRefreshToken: !!response.data.refreshToken
      });
      
      if (response.data.token && response.data.refreshToken) {
        console.log('Storing new tokens from refresh response');
        
        // Store new tokens in localStorage
        localStorage.setItem('token', response.data.token);
        localStorage.setItem('refreshToken', response.data.refreshToken);
        
        // Update user data if needed
        const userStr = localStorage.getItem('user');
        if (userStr) {
          try {
            const userData = JSON.parse(userStr);
            // You might want to update user data here if the API returns updated user info
            localStorage.setItem('user', JSON.stringify(userData));
          } catch (parseError) {
            console.error('Error parsing user data during token refresh:', parseError);
          }
        }
      } else {
        console.error('Refresh token response missing tokens:', response.data);
        throw new Error('Invalid refresh token response');
      }
      
      return response.data;
    } catch (error) {
      console.error('Error in auth service refreshToken:', error);
      throw error;
    }
  }
}

export default new AuthService();