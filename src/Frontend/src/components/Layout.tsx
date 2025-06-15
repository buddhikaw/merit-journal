import React from 'react';
import { Outlet, useNavigate } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
import {
  AppBar,
  Box,
  Toolbar,
  Typography,
  Button,
  Container,
  Drawer,
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
  Divider,
} from '@mui/material';
import HomeIcon from '@mui/icons-material/Home';
import LogoutIcon from '@mui/icons-material/Logout';
import AddIcon from '@mui/icons-material/Add';
import { logout, selectIsAuthenticated, selectUser } from '../features/auth/authSlice';

// TEMPORARY: Flag to bypass authentication
const BYPASS_AUTH = true;

const Layout: React.FC = () => {
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const isAuthenticated = useSelector(selectIsAuthenticated);
  const user = useSelector(selectUser);
  
  const handleLogout = () => {
    dispatch(logout());
    navigate('/login');
  };

  const handleNavigateHome = () => {
    navigate('/');
  };

  const handleNavigateNew = () => {
    navigate('/entries/new');
  };
  
  // Use authenticated or bypass auth
  const showContent = isAuthenticated || BYPASS_AUTH;
  
  return (
    <Box sx={{ display: 'flex', minHeight: '100vh' }}>
      <AppBar position="fixed" sx={{ zIndex: (theme) => theme.zIndex.drawer + 1 }}>
        <Toolbar>
          <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
            Merit Journal
          </Typography>
          {showContent && (
            <>
              {isAuthenticated ? (
                <Button color="inherit" onClick={handleLogout}>
                  Logout
                </Button>
              ) : (
                <Typography variant="body1">
                  Development Mode (Auth Bypassed)
                </Typography>
              )}
            </>
          )}
        </Toolbar>
      </AppBar>
      
      <Drawer
        variant="permanent"
        sx={{
          width: 240,
          flexShrink: 0,
          [`& .MuiDrawer-paper`]: { width: 240, boxSizing: 'border-box' },
        }}
      >
        <Toolbar />
        <Box sx={{ overflow: 'auto' }}>
          <List>
            <ListItem button onClick={handleNavigateHome}>
              <ListItemIcon>
                <HomeIcon />
              </ListItemIcon>
              <ListItemText primary="Home" />
            </ListItem>
            {isAuthenticated && (
              <ListItem button onClick={handleNavigateNew}>
                <ListItemIcon>
                  <AddIcon />
                </ListItemIcon>
                <ListItemText primary="New Entry" />
              </ListItem>
            )}
          </List>
          <Divider />
          {isAuthenticated && user && (
            <Box sx={{ p: 2 }}>
              <Typography variant="subtitle1">
                {user.profile?.name}
              </Typography>
              <Typography variant="body2" color="textSecondary">
                {user.profile?.email}
              </Typography>
              <Button 
                variant="outlined"
                size="small"
                startIcon={<LogoutIcon />}
                onClick={handleLogout}
                sx={{ mt: 1 }}
              >
                Logout
              </Button>
            </Box>
          )}
        </Box>
      </Drawer>
      
      <Box component="main" sx={{ flexGrow: 1, p: 3 }}>
        <Toolbar />
        <Container maxWidth="lg">
          <Outlet />
        </Container>
      </Box>
    </Box>
  );
};

export default Layout;
