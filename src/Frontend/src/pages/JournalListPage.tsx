import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
import {
  Container,
  Typography,
  Button,
  Card,
  CardContent,
  CardActions,
  Box,
  Grid,
  Chip,
  CircularProgress
} from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import { selectAllJournalEntries, selectJournalLoading, fetchJournalEntries } from '../features/journal/journalSlice';
import { AppDispatch } from '../app/store';
import { format } from 'date-fns';

const JournalListPage: React.FC = () => {
  const navigate = useNavigate();
  const dispatch = useDispatch<AppDispatch>();
  const entries = useSelector(selectAllJournalEntries);
  const loading = useSelector(selectJournalLoading);

  useEffect(() => {
    dispatch(fetchJournalEntries());
  }, [dispatch]);

  const handleCreateEntry = () => {
    navigate('/entries/new');
  };

  const handleViewEntry = (id: number) => {
    navigate(`/entries/${id}`);
  };

  // Group entries by date
  const entriesByDate = entries.reduce<Record<string, typeof entries>>((acc, entry) => {
    const date = entry.entryDate.substring(0, 10); // YYYY-MM-DD
    if (!acc[date]) {
      acc[date] = [];
    }
    acc[date].push(entry);
    return acc;
  }, {});

  // Sort dates in descending order
  const sortedDates = Object.keys(entriesByDate).sort((a, b) => new Date(b).getTime() - new Date(a).getTime());

  if (loading) {
    return (
      <Container sx={{ mt: 4, display: 'flex', justifyContent: 'center' }}>
        <CircularProgress />
      </Container>
    );
  }

  return (
    <Container maxWidth="md" sx={{ mt: 4, mb: 4 }}>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={4}>
        <Typography variant="h4" component="h1">
          Merit Journal Entries
        </Typography>
        <Button
          variant="contained"
          color="primary"
          startIcon={<AddIcon />}
          onClick={handleCreateEntry}
        >
          New Entry
        </Button>
      </Box>

      {entries.length === 0 ? (
        <Box textAlign="center" mt={8}>
          <Typography variant="h6" color="textSecondary" gutterBottom>
            You don't have any journal entries yet.
          </Typography>
          <Button
            variant="outlined"
            color="primary"
            startIcon={<AddIcon />}
            onClick={handleCreateEntry}
            sx={{ mt: 2 }}
          >
            Create Your First Entry
          </Button>
        </Box>
      ) : (
        sortedDates.map(date => (
          <Box key={date} mb={4}>
            <Typography variant="h6" component="h2" gutterBottom>
              {format(new Date(date), 'EEEE, MMMM d, yyyy')}
            </Typography>
            <Grid container spacing={3}>
              {entriesByDate[date].map(entry => (
                <Grid item xs={12} key={entry.id}>
                  <Card variant="outlined">
                    <CardContent>
                      <Typography variant="h6" component="h3" gutterBottom>
                        {entry.title}
                      </Typography>
                      <Typography variant="body2" color="textSecondary" gutterBottom>
                        Created: {format(new Date(entry.createdAt), 'h:mm a')}
                      </Typography>
                      <Typography
                        variant="body1"
                        sx={{
                          display: '-webkit-box',
                          overflow: 'hidden',
                          WebkitBoxOrient: 'vertical',
                          WebkitLineClamp: 3,
                          mb: 2
                        }}
                      >
                        {/* Render plain text preview of HTML content */}
                        {entry.content.replace(/<[^>]+>/g, ' ').substring(0, 150)}
                        {entry.content.length > 150 ? '...' : ''}
                      </Typography>
                      <Box display="flex" flexWrap="wrap" gap={1}>
                        {entry.tags?.map(tag => (
                          <Chip key={tag} label={tag} size="small" />
                        ))}
                      </Box>
                    </CardContent>
                    <CardActions>
                      <Button size="small" onClick={() => handleViewEntry(entry.id)}>
                        View
                      </Button>
                      <Button size="small" onClick={() => navigate(`/entries/${entry.id}/edit`)}>
                        Edit
                      </Button>
                    </CardActions>
                  </Card>
                </Grid>
              ))}
            </Grid>
          </Box>
        ))
      )}
    </Container>
  );
};

export default JournalListPage;
