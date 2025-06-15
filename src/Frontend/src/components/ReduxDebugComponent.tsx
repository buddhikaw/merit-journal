import React from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { Box, Typography, Paper, Alert, Divider, Button } from '@mui/material';
import { RootState } from '../app/store';
import { loginSuccess } from '../features/auth/authSlice';

const ReduxDebugComponent: React.FC = () => {
  const dispatch = useDispatch();
  const authState = useSelector((state: RootState) => state.auth);
  const journalState = useSelector((state: RootState) => state.journal);
    // Mock user for testing auth
  const mockUser = {
    id_token: 'mock_id_token',
    access_token: 'mock_access_token',
    token_type: 'Bearer',
    profile: {
      sub: 'mock_user_123',
      name: 'Test User',
      email: 'test@example.com',
    },
    expires_at: Date.now() + 3600 * 1000, // 1 hour from now
    state: {},
    session_state: null,
    scope: 'openid profile email',
    toStorageString: () => '',
    expires_in: 3600,
    expired: false,
    scopes: ['openid', 'profile', 'email']
  };
  
  const simulateLogin = () => {
    dispatch(loginSuccess(mockUser as any));
  };
  
  return (
    <Paper sx={{ p: 3, m: 2 }}>
      <Typography variant="h4">Redux Store Debug</Typography>
      
      <Box sx={{ mt: 3 }}>
        <Typography variant="h6">Auth State:</Typography>
        <pre style={{ backgroundColor: '#f5f5f5', padding: '10px', overflow: 'auto' }}>
          {JSON.stringify(authState, null, 2)}
        </pre>
        
        <Button 
          variant="contained" 
          color="primary" 
          sx={{ mt: 1 }}
          onClick={simulateLogin}
        >
          Simulate Login
        </Button>
      </Box>
      
      <Divider sx={{ my: 3 }} />
      
      <Box>
        <Typography variant="h6">Journal State:</Typography>
        <pre style={{ backgroundColor: '#f5f5f5', padding: '10px', overflow: 'auto' }}>
          {JSON.stringify(journalState, null, 2)}
        </pre>
        
        {journalState.error && (
          <Alert severity="error" sx={{ mt: 2 }}>
            Journal Error: {journalState.error}
          </Alert>
        )}
      </Box>
    </Paper>
  );
};

export default ReduxDebugComponent;
