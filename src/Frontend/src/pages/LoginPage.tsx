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
import { loginFailure } from '../features/auth/authSlice';
import authService from '../services/authService';

const LoginPage: React.FC = () => {
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const handleLogin = async () => {
    setLoading(true);
    
    try {
      // Use authService to handle login
      await authService.login();
      
      // Navigation will happen in the callback or directly in the authService if mock auth is used
      // Set timeout to simulate loading for mock auth
      setTimeout(() => {
        navigate('/');
        setLoading(false);
      }, 1500);
    } catch (error) {
      dispatch(loginFailure((error as Error).message));
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
          <CardActions sx={{ padding: 2, flexDirection: 'column', gap: 1 }}>            <Button
              fullWidth
              variant="contained"
              color="primary"
              startIcon={<GoogleIcon />}
              onClick={() => handleLogin()}
              disabled={loading}
              sx={{
                bgcolor: '#4285F4',
                '&:hover': {
                  bgcolor: '#357ae8'
                }
              }}
            >
              Sign in with Google
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
