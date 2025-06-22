import { User } from 'oidc-client-ts';
import { userManager, getMockUser, handleLoginError, AuthPaths } from './authConfig';
import { store } from '../app/store';
import { loginStart, loginSuccess, loginFailure, logout } from '../features/auth/authSlice';

// Check if we're in development mode
const IS_DEV = import.meta.env.DEV || process.env.NODE_ENV === 'development';

// Use mock auth if no Google Client ID is provided or mock auth is explicitly enabled
const USE_MOCK_AUTH = IS_DEV && (
  !import.meta.env.VITE_GOOGLE_CLIENT_ID || 
  import.meta.env.VITE_USE_MOCK_AUTH === 'true'
);

/**
 * Authentication service for handling OIDC operations
 */
export const authService = {
  /**
   * Initialize auth - check if user is still logged in
   */
  async initializeAuth() {
    try {
      const user = await userManager.getUser();
      if (user && !user.expired) {
        store.dispatch(loginSuccess(user));
        return user;
      }
    } catch (error) {
      console.error('Error initializing auth:', error);
    }
    
    return null;
  },
  
  /**
   * Start the login process
   * In production, this redirects to the IDP
   * In development with mock auth, it directly logs the user in
   */
  async login() {
    store.dispatch(loginStart());
    
    try {
      if (USE_MOCK_AUTH) {
        // Development mode with mock auth
        await this.handleMockLogin();
      } else {
        // Production mode or dev without mock auth
        await userManager.signinRedirect();
      }
    } catch (error) {
      store.dispatch(loginFailure((error as Error).message));
      handleLoginError(error as Error);
    }
  },
  
  /**
   * Complete the login process after redirect from IDP
   */
  async completeLogin() {
    try {
      const user = await userManager.signinRedirectCallback();
      store.dispatch(loginSuccess(user));
      return user;
    } catch (error) {
      store.dispatch(loginFailure((error as Error).message));
      handleLoginError(error as Error);
      return null;
    }
  },
  
  /**
   * Complete silent refresh
   */
  async completeSilentLogin() {
    try {
      const user = await userManager.signinSilentCallback();
      return user;
    } catch (error) {
      handleLoginError(error as Error);
      return null;
    }
  },
  
  /**
   * Logout the user
   */
  async logout() {
    try {
      if (USE_MOCK_AUTH) {
        store.dispatch(logout());
      } else {
        const user = await userManager.getUser();
        if (user) {
          await userManager.signoutRedirect();
        }
        store.dispatch(logout());
      }
    } catch (error) {
      console.error('Logout error:', error);
      // Still clear the local state even if there was an error
      store.dispatch(logout());
    }
  },
  
  /**
   * Get the current user
   */
  async getUser(): Promise<User | null> {
    try {
      const user = await userManager.getUser();
      return user;
    } catch (error) {
      console.error('Error getting user:', error);
      return null;
    }
  },
  
  /**
   * Check if the user is authenticated
   */
  async isAuthenticated(): Promise<boolean> {
    const user = await this.getUser();
    return !!user && !user.expired;
  },
  
  /**
   * Handle mock login for development
   */
  async handleMockLogin() {
    try {
      // Short delay to simulate network request
      await new Promise(resolve => setTimeout(resolve, 1000));
      
      // Create a mock user object
      const mockUser = getMockUser();
      
      store.dispatch(loginSuccess(mockUser));
    } catch (error) {
      store.dispatch(loginFailure((error as Error).message));
    }
  },
};

export default authService;
