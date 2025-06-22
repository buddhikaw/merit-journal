import React, { useEffect, useState } from 'react';
import { Navigate } from 'react-router-dom';
import { Box, CircularProgress, Typography } from '@mui/material';
import authService from '../services/authService';

const AuthCallback: React.FC = () => {
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  useEffect(() => {
    const processCallback = async () => {
      try {
        // Log the raw URL for debugging purposes
        console.log('Auth callback URL:', window.location.href);
        
        const user = await authService.completeLogin();
        console.log('Login completed successfully', user ? 'User authenticated' : 'No user object returned');
        setIsLoading(false);
      } catch (error) {
        console.error('Error during callback processing:', error);
        
        // Enhanced error logging for OIDC errors
        if (typeof error === 'object' && error !== null) {
          // Log additional details that might be available
          console.error('Error details:', {
            name: (error as Error).name,
            message: (error as Error).message,
            stack: (error as Error).stack,
            // @ts-ignore - Check for OIDC specific error properties
            error_description: (error as any).error_description,
            // @ts-ignore
            state: (error as any).state
          });
        }
        
        setError((error as Error).message);
        setIsLoading(false);
      }
    };

    processCallback();
  }, []);

  // Show loading spinner while processing the callback
  if (isLoading) {
    return (
      <Box
        sx={{
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          justifyContent: 'center',
          height: '100vh',
        }}
      >
        <CircularProgress size={60} />
        <Typography variant="h6" sx={{ mt: 2 }}>
          Completing login...
        </Typography>
      </Box>
    );
  }
  // Show error if there was a problem
  if (error) {
    return (
      <Box
        sx={{
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          justifyContent: 'center',
          height: '100vh',
          padding: 3,
        }}
      >
        <Typography variant="h5" color="error" gutterBottom>
          Authentication Error
        </Typography>
        
        <Typography variant="body1" sx={{ mt: 2, textAlign: 'center', maxWidth: '600px' }}>
          {error}
        </Typography>
        
        <Box sx={{ mt: 4, bgcolor: '#f5f5f5', p: 2, borderRadius: 1, maxWidth: '600px' }}>
          <Typography variant="subtitle2" gutterBottom>
            Common Solutions:
          </Typography>
          
          <Typography variant="body2" component="ul" sx={{ pl: 2 }}>
            <li>Verify that your Google OAuth client is configured as "Web application" type</li>
            <li>Check that <code>http://localhost:3000/authentication/callback</code> is in your authorized redirect URIs</li>
            <li>Ensure your Client ID is correctly set in .env.local</li>
            <li>Clear your browser cookies and localStorage</li>
          </Typography>
          
          {error.includes('client_secret') && (
            <Typography variant="body2" sx={{ mt: 2, color: 'error.main' }}>
              <strong>Note:</strong> The "client_secret is missing" error typically means Google is expecting a client 
              secret when it shouldn't be for this flow. Make sure your OAuth client is set as "Web application", not "Other".
            </Typography>
          )}
        </Box>
        
        <Typography variant="body2" sx={{ mt: 3 }}>
          <a href="/authentication/login">Return to login</a>
        </Typography>
      </Box>
    );
  }

  // Redirect to the home page on successful login
  return <Navigate to="/" replace />;
};

export default AuthCallback;
