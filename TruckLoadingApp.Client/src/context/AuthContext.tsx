import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import authService from '../services/auth.service';
import { User, UserType } from '../types/auth.types';

interface AuthContextType {
  isAuthenticated: boolean;
  user: User | null;
  loading: boolean;
  login: (username: string, password: string) => Promise<void>;
  logout: () => void;
  registerShipper: (email: string, password: string, confirmPassword: string, firstName: string, lastName: string) => Promise<void>;
  registerTrucker: (email: string, password: string, confirmPassword: string, firstName: string, lastName: string, truckOwnerType: string) => Promise<void>;
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

  // Check if user is already logged in on component mount
  useEffect(() => {
    const checkAuth = () => {
      const isLoggedIn = authService.isLoggedIn();
      setIsAuthenticated(isLoggedIn);
      
      if (isLoggedIn) {
        // Get user data from localStorage
        const userStr = localStorage.getItem('user');
        if (userStr) {
          try {
            const userData = JSON.parse(userStr);
            setUser(userData as User);
          } catch (error) {
            console.error('Error parsing user data:', error);
          }
        }
      }
      
      setLoading(false);
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
    truckOwnerType: string
  ) => {
    try {
      setLoading(true);
      const response = await authService.registerTrucker({
        email,
        password,
        confirmPassword,
        firstName,
        lastName,
        userType: UserType.Trucker,
        truckOwnerType: truckOwnerType as any
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
          userType: UserType.Trucker,
          truckOwnerType: truckOwnerType as any,
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
    login,
    logout,
    registerShipper,
    registerTrucker,
    registerCompany,
    hasRole
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}; 