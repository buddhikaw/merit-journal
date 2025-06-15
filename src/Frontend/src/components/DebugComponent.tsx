import React, { useEffect } from 'react';
import { Box, Typography, Paper, Button } from '@mui/material';

const DebugComponent: React.FC = () => {
  // Log when component mounts
  useEffect(() => {
    console.log('DebugComponent mounted');
    
    // Check Redux state
    try {
      const state = window.__REDUX_DEVTOOLS_EXTENSION__ && 
                   window.__REDUX_DEVTOOLS_EXTENSION__.connect();
      if (state) {
        console.log('Redux DevTools is available');
      }
    } catch (err) {
      console.error('Redux DevTools error:', err);
    }
  }, []);

  return (
    <Paper sx={{ p: 3, m: 2 }}>
      <Typography variant="h4">Debug Component</Typography>
      <Typography variant="body1" sx={{ mb: 2, mt: 1 }}>
        If you can see this, React rendering is working correctly!
      </Typography>
      <Box sx={{ mt: 2 }}>
        <Button 
          variant="contained" 
          onClick={() => {
            console.log('Debug button clicked');
            alert('UI interactions are working!');
          }}
        >
          Test Interactivity
        </Button>
      </Box>
    </Paper>
  );
};

// Add this to make TypeScript happy with the window.__REDUX_DEVTOOLS_EXTENSION__ property
declare global {
  interface Window {
    __REDUX_DEVTOOLS_EXTENSION__?: any;
  }
}

export default DebugComponent;
