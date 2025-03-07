import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import authService from '../services/auth.service';
import { User, UserType, TruckOwnerType } from '../types/auth.types';

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

  // Function to refresh the token
  const refreshAuthToken = async () => {
    if (!isAuthenticated) return;
    
    const token = localStorage.getItem('token');
    const refreshToken = localStorage.getItem('refreshToken');
    
    if (!token || !refreshToken) return;
    
    try {
      console.log('Periodic token refresh initiated');
      const response = await authService.refreshToken(token, refreshToken);
      
      if (!response.success) {
        console.error('Periodic token refresh failed:', response.message);
        // Don't log out the user here, let the interceptor handle it
      } else {
        console.log('Periodic token refresh successful, tokens updated');
        
        // Update user data if needed
        if (user && response.userId === user.id) {
          // Keep the existing user data but update any fields that might have changed
          setUser(prevUser => {
            if (!prevUser) return null;
            return {
              ...prevUser,
              roles: response.roles || prevUser.roles
            };
          });
        }
      }
    } catch (error) {
      console.error('Error during periodic token refresh:', error);
      // Don't log out the user here, let the interceptor handle it
    }
  };

  // Set up periodic token refresh (every 10 minutes)
  useEffect(() => {
    if (isAuthenticated) {
      const refreshInterval = setInterval(refreshAuthToken, 10 * 60 * 1000); // 10 minutes
      
      return () => {
        clearInterval(refreshInterval);
      };
    }
  }, [isAuthenticated]);

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
              
              // Get tokens from localStorage
              const token = localStorage.getItem('token');
              const refreshToken = localStorage.getItem('refreshToken');
              
              if (!token || !refreshToken) {
                console.error('Missing tokens in localStorage');
                localStorage.removeItem('token');
                localStorage.removeItem('refreshToken');
                localStorage.removeItem('user');
                setIsAuthenticated(false);
                setUser(null);
                setError('Session expired. Please login again.');
                return;
              }
              
              try {
                // First try to verify the token without refreshing
                console.log('Verifying token...');
                await authService.verifyToken();
                console.log('Token is valid');
                setUser(userData as User);
                setIsAuthenticated(true);
                setError(null);
              } catch (verifyError) {
                console.error('Token verification failed:', verifyError instanceof Error ? verifyError.message : 'Unknown error');
                
                // If verification fails, try to refresh the token
                try {
                  console.log('Attempting to refresh token...');
                  const response = await authService.refreshToken(token, refreshToken);
                  
                  if (response.success) {
                    console.log('Token refreshed successfully');
                    // Update user data if needed
                    setUser(userData as User);
                    setIsAuthenticated(true);
                    setError(null);
                  } else {
                    console.error('Token refresh failed:', response.message);
                    // If refresh failed, clear everything
                    localStorage.removeItem('token');
                    localStorage.removeItem('refreshToken');
                    localStorage.removeItem('user');
                    setIsAuthenticated(false);
                    setUser(null);
                    setError('Session expired. Please login again.');
                  }
                } catch (refreshError) {
                  console.error('Error refreshing token:', refreshError instanceof Error ? refreshError.message : 'Unknown error');
                  // If refresh token fails, clear everything
                  localStorage.removeItem('token');
                  localStorage.removeItem('refreshToken');
                  localStorage.removeItem('user');
                  setIsAuthenticated(false);
                  setUser(null);
                  setError('Session expired. Please login again.');
                }
              }
            } catch (parseError) {
              console.error('Error parsing user data:', parseError instanceof Error ? parseError.message : 'Unknown error');
              localStorage.removeItem('user');
              setError('Error loading user data. Please login again.');
            }
          } else {
            console.error('No user data in localStorage');
            localStorage.removeItem('token');
            localStorage.removeItem('refreshToken');
            setIsAuthenticated(false);
            setUser(null);
          }
        } else {
          console.log('User is not logged in');
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
        username: email,
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
        experience
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
        username: email,
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
          roles: response.roles,
          companyName,
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
    if (!user || !user.roles) return false;
    return user.roles.includes(role);
  };

  return (
    <AuthContext.Provider
      value={{
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
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export default AuthContext;