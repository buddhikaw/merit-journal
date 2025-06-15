import React, { useState, ChangeEvent } from 'react';
import { useNavigate } from 'react-router-dom';
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
  Autocomplete,
  Chip
} from '@mui/material';
// Simple date input instead of DatePicker to avoid dependency issues
import SaveIcon from '@mui/icons-material/Save';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { createJournalEntry, selectJournalLoading, selectTags } from '../features/journal/journalSlice';
import { AppDispatch } from '../app/store';

const CreateJournalEntryPage: React.FC = () => {
  const navigate = useNavigate();
  const dispatch = useDispatch<AppDispatch>();
  const loading = useSelector(selectJournalLoading);
  const availableTags = useSelector(selectTags);
  
  const [title, setTitle] = useState('');
  const [content, setContent] = useState('');
  const [entryDate, setEntryDate] = useState<string>(new Date().toISOString().substring(0, 10));
  const [tags, setTags] = useState<string[]>([]);
  const [newTag, setNewTag] = useState('');
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!title || !content || !entryDate) return;
    
    // Ensure entryDate is a proper ISO string with UTC timezone
    const entryDateUTC = new Date(entryDate).toISOString();
    
    await dispatch(createJournalEntry({
      title,
      content,
      entryDate: entryDateUTC,
      tags
    }));
    
    navigate('/');
  };

  const handleBack = () => {
    navigate('/');
  };

  const handleTagDelete = (tagToDelete: string) => {
    setTags(tags.filter(tag => tag !== tagToDelete));
  };

  const handleTagAdd = () => {
    if (newTag && !tags.includes(newTag)) {
      setTags([...tags, newTag]);
      setNewTag('');
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

      <Paper elevation={2} sx={{ p: 3 }}>
        <Typography variant="h4" component="h1" gutterBottom>
          Create New Entry
        </Typography>
        
        <Box component="form" onSubmit={handleSubmit} sx={{ mt: 3 }}>
          <Grid container spacing={3}>
            <Grid item xs={12} sm={8}>
              <TextField
                fullWidth
                label="Title"
                value={title}
                onChange={(e) => setTitle(e.target.value)}
                required
                variant="outlined"
              />
            </Grid>            <Grid item xs={12} sm={4}>
              <TextField
                fullWidth
                label="Entry Date"
                type="date"
                value={entryDate}
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
                <Autocomplete
                  freeSolo
                  options={availableTags.filter(tag => !tags.includes(tag))}
                  value={newTag}
                  onChange={(_, value) => setNewTag(value || '')}
                  onInputChange={(_, value) => setNewTag(value)}
                  sx={{ flexGrow: 1, mr: 1 }}
                  renderInput={(params) => (
                    <TextField {...params} label="Add a tag" variant="outlined" />
                  )}
                />
                <Button 
                  variant="contained"
                  onClick={handleTagAdd}
                  disabled={!newTag || tags.includes(newTag)}
                >
                  Add
                </Button>
              </Box>
              
              <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
                {tags.map(tag => (
                  <Chip
                    key={tag}
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
                {loading ? <CircularProgress size={24} /> : 'Save Entry'}
              </Button>
            </Grid>
          </Grid>
        </Box>
      </Paper>
    </Container>
  );
};

export default CreateJournalEntryPage;
