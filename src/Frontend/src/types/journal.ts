/**
 * Types for journal-related data
 */

export interface JournalImage {
  id: number;
  journalEntryId: number;
  imageUrl: string;
  caption?: string;
  sortOrder: number;
}

export interface Tag {
  id: number;
  name: string;
}

export interface JournalEntry {
  id: number;
  userId: string;
  title: string;
  content: string;
  entryDate: string;
  createdAt: string;
  updatedAt: string;
  images: JournalImage[];
  tags: string[];
}

/**
 * DTO for creating/updating a journal entry
 */
export interface JournalEntryDto {
  title: string;
  content: string;
  entryDate: string;
  images?: JournalImage[];
  tags?: string[];
}
