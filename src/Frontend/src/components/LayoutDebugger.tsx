import React from 'react';
import { Box, Typography, Alert, Paper, Button } from '@mui/material';
import Layout from './Layout';
import { useNavigate } from 'react-router-dom';

const LayoutDebugger: React.FC = () => {
  const navigate = useNavigate();
  
  return (
    <Box sx={{ p: 3 }}>
      <Paper sx={{ p: 3, mb: 3 }}>
        <Typography variant="h4">Layout Debugger</Typography>
        <Typography variant="body1" sx={{ mt: 1 }}>
          This component will attempt to render the Layout component below.
        </Typography>
        <Alert severity="info" sx={{ my: 2 }}>
          If you don't see the layout below, there might be a rendering error in the Layout component.
        </Alert>
        <Box sx={{ mt: 2, display: 'flex', gap: 2 }}>
          <Button variant="outlined" onClick={() => navigate('/debug')}>
            Go to Debug Page
          </Button>
          <Button variant="outlined" onClick={() => navigate('/redux-debug')}>
            Go to Redux Debug
          </Button>
          <Button variant="outlined" onClick={() => navigate('/api-test')}>
            Go to API Test
          </Button>
        </Box>
      </Paper>
      
      <Paper sx={{ p: 3, mb: 3 }}>
        <Typography variant="h5" sx={{ mb: 2 }}>Layout Component:</Typography>
        <Box sx={{ border: '2px dashed #ccc', p: 2 }}>
          <React.Suspense fallback={<Typography>Loading layout...</Typography>}>
            <ErrorBoundary>
              <Layout />
            </ErrorBoundary>
          </React.Suspense>
        </Box>
      </Paper>
    </Box>
  );
};

// Simple error boundary component
class ErrorBoundary extends React.Component<
  {children: React.ReactNode}, 
  {hasError: boolean, error: Error | null}
> {
  constructor(props: {children: React.ReactNode}) {
    super(props);
    this.state = { hasError: false, error: null };
  }

  static getDerivedStateFromError(error: Error) {
    return { hasError: true, error };
  }

  render() {
    if (this.state.hasError) {
      return (
        <Alert severity="error" sx={{ my: 2 }}>
          <Typography variant="h6">Error in Layout Component:</Typography>
          <pre style={{ whiteSpace: 'pre-wrap' }}>
            {this.state.error?.message || 'Unknown error'}
            {this.state.error?.stack && (
              <Box component="span" sx={{ display: 'block', mt: 1, fontSize: '0.8em' }}>
                {this.state.error.stack}
              </Box>
            )}
          </pre>
        </Alert>
      );
    }

    return this.props.children;
  }
}

export default LayoutDebugger;
