import { User, UserManager, UserManagerSettings, WebStorageStateStore } from 'oidc-client-ts';

// Google OIDC configuration settings with Authorization Code flow + PKCE
export const oidcSettings: UserManagerSettings = {  
  // Use the base Google accounts URL as authority
  authority: 'https://accounts.google.com',
  
  // Complete metadata for Google endpoints
  metadata: {
    issuer: 'https://accounts.google.com',
    authorization_endpoint: 'https://accounts.google.com/o/oauth2/v2/auth',
    token_endpoint: 'https://oauth2.googleapis.com/token',
    userinfo_endpoint: 'https://openidconnect.googleapis.com/v1/userinfo',
    jwks_uri: 'https://www.googleapis.com/oauth2/v3/certs'
  },
    client_id: import.meta.env.VITE_GOOGLE_CLIENT_ID || '',
  client_secret: import.meta.env.VITE_GOOGLE_CLIENT_SECRET || '',
  redirect_uri: `${window.location.origin}/authentication/callback`,
  post_logout_redirect_uri: `${window.location.origin}`,  
  response_type: 'code',
  scope: 'openid profile email',
    // PKCE (Proof Key for Code Exchange) is enabled by default for code flow
  
  automaticSilentRenew: true,
  includeIdTokenInSilentRenew: true,
  silent_redirect_uri: `${window.location.origin}/authentication/silent-callback`,
  userStore: new WebStorageStateStore({ store: window.localStorage }),
  
  // Google specific settings
  filterProtocolClaims: true,
  loadUserInfo: true,
};

// Create the UserManager with our settings
export const userManager = new UserManager(oidcSettings);

// Helper function to handle authentication errors
export const handleLoginError = (error: Error): void => {
  console.error('OIDC login error:', error);
};

// In development mode, you can get the mock user for testing
export const getMockUser = () => {
  const mockId = Math.random().toString(36).substr(2, 9);
  const expiresIn = 3600; // 1 hour
  
  return {
    id_token: 'mock_id_token',
    session_state: 'mock_session',
    access_token: 'mock_token_' + mockId,
    refresh_token: 'mock_refresh_token',
    token_type: 'Bearer',
    scope: 'openid profile email',
    profile: {
      sub: 'user_' + mockId,
      name: 'Test User',
      email: 'testuser@example.com',
      picture: 'https://lh3.googleusercontent.com/a/default-user',
    },
    expires_at: new Date().getTime() / 1000 + expiresIn,
    expires_in: expiresIn,
    state: 'mock_state',
    // Additional properties needed to satisfy the User type
    expired: false,
    scopes: ['openid', 'profile', 'email'],
    toStorageString: function() { return JSON.stringify(this); }
  } as unknown as User;
};

// Constants for auth-related URL paths
export const AuthPaths = {
  LOGIN: '/authentication/login',
  CALLBACK: '/authentication/callback',
  SILENT_CALLBACK: '/authentication/silent-callback',
  LOGOUT: '/authentication/logout',
};
