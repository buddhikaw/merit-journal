import React, { useState } from 'react';
import { useDispatch } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import {
  Container,
  Box,
  Typography,
  Button,
  Card,
  CardContent,
  CardActions,
  CircularProgress,
} from '@mui/material';
import GoogleIcon from '@mui/icons-material/Google';
import MicrosoftIcon from '@mui/icons-material/Window';
import { loginStart, loginSuccess, loginFailure } from '../features/auth/authSlice';
// For now, we'll mock the OIDC login since we're focusing on the API integration

const LoginPage: React.FC = () => {
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);

  const handleLogin = async (provider: 'google' | 'microsoft') => {
    setLoading(true);
    dispatch(loginStart());
    
    try {
      // For development/testing purposes only - this is a mock of the OIDC login flow
      // In a real app, this would redirect to the OIDC provider
      
      // Mock successful login after a short delay
      await new Promise(resolve => setTimeout(resolve, 1000));
      
      // Create a mock user object
      const mockUser = {
        access_token: 'mock_token_' + Math.random().toString(36).substr(2, 9),
        id_token: 'mock_id_token',
        profile: {
          sub: 'user_' + Math.random().toString(36).substr(2, 9),
          name: 'Test User',
          email: 'testuser@example.com',
        },
      };
      
      // Store the token in localStorage for the API service to use
      localStorage.setItem('accessToken', mockUser.access_token);
      
      dispatch(loginSuccess(mockUser));
      navigate('/');
    } catch (error) {
      dispatch(loginFailure((error as Error).message));
    } finally {
      setLoading(false);
    }
  };

  return (
    <Container maxWidth="sm">
      <Box sx={{ mt: 8, display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
        <Typography variant="h4" component="h1" gutterBottom>
          Merit Journal
        </Typography>
        
        <Typography variant="body1" align="center" color="textSecondary" paragraph>
          Sign in to record and reflect on your meritorious deeds
        </Typography>
        
        <Card variant="outlined" sx={{ width: '100%', mt: 4 }}>
          <CardContent>
            <Typography variant="h6" component="h2" gutterBottom>
              Sign In
            </Typography>
            <Typography variant="body2" color="textSecondary" paragraph>
              Please sign in using one of the following providers:
            </Typography>
          </CardContent>
          <CardActions sx={{ padding: 2, flexDirection: 'column', gap: 1 }}>
            <Button
              fullWidth
              variant="outlined"
              startIcon={<GoogleIcon />}
              onClick={() => handleLogin('google')}
              disabled={loading}
            >
              Sign in with Google
            </Button>
            
            <Button
              fullWidth
              variant="outlined"
              startIcon={<MicrosoftIcon />}
              onClick={() => handleLogin('microsoft')}
              disabled={loading}
            >
              Sign in with Microsoft
            </Button>
            
            {loading && (
              <Box sx={{ display: 'flex', justifyContent: 'center', mt: 2 }}>
                <CircularProgress size={24} />
              </Box>
            )}
          </CardActions>
        </Card>
      </Box>
    </Container>
  );
};

export default LoginPage;
