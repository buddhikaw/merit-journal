import React, { useState } from 'react';
import { Box, Typography, Paper, Alert, List, ListItem, ListItemText, CircularProgress, Button } from '@mui/material';
import { journalService } from '../services/journalService';
import { JournalEntry } from '../types/journal';

const ApiTestComponent: React.FC = () => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [entries, setEntries] = useState<JournalEntry[]>([]);
  
  const testApiConnection = async () => {
    setLoading(true);
    setError(null);
    
    try {
      console.log('Testing API connection...');
      const result = await journalService.getAll();
      console.log('API response:', result);
      setEntries(result);
    } catch (err) {
      console.error('API Error:', err);
      setError(err instanceof Error ? err.message : 'Unknown error occurred');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Paper sx={{ p: 3, m: 2 }}>
      <Typography variant="h4">API Connection Test</Typography>
      <Box sx={{ my: 2 }}>
        <Button 
          variant="contained" 
          onClick={testApiConnection}
          disabled={loading}
        >
          Test API Connection
        </Button>
      </Box>
      
      {loading && (
        <Box sx={{ display: 'flex', justifyContent: 'center', my: 2 }}>
          <CircularProgress />
        </Box>
      )}
      
      {error && (
        <Alert severity="error" sx={{ my: 2 }}>
          API Error: {error}
        </Alert>
      )}
      
      {entries.length > 0 && (
        <>
          <Typography variant="h6" sx={{ mt: 3 }}>Journal Entries:</Typography>
          <List>
            {entries.map(entry => (
              <ListItem key={entry.id}>
                <ListItemText 
                  primary={entry.title} 
                  secondary={`${new Date(entry.entryDate).toLocaleDateString()} - ${entry.content.substring(0, 50)}...`} 
                />
              </ListItem>
            ))}
          </List>
        </>
      )}
      
      {entries.length === 0 && !loading && !error && (
        <Alert severity="info" sx={{ my: 2 }}>
          No entries found. Click the button to test the API connection.
        </Alert>
      )}
    </Paper>
  );
};

export default ApiTestComponent;
