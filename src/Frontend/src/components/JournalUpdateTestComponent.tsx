import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { Box, Button, Paper, Typography, CircularProgress } from '@mui/material';
import { fetchJournalEntries, updateJournalEntry, selectAllJournalEntries, selectJournalLoading } from '../features/journal/journalSlice';
import { JournalEntry } from '../types/journal';
import { AppDispatch } from '../app/store';

/**
 * A component to test journal entry update functionality
 */
const JournalUpdateTestComponent: React.FC = () => {
  const dispatch = useDispatch<AppDispatch>();
  const entries = useSelector(selectAllJournalEntries);
  const loading = useSelector(selectJournalLoading);

  const handleFetchEntries = () => {
    dispatch(fetchJournalEntries());
  };

  const handleUpdateEntry = (entry: JournalEntry) => {
    const updatedEntry = {
      title: `${entry.title} (Updated)`,
      content: `${entry.content} - Updated at ${new Date().toLocaleTimeString()}`,
      entryDate: entry.entryDate,
      tags: entry.tags.map(tag => tag.name)
    };

    dispatch(updateJournalEntry({ id: entry.id, entry: updatedEntry }));
  };

  return (
    <Paper sx={{ p: 3, m: 2 }}>
      <Typography variant="h5" gutterBottom>Journal Update Test</Typography>

      <Button 
        variant="contained" 
        onClick={handleFetchEntries}
        disabled={loading}
        sx={{ mb: 2 }}
      >
        {loading ? <CircularProgress size={24} /> : 'Fetch Journal Entries'}
      </Button>

      {loading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', mt: 2 }}>
          <CircularProgress />
        </Box>
      ) : (
        <Box>
          <Typography variant="subtitle1" gutterBottom>
            Found {entries.length} entries
          </Typography>
          
          {entries.map((entry) => (
            <Box key={entry.id} sx={{ mb: 2, p: 2, border: '1px solid #ddd', borderRadius: 1 }}>
              <Typography variant="h6">{entry.title}</Typography>
              <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
                {new Date(entry.entryDate).toLocaleDateString()}
              </Typography>
              <Typography variant="body2" sx={{ mb: 1 }}>
                {entry.content.substring(0, 100)}...
              </Typography>
              <Box sx={{ mt: 1 }}>
                <Button 
                  variant="outlined"
                  onClick={() => handleUpdateEntry(entry)}
                  size="small"
                >
                  Update This Entry
                </Button>
              </Box>
            </Box>
          ))}
        </Box>
      )}
    </Paper>
  );
};

export default JournalUpdateTestComponent;
