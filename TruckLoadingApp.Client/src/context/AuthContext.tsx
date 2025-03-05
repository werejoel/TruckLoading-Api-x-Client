import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import authService from '../services/auth.service';
import { User, UserType } from '../types/auth.types';

interface AuthContextType {
  isAuthenticated: boolean;
  user: User | null;
  loading: boolean;
  error: string | null;
  login: (username: string, password: string) => Promise<void>;
  logout: () => void;
  registerShipper: (email: string, password: string, confirmPassword: string, firstName: string, lastName: string) => Promise<void>;
  registerTrucker: (email: string, password: string, confirmPassword: string, firstName: string, lastName: string, truckOwnerType: string, licenseNumber: string, licenseExpiryDate: string | Date, experience?: number | null, phoneNumber?: string) => Promise<void>;
  registerCompany: (email: string, password: string, confirmPassword: string, firstName: string, lastName: string, companyName: string, companyAddress: string, companyRegistrationNumber: string, companyContact: string) => Promise<void>;
  hasRole: (role: string) => boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

interface AuthProviderProps {
  children: ReactNode;
}

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  // Check if user is already logged in on component mount
  useEffect(() => {
    const checkAuth = async () => {
      try {
        console.log('Checking authentication status...');
        const isLoggedIn = authService.isLoggedIn();
        console.log('Is logged in:', isLoggedIn);
        
        if (isLoggedIn) {
          // Get user data from localStorage
          const userStr = localStorage.getItem('user');
          console.log('User data from localStorage:', userStr);
          
          if (userStr) {
            try {
              const userData = JSON.parse(userStr);
              console.log('Parsed user data:', userData);
              
              // Verify token is still valid
              const token = localStorage.getItem('token');
              if (token) {
                try {
                  // Make a test request to verify token
                  await authService.verifyToken();
                  setUser(userData as User);
                  setIsAuthenticated(true);
                  setError(null);
                } catch (error: unknown) {
                  console.error('Token verification failed:', error instanceof Error ? error.message : 'Unknown error');
                  // Token is invalid, clear everything
                  localStorage.removeItem('token');
                  localStorage.removeItem('refreshToken');
                  localStorage.removeItem('user');
                  setIsAuthenticated(false);
                  setUser(null);
                  setError('Session expired. Please login again.');
                }
              }
            } catch (error: unknown) {
              console.error('Error parsing user data:', error instanceof Error ? error.message : 'Unknown error');
              setError('Error loading user data. Please login again.');
            }
          }
        }
      } catch (error: unknown) {
        console.error('Error during auth check:', error instanceof Error ? error.message : 'Unknown error');
        setError('Error checking authentication status.');
      } finally {
        setLoading(false);
      }
    };
    
    checkAuth();
  }, []);

  // Login function
  const login = async (username: string, password: string) => {
    try {
      setLoading(true);
      const response = await authService.login({ username, password });
      
      if (response.success) {
        setIsAuthenticated(true);
        // Set user data
        const userData: User = {
          id: response.userId,
          username: response.username,
          email: response.username,
          firstName: '',
          lastName: '',
          userType: UserType.Shipper, // Default, will be updated when we fetch full user profile
          roles: response.roles,
          createdDate: new Date().toISOString()
        };
        setUser(userData);
      }
    } catch (error) {
      console.error('Login error:', error);
      throw error;
    } finally {
      setLoading(false);
    }
  };

  // Logout function
  const logout = () => {
    authService.logout();
    setIsAuthenticated(false);
    setUser(null);
  };

  // Register shipper function
  const registerShipper = async (
    email: string, 
    password: string, 
    confirmPassword: string,
    firstName: string, 
    lastName: string
  ) => {
    try {
      setLoading(true);
      const response = await authService.registerShipper({
        email,
        password,
        confirmPassword,
        firstName,
        lastName,
        userType: UserType.Shipper
      });
      
      if (response.success) {
        setIsAuthenticated(true);
        // Set user data
        const userData: User = {
          id: response.userId,
          username: response.username,
          email: response.username,
          firstName,
          lastName,
          userType: UserType.Shipper,
          roles: response.roles,
          createdDate: new Date().toISOString()
        };
        setUser(userData);
      }
    } catch (error) {
      console.error('Register shipper error:', error);
      throw error;
    } finally {
      setLoading(false);
    }
  };

  // Register trucker function
  const registerTrucker = async (
    email: string, 
    password: string, 
    confirmPassword: string,
    firstName: string, 
    lastName: string,
    truckOwnerType: string,
    licenseNumber: string,
    licenseExpiryDate: string | Date,
    experience?: number | null,
    phoneNumber?: string
  ) => {
    try {
      setLoading(true);
      const response = await authService.registerTrucker(
        email,
        password,
        confirmPassword,
        firstName,
        lastName,
        truckOwnerType as TruckOwnerType,
        licenseNumber,
        licenseExpiryDate,
        experience,
        phoneNumber
      );
      
      if (response.success) {
        setIsAuthenticated(true);
        // Set user data
        const userData: User = {
          id: response.userId,
          username: response.username,
          email: response.username,
          firstName,
          lastName,
          userType: UserType.Trucker,
          truckOwnerType: truckOwnerType as TruckOwnerType,
          roles: response.roles,
          createdDate: new Date().toISOString()
        };
        setUser(userData);
      }
    } catch (error) {
      console.error('Register trucker error:', error);
      throw error;
    } finally {
      setLoading(false);
    }
  };

  // Register company function
  const registerCompany = async (
    email: string, 
    password: string, 
    confirmPassword: string,
    firstName: string, 
    lastName: string, 
    companyName: string, 
    companyAddress: string, 
    companyRegistrationNumber: string, 
    companyContact: string
  ) => {
    try {
      setLoading(true);
      const response = await authService.registerCompany({
        email,
        password,
        confirmPassword,
        firstName,
        lastName,
        userType: UserType.Company,
        companyName,
        companyAddress,
        companyRegistrationNumber,
        companyContact
      });
      
      if (response.success) {
        setIsAuthenticated(true);
        // Set user data
        const userData: User = {
          id: response.userId,
          username: response.username,
          email: response.username,
          firstName,
          lastName,
          userType: UserType.Company,
          companyName,
          companyRegistrationNumber,
          roles: response.roles,
          createdDate: new Date().toISOString()
        };
        setUser(userData);
      }
    } catch (error) {
      console.error('Register company error:', error);
      throw error;
    } finally {
      setLoading(false);
    }
  };

  // Check if user has a specific role
  const hasRole = (role: string) => {
    return authService.hasRole(role);
  };

  const value = {
    isAuthenticated,
    user,
    loading,
    error,
    login,
    logout,
    registerShipper,
    registerTrucker,
    registerCompany,
    hasRole
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export default AuthContext;