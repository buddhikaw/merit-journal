import React from 'react';
import { Box, Typography, Paper, List, ListItem, ListItemText, Button } from '@mui/material';
import { Link as RouterLink } from 'react-router-dom';

const DebugIndexPage: React.FC = () => {  const debugRoutes = [
    { path: '/debug', name: 'Basic Debug', description: 'Simple component to verify React rendering' },
    { path: '/api-test', name: 'API Test', description: 'Test API connection to backend' },
    { path: '/redux-debug', name: 'Redux Debug', description: 'Inspect Redux store state' },
    { path: '/layout-debug', name: 'Layout Debug', description: 'Debug the main Layout component' },
    { path: '/test', name: 'Test Page', description: 'Application test page' },
    { path: '/', name: 'Main App', description: 'Go to the main application' },
  ];

  return (
    <Box sx={{ p: 3, maxWidth: 800, mx: 'auto' }}>
      <Paper sx={{ p: 3 }}>
        <Typography variant="h4" gutterBottom>MeritJournal Debug Tools</Typography>
        <Typography variant="body1" paragraph>
          Use these debug tools to diagnose issues with the application.
        </Typography>
        
        <List>
          {debugRoutes.map((route) => (
            <Paper 
              key={route.path} 
              variant="outlined" 
              sx={{ mb: 2, p: 1 }}
            >
              <ListItem 
                component={RouterLink} 
                to={route.path}
                sx={{ 
                  color: 'inherit', 
                  textDecoration: 'none', 
                  display: 'block' 
                }}
              >
                <ListItemText 
                  primary={<Typography variant="h6">{route.name}</Typography>}
                  secondary={route.description} 
                />
                <Button variant="contained" size="small">
                  Go
                </Button>
              </ListItem>
            </Paper>
          ))}
        </List>
      </Paper>
    </Box>
  );
};

export default DebugIndexPage;
