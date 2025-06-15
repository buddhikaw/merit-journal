import React, { useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
import {
  Container,
  Typography,
  Box,
  Button,
  Paper,
  Chip,
  Grid,
  CircularProgress,
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import { 
  fetchJournalEntryById, 
  selectCurrentEntry, 
  selectJournalLoading,
  deleteJournalEntry
} from '../features/journal/journalSlice';
import { AppDispatch } from '../app/store';
import { format } from 'date-fns';

const JournalEntryPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const dispatch = useDispatch<AppDispatch>();
  const entry = useSelector(selectCurrentEntry);
  const loading = useSelector(selectJournalLoading);

  useEffect(() => {
    if (id) {
      dispatch(fetchJournalEntryById(parseInt(id)));
    }
  }, [id, dispatch]);

  const handleBack = () => {
    navigate('/');
  };

  const handleEdit = () => {
    navigate(`/entries/${id}/edit`);
  };

  const handleDelete = async () => {
    if (id && window.confirm('Are you sure you want to delete this journal entry?')) {
      await dispatch(deleteJournalEntry(parseInt(id)));
      navigate('/');
    }
  };

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (!entry) {
    return (
      <Container maxWidth="md" sx={{ mt: 4 }}>
        <Typography variant="h5">Entry not found</Typography>
        <Button 
          startIcon={<ArrowBackIcon />}
          onClick={handleBack}
          sx={{ mt: 2 }}
        >
          Back to entries
        </Button>
      </Container>
    );
  }

  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 3 }}>
        <Button 
          startIcon={<ArrowBackIcon />}
          onClick={handleBack}
        >
          Back
        </Button>
        <Box>
          <Button 
            startIcon={<EditIcon />}
            onClick={handleEdit}
            sx={{ mr: 1 }}
          >
            Edit
          </Button>
          <Button 
            startIcon={<DeleteIcon />}
            color="error"
            onClick={handleDelete}
          >
            Delete
          </Button>
        </Box>
      </Box>

      <Paper elevation={2} sx={{ p: 3, mb: 3 }}>
        <Typography variant="overline" color="textSecondary">
          {format(new Date(entry.entryDate), 'MMMM d, yyyy')}
        </Typography>
        <Typography variant="h4" component="h1" gutterBottom>
          {entry.title}
        </Typography>
        
        <Box sx={{ mt: 4, whiteSpace: 'pre-wrap' }}>
          {entry.content}
        </Box>
        
        {entry.tags && entry.tags.length > 0 && (
          <Box sx={{ mt: 4 }}>
            {entry.tags.map((tag) => (
              <Chip 
                key={tag.id} 
                label={tag.name} 
                sx={{ mr: 1, mb: 1 }} 
              />
            ))}
          </Box>
        )}
      </Paper>

      {entry.images && entry.images.length > 0 && (
        <Box sx={{ mt: 3 }}>
          <Typography variant="h6" gutterBottom>Images</Typography>
          <Grid container spacing={2}>
            {entry.images.map((image) => (
              <Grid item xs={12} sm={6} md={4} key={image.id}>
                <Paper elevation={2}>
                  <Box
                    component="img"
                    src={image.imageUrl}
                    alt={image.caption || 'Journal image'}
                    sx={{ 
                      width: '100%',
                      height: 200,
                      objectFit: 'cover'
                    }}
                  />
                  {image.caption && (
                    <Box sx={{ p: 2 }}>
                      <Typography variant="body2">{image.caption}</Typography>
                    </Box>
                  )}
                </Paper>
              </Grid>
            ))}
          </Grid>
        </Box>
      )}
    </Container>
  );
};

export default JournalEntryPage;
