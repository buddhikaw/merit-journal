import { logout } from '../features/auth/authSlice';
import authService from './authService';

// Authentication is now enabled
const BYPASS_AUTH = false;

// TODO: Implement token refresh mechanism to handle token expiration more gracefully
// Currently, expired tokens will just cause a logout and redirect to login page

// Will be set when store is created (to avoid circular dependency)
let storeRef: any = null;

// Function to set the store reference
export const setStoreRef = (store: any) => {
  storeRef = store;
};

/**
 * Base API URL for all requests
 */
const API_BASE_URL = import.meta.env.VITE_API_URL || 'https://localhost:5001/api';

/**
 * Options for API requests
 */
interface RequestOptions {
  method?: 'GET' | 'POST' | 'PUT' | 'DELETE';
  body?: any;
  headers?: Record<string, string>;
}

/**
 * API service for making requests to the backend
 */
export const apiService = {
  /**
   * Make a request to the API
   * @param endpoint - The API endpoint to request
   * @param options - Request options
   * @returns The response data
   */  async request<T>(endpoint: string, options: RequestOptions = {}): Promise<T> {
    let accessToken = null;
    
    // Only get token if not bypassing auth
    if (!BYPASS_AUTH) {
      // Get the current user from auth service
      const user = await authService.getUser();
      if (user && !user.expired) {
        accessToken = user.access_token;
      }
    }
    
    const url = `${API_BASE_URL}${endpoint}`;
    const headers: Record<string, string> = {
      'Content-Type': 'application/json',
      ...options.headers,
    };

    // Add authorization header if token is available and not bypassing auth
    if (accessToken) {
      headers.Authorization = `Bearer ${accessToken}`;
    }

    const requestOptions: RequestInit = {
      method: options.method || 'GET',
      headers,
      credentials: 'include',
    };

    // Add body if provided
    if (options.body) {
      requestOptions.body = JSON.stringify(options.body);
    }    try {
      const response = await fetch(url, requestOptions);
      
      if (!response.ok) {
        // Handle 401 Unauthorized - logout the user
        if (response.status === 401 && !BYPASS_AUTH && storeRef) {
          // Clear any stored tokens
          localStorage.removeItem('accessToken');
          
          // Dispatch logout action to Redux
          storeRef.dispatch(logout());
          
          // Redirect to login page
          window.location.href = '/login';
          
          throw new Error('Session expired. Please login again.');
        }
        
        const errorData = await response.json().catch(() => ({}));
        throw new Error(
          errorData.message || `API request failed with status ${response.status}`
        );
      }
      
      // For DELETE operations or other operations that might not return data
      if (response.status === 204) {
        return {} as T;
      }
      
      return await response.json();
    } catch (error) {
      console.error('API request error:', error);
      throw error;
    }
  },

  // Convenience methods for different HTTP methods
  get<T>(endpoint: string, options: Omit<RequestOptions, 'method' | 'body'> = {}) {
    return this.request<T>(endpoint, { ...options, method: 'GET' });
  },
  
  post<T>(endpoint: string, body: any, options: Omit<RequestOptions, 'method' | 'body'> = {}) {
    return this.request<T>(endpoint, { ...options, method: 'POST', body });
  },
  
  put<T>(endpoint: string, body: any, options: Omit<RequestOptions, 'method' | 'body'> = {}) {
    return this.request<T>(endpoint, { ...options, method: 'PUT', body });
  },
  
  delete<T>(endpoint: string, options: Omit<RequestOptions, 'method' | 'body'> = {}) {
    return this.request<T>(endpoint, { ...options, method: 'DELETE' });
  }
};
