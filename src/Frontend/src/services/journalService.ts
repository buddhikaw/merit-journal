import { apiService } from './apiService';
import { JournalEntry, JournalEntryDto } from '../types/journal';

/**
 * Helper function to ensure dates are in the correct format for the backend
 * PostgreSQL requires UTC dates for timestamp with time zone columns
 */
const ensureUTCDates = (entry: JournalEntryDto): JournalEntryDto => {
  // Clone the entry to avoid mutating the original
  const entryClone = { ...entry };
  
  // Convert entryDate to an explicit ISO string with 'Z' to ensure UTC
  if (entryClone.entryDate && typeof entryClone.entryDate === 'string') {
    const date = new Date(entryClone.entryDate);
    // Use the UTC version of the date and ensure it ends with 'Z'
    entryClone.entryDate = date.toISOString();
  }
  
  return entryClone;
};

/**
 * Service for journal-related API operations
 */
export const journalService = {
  /**
   * Get all journal entries
   */
  async getAll(): Promise<JournalEntry[]> {
    return apiService.get<JournalEntry[]>('/journal-entries');
  },
  
  /**
   * Get a single journal entry by ID
   */
  async getById(id: number): Promise<JournalEntry> {
    return apiService.get<JournalEntry>(`/journal-entries/${id}`);
  },
  
  /**
   * Create a new journal entry
   */
  async create(entry: JournalEntryDto): Promise<JournalEntry> {
    const entryWithUTCDates = ensureUTCDates(entry);
    return apiService.post<JournalEntry>('/journal-entries', entryWithUTCDates);
  },
  
  /**
   * Update an existing journal entry
   */
  async update(id: number, entry: JournalEntryDto): Promise<JournalEntry> {
    const entryWithUTCDates = ensureUTCDates(entry);
    return apiService.put<JournalEntry>(`/journal-entries/${id}`, entryWithUTCDates);
  },
  
  /**
   * Delete a journal entry
   */
  async delete(id: number): Promise<void> {
    return apiService.delete<void>(`/journal-entries/${id}`);
  },
  
  /**
   * Search journal entries by tag
   */
  async searchByTag(tag: string): Promise<JournalEntry[]> {
    return apiService.get<JournalEntry[]>(`/journal-entries/search?tag=${encodeURIComponent(tag)}`);
  }
};
