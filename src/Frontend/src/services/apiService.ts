import { selectAccessToken } from '../features/auth/authSlice';

// TEMPORARY: Flag to indicate development mode where authentication can be bypassed
const BYPASS_AUTH = true;

// Will be set when store is created (to avoid circular dependency)
let storeRef: any = null;

// Function to set the store reference
export const setStoreRef = (store: any) => {
  storeRef = store;
};

/**
 * Base API URL for all requests
 */
const API_BASE_URL = 'https://localhost:5001/api'; // Local development URL (HTTPS)

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
    // Only get token if not bypassing auth and store is available
    const accessToken = BYPASS_AUTH ? null : 
      (storeRef ? selectAccessToken(storeRef.getState()) : null);
    
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
    }

    try {
      const response = await fetch(url, requestOptions);
      
      if (!response.ok) {
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
