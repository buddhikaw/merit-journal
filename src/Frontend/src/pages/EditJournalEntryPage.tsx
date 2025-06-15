import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
import {
  Container,
  Typography,
  Box,
  TextField,
  Button,
  Paper,
  Grid,
  CircularProgress,
  Chip
} from '@mui/material';
import SaveIcon from '@mui/icons-material/Save';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { 
  fetchJournalEntryById, 
  updateJournalEntry, 
  selectCurrentEntry, 
  selectJournalLoading 
} from '../features/journal/journalSlice';
import { AppDispatch } from '../app/store';
import { JournalEntry } from '../types/journal';

const EditJournalEntryPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const dispatch = useDispatch<AppDispatch>();
  const entry = useSelector(selectCurrentEntry) as JournalEntry;
  const loading = useSelector(selectJournalLoading);
  
  const [title, setTitle] = useState('');
  const [content, setContent] = useState('');
  const [entryDate, setEntryDate] = useState<string>('');  const [tags, setTags] = useState<string[]>([]);
  const [newTagName, setNewTagName] = useState('');
  
  useEffect(() => {
    if (id) {
      dispatch(fetchJournalEntryById(parseInt(id)));
    }
  }, [id, dispatch]);
    useEffect(() => {
    if (entry) {
      setTitle(entry.title || '');
      setContent(entry.content || '');
      setEntryDate(entry.entryDate || '');
        // Set tags from entry
      if (entry.tags && Array.isArray(entry.tags)) {
        setTags(entry.tags);
      } else {
        setTags([]);
      }
    }
  }, [entry]);

  if (!entry && !loading) {
    return (
      <Container maxWidth="md" sx={{ mt: 4 }}>
        <Typography variant="h5">Entry not found</Typography>
        <Button 
          startIcon={<ArrowBackIcon />}
          onClick={() => navigate('/')}
          sx={{ mt: 2 }}
        >
          Back to entries
        </Button>
      </Container>
    );
  }  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!id || !title || !content || !entryDate) return;
      // Filter out any empty tag names and ensure we have unique tags
    const validTags = tags
      .map(tag => tag.trim())
      .filter(tag => tag !== '')
      .filter((tag, index, self) => self.indexOf(tag) === index);
    
    const entryData = {
      title,
      content,
      entryDate,
      tags: validTags
    };
    
    try {
      await dispatch(updateJournalEntry({
        id: parseInt(id),
        entry: entryData
      }));
      
      navigate(`/entries/${id}`);
    } catch (error) {
      console.error('Error updating journal entry:', error);
      // You could add error handling UI here if needed
    }
  };

  const handleBack = () => {
    navigate(`/entries/${id}`);
  };
  const handleTagDelete = (tagToDelete: string) => {
    setTags(tags.filter(tag => tag !== tagToDelete));
  };

  const handleTagAdd = () => {
    if (newTagName && !tags.includes(newTagName)) {
      setTags([...tags, newTagName]);
      setNewTagName('');
    }
  };

  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 3 }}>
        <Button 
          startIcon={<ArrowBackIcon />}
          onClick={handleBack}
        >
          Back
        </Button>
      </Box>

      {loading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}>
          <CircularProgress />
        </Box>
      ) : (
        <Paper elevation={2} sx={{ p: 3 }}>
          <Typography variant="h4" component="h1" gutterBottom>
            Edit Entry
          </Typography>
          
          <Box component="form" onSubmit={handleSubmit} sx={{ mt: 3 }}>
            <Grid container spacing={3}>
              <Grid item xs={12}>
                <TextField
                  fullWidth
                  label="Title"
                  value={title}
                  onChange={(e) => setTitle(e.target.value)}
                  required
                  variant="outlined"
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  type="date"
                  fullWidth
                  label="Entry Date"
                  value={entryDate.substring(0, 10)}
                  onChange={(e) => setEntryDate(e.target.value)}
                  required
                  variant="outlined"
                  InputLabelProps={{ shrink: true }}
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  fullWidth
                  label="Content"
                  value={content}
                  onChange={(e) => setContent(e.target.value)}
                  required
                  multiline
                  rows={10}
                  variant="outlined"
                />
              </Grid>
              
              <Grid item xs={12}>
                <Typography variant="subtitle1" gutterBottom>
                  Tags
                </Typography>
                <Box sx={{ display: 'flex', mb: 2 }}>
                  <TextField
                    fullWidth
                    label="Add a tag"
                    value={newTagName}
                    onChange={(e) => setNewTagName(e.target.value)}
                    variant="outlined"
                    sx={{ mr: 1 }}
                  />
                  <Button 
                    variant="contained"
                    onClick={handleTagAdd}
                    disabled={!newTagName || tags.includes(newTagName)}
                  >
                    Add
                  </Button>
                </Box>
                
                <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>                  {tags.map((tag, index) => (
                    <Chip
                      key={`tag-${index}`}
                      label={tag}
                      onDelete={() => handleTagDelete(tag)}
                    />
                  ))}
                </Box>
              </Grid>
              
              <Grid item xs={12} sx={{ mt: 2 }}>
                <Button
                  type="submit"
                  variant="contained"
                  color="primary"
                  size="large"
                  startIcon={<SaveIcon />}
                  disabled={loading || !title || !content || !entryDate}
                >
                  {loading ? <CircularProgress size={24} /> : 'Save Changes'}
                </Button>
              </Grid>
            </Grid>
          </Box>
        </Paper>
      )}
    </Container>
  );
};

export default EditJournalEntryPage;
