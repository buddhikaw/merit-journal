import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit';
import { journalService } from '../../services/journalService';
import { JournalEntry, JournalEntryDto, Tag } from '../../types/journal';

interface JournalState {
  entries: JournalEntry[];
  currentEntry: JournalEntry | null;
  loading: boolean;
  error: string | null;
  // Store tag names as strings for convenience
  tags: string[];
}

// Initial state
const initialState: JournalState = {
  entries: [],
  currentEntry: null,
  loading: false,
  error: null,
  tags: [],
};

// Thunks
export const fetchJournalEntries = createAsyncThunk(
  'journal/fetchJournalEntries',
  async (_, { rejectWithValue }) => {
    try {
      return await journalService.getAll();
    } catch (error) {
      return rejectWithValue((error as Error).message);
    }
  }
);

export const fetchJournalEntryById = createAsyncThunk(
  'journal/fetchJournalEntryById',
  async (id: number, { rejectWithValue }) => {
    try {
      return await journalService.getById(id);
    } catch (error) {
      return rejectWithValue((error as Error).message);
    }
  }
);

export const createJournalEntry = createAsyncThunk(
  'journal/createJournalEntry',
  async (entry: JournalEntryDto, { rejectWithValue }) => {
    try {
      return await journalService.create(entry);
    } catch (error) {
      return rejectWithValue((error as Error).message);
    }
  }
);

export const updateJournalEntry = createAsyncThunk(
  'journal/updateJournalEntry',
  async (params: { id: number, entry: JournalEntryDto }, { rejectWithValue }) => {
    try {
      return await journalService.update(params.id, params.entry);
    } catch (error) {
      return rejectWithValue((error as Error).message);
    }
  }
);

export const deleteJournalEntry = createAsyncThunk(
  'journal/deleteJournalEntry',
  async (id: number, { rejectWithValue }) => {
    try {
      await journalService.delete(id);
      return id; // Return the ID for state updates
    } catch (error) {
      return rejectWithValue((error as Error).message);
    }
  }
);

// Slice
export const journalSlice = createSlice({
  name: 'journal',
  initialState,
  reducers: {
    clearCurrentEntry: (state) => {
      state.currentEntry = null;
    },
  },
  extraReducers: (builder) => {
    builder
      // Fetch journal entries
      .addCase(fetchJournalEntries.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchJournalEntries.fulfilled, (state, action: PayloadAction<JournalEntry[]>) => {
        state.loading = false;
        state.entries = action.payload;
        
        // Extract unique tags from entries
        const tagSet = new Set<string>();
        action.payload.forEach(entry => {
          // Handle Tag objects with IDs and names
          entry.tags?.forEach((tag: Tag) => tagSet.add(tag.name));
        });
        state.tags = Array.from(tagSet);
      })
      .addCase(fetchJournalEntries.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      
      // Fetch journal entry by ID
      .addCase(fetchJournalEntryById.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchJournalEntryById.fulfilled, (state, action: PayloadAction<JournalEntry>) => {
        state.loading = false;
        state.currentEntry = action.payload;
      })
      .addCase(fetchJournalEntryById.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      
      // Create journal entry
      .addCase(createJournalEntry.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(createJournalEntry.fulfilled, (state, action: PayloadAction<JournalEntry>) => {
        state.loading = false;
        state.entries = [...state.entries, action.payload];
        
        // Add new tags
        if (action.payload.tags) {
          const tagNames = action.payload.tags.map((tag: Tag) => tag.name);
          const newTags = tagNames.filter((tagName: string) => !state.tags.includes(tagName));
          if (newTags.length > 0) {
            state.tags = [...state.tags, ...newTags];
          }
        }
      })
      .addCase(createJournalEntry.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      
      // Update journal entry
      .addCase(updateJournalEntry.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(updateJournalEntry.fulfilled, (state, action: PayloadAction<JournalEntry>) => {
        state.loading = false;
        state.entries = state.entries.map((entry: JournalEntry) => 
          entry.id === action.payload.id ? action.payload : entry
        );
        if (state.currentEntry?.id === action.payload.id) {
          state.currentEntry = action.payload;
        }
        
        // Update tags
        if (action.payload.tags) {
          const tagSet = new Set<string>(state.tags);
          action.payload.tags.forEach((tag: Tag) => tagSet.add(tag.name));
          state.tags = Array.from(tagSet);
        }
      })
      .addCase(updateJournalEntry.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      
      // Delete journal entry
      .addCase(deleteJournalEntry.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(deleteJournalEntry.fulfilled, (state, action: PayloadAction<number>) => {
        state.loading = false;
        state.entries = state.entries.filter((entry: JournalEntry) => entry.id !== action.payload);
        if (state.currentEntry?.id === action.payload) {
          state.currentEntry = null;
        }
      })
      .addCase(deleteJournalEntry.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      });
  },
});

// Actions
export const { clearCurrentEntry } = journalSlice.actions;

// Selectors
export const selectAllJournalEntries = (state: { journal: JournalState }) => state.journal.entries;
export const selectCurrentEntry = (state: { journal: JournalState }) => state.journal.currentEntry;
export const selectJournalLoading = (state: { journal: JournalState }) => state.journal.loading;
export const selectJournalError = (state: { journal: JournalState }) => state.journal.error;
export const selectAllTags = (state: { journal: JournalState }) => state.journal.tags;
export const selectTags = (state: { journal: JournalState }) => state.journal.tags;

export default journalSlice.reducer;
