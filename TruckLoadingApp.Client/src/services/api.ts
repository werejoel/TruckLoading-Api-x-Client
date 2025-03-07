import axios, { AxiosInstance, AxiosRequestConfig } from 'axios';

// Create a base API instance
const baseURL = 'https://localhost:7021/api'; // Updated to use HTTPS and port 7021 for local development

const api: AxiosInstance = axios.create({
  baseURL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Flag to track if a token refresh is in progress
let isRefreshing = false;
// Queue of requests to retry after token refresh
let refreshSubscribers: Array<(token: string) => void> = [];

// Function to add request to queue
const subscribeTokenRefresh = (callback: (token: string) => void) => {
  refreshSubscribers.push(callback);
};

// Function to process queue with new token
const onTokenRefreshed = (newToken: string) => {
  refreshSubscribers.forEach(callback => callback(newToken));
  refreshSubscribers = [];
};

// Function to reject all requests in queue
const onRefreshError = (error: any) => {
  refreshSubscribers.forEach(callback => callback(''));
  refreshSubscribers = [];
  return Promise.reject(error);
};

// Add a request interceptor to include the auth token in requests
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Add a response interceptor to handle token expiration
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;
    
    // If the error is 401 (Unauthorized) and we haven't already tried to refresh the token
    if (error.response?.status === 401 && !originalRequest._retry) {
      // Mark this request as retried
      originalRequest._retry = true;
      
      // If we're not already refreshing the token
      if (!isRefreshing) {
        isRefreshing = true;
        
        try {
          // Try to refresh the token
          const refreshToken = localStorage.getItem('refreshToken');
          const token = localStorage.getItem('token');
          
          if (!refreshToken || !token) {
            // No refresh token available, redirect to login
            localStorage.removeItem('token');
            localStorage.removeItem('refreshToken');
            localStorage.removeItem('user');
            
            // Only redirect if we're not already on the login page
            if (!window.location.pathname.includes('/login')) {
              window.location.href = '/login';
            }
            return Promise.reject(error);
          }
          
          console.log('Attempting to refresh token with:', { 
            tokenPrefix: token.substring(0, 10) + '...', 
            refreshTokenPrefix: refreshToken.substring(0, 10) + '...' 
          });
          
          const response = await axios.post(`${baseURL}/auth/refresh-token`, {
            token,
            refreshToken
          });
          
          if (response.data && response.data.token && response.data.refreshToken) {
            console.log('Token refresh successful, storing new tokens');
            
            // Save the new tokens
            localStorage.setItem('token', response.data.token);
            localStorage.setItem('refreshToken', response.data.refreshToken);
            
            // Update the Authorization header for the original request
            originalRequest.headers.Authorization = `Bearer ${response.data.token}`;
            
            // Process any queued requests with the new token
            onTokenRefreshed(response.data.token);
            
            // Reset refreshing flag
            isRefreshing = false;
            
            // Retry the original request
            return api(originalRequest);
          } else {
            console.error('Token refresh response did not contain new tokens:', response.data);
            throw new Error('Invalid token refresh response');
          }
        } catch (refreshError) {
          console.error('Error refreshing token:', refreshError);
          
          // If refresh token fails, redirect to login
          localStorage.removeItem('token');
          localStorage.removeItem('refreshToken');
          localStorage.removeItem('user');
          
          // Process any queued requests with error
          onRefreshError(refreshError);
          
          // Reset refreshing flag
          isRefreshing = false;
          
          // Only redirect if we're not already on the login page
          if (!window.location.pathname.includes('/login')) {
            window.location.href = '/login';
          }
          return Promise.reject(refreshError);
        }
      } else {
        // If we're already refreshing, add this request to the queue
        return new Promise(resolve => {
          subscribeTokenRefresh((token: string) => {
            if (token) {
              // If we got a new token, update the header and retry
              originalRequest.headers.Authorization = `Bearer ${token}`;
              resolve(api(originalRequest));
            } else {
              // If refresh failed, reject
              resolve(Promise.reject(error));
            }
          });
        });
      }
    }
    
    return Promise.reject(error);
  }
);

export default api; 