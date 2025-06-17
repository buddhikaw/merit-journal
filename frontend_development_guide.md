# Merit Journal Frontend Development Guide

## Project Overview

Merit Journal is a full-stack application that allows users to record and manage meritorious acts or significant things done on a particular day. The frontend is built with React, TypeScript, and Material-UI (MUI).

## Architecture

The frontend follows modern React patterns:

1. **Component-Based Architecture**: UI elements are broken down into reusable components.
2. **Container-Component Pattern**: Separation of components that manage data and those that render UI.
3. **Redux for State Management**: Centralized state management with Redux Toolkit.
4. **Type-Safe Development**: TypeScript is used throughout the application.

## Key Technology Stack

- **React**: UI library
- **TypeScript**: Type-safe JavaScript
- **Redux Toolkit**: State management
- **Material-UI (MUI)**: UI component library
- **oidc-client-ts**: Authentication with OpenID Connect
- **Vite**: Build tool and development server

## Project Structure

```
src/
  Frontend/
    public/                # Static assets
    src/
      app/                 # App configuration
        store.ts          # Redux store setup
      components/          # Reusable UI components
      features/            # Feature-based modules with their own state
        auth/             # Authentication related code
        journalEntry/     # Journal entry related code
      pages/               # Page components
      services/            # API service functions
      types/               # TypeScript type definitions
      App.tsx              # Main application component
      index.tsx            # Application entry point
      theme.ts            # Material-UI theme customization
    index.html             # HTML entry point
    package.json           # Dependencies and scripts
    tsconfig.json          # TypeScript configuration
    vite.config.ts         # Vite configuration
```

## State Management

The application uses Redux Toolkit for state management:

### Store Structure

```
{
  auth: {
    isAuthenticated: boolean,
    user: User | null,
    token: string | null,
    loading: boolean,
    error: string | null
  },
  journalEntries: {
    entries: JournalEntry[],
    currentEntry: JournalEntry | null,
    loading: boolean,
    error: string | null
  }
}
```

### Key Slices

- **authSlice**: Manages authentication state
- **journalEntrySlice**: Manages journal entry data

### Async Thunks

Redux Toolkit thunks are used for async operations:
- `loginUser`: Initiates OIDC login flow
- `fetchJournalEntries`: Gets all journal entries for the current user
- `createJournalEntry`: Creates a new journal entry
- `updateJournalEntry`: Updates an existing journal entry
- `deleteJournalEntry`: Deletes a journal entry

## Authentication Flow

The application uses OIDC authentication:

1. User clicks "Login" which initiates the OIDC flow
2. User is redirected to the identity provider (e.g., Google, Microsoft)
3. After successful authentication, user is redirected back with tokens
4. The access token is stored in Redux state
5. The token is sent with each API request in the Authorization header

## API Interactions

Services are organized by domain and handle API calls:

```typescript
// Example of a service function
export const fetchJournalEntries = async (token: string) => {
  const response = await fetch(`${API_BASE_URL}/api/journal-entries`, {
    headers: {
      Authorization: `Bearer ${token}`
    }
  });
  
  if (!response.ok) {
    throw new Error('Failed to fetch journal entries');
  }
  
  return response.json();
};
```

## Components Organization

1. **Base Components**: Reusable UI elements like buttons, inputs, cards
2. **Feature Components**: Components specific to a feature, like JournalEntryForm
3. **Page Components**: Top-level components that compose the UI for entire pages
4. **Layout Components**: Components that handle layout structure

## Styling

Material-UI is used for consistent styling:

- Theme customization in `theme.ts`
- Component styling using Material-UI's `sx` prop and `styled` utility
- Responsive design using MUI's Grid system and breakpoints

## Forms and Validation

- Forms are built using controlled components
- Validation is handled with a mix of:
  - HTML5 validation attributes
  - Form-level validation in component state
  - Server-side validation responses handled in UI

## Image Handling

Images are stored as base64-encoded strings:

1. When selecting an image file, it's converted to a base64 string
2. This string is sent to the API as part of the journal entry data
3. When displaying, the base64 string is used directly in img src attributes

## Key Features Implementation

### Journal Entry Management

- **List View**: Grid of journal entry cards, sorted by date
- **Detail View**: Full journal entry with formatted content and images
- **Creation/Editing**: Form with rich text editor and image upload
- **Tagging**: Multi-select component for adding and managing tags

### Tag Search

- Search bar that filters journal entries by tag
- Tag cloud showing most used tags

## Common Tasks

### Adding a New Component

1. Create a new file in the appropriate folder (e.g., `components/`, `features/`)
2. Import necessary dependencies
3. Define the component with proper TypeScript interfaces
4. Export the component

Example:
```tsx
import React from 'react';
import { Box, Typography } from '@mui/material';

interface MyComponentProps {
  title: string;
  content: string;
}

export const MyComponent: React.FC<MyComponentProps> = ({ title, content }) => {
  return (
    <Box sx={{ padding: 2 }}>
      <Typography variant="h5">{title}</Typography>
      <Typography variant="body1">{content}</Typography>
    </Box>
  );
};
```

### Adding a New Redux Slice

1. Create a new file in `features/[feature]/[feature]Slice.ts`
2. Define the initial state and action creators
3. Create reducers and thunks as needed
4. Add the slice to the store in `app/store.ts`

### Adding a New API Service

1. Create or update a file in `services/` folder
2. Define functions for API interactions
3. Use fetch API with proper error handling
4. Return structured data for use in components or thunks

## Testing

- Component tests with React Testing Library
- Redux tests for slice reducers and thunks
- Mock API responses for testing async operations

## Build and Deployment

- Development: `npm run dev` - Starts Vite dev server
- Build: `npm run build` - Creates production build in `dist/`
- Preview: `npm run preview` - Preview production build locally

## Troubleshooting

### Common Issues

1. **Authentication Errors**:
   - Check if token is expired
   - Verify OIDC configuration
   - Check browser console for CORS errors

2. **API Integration Issues**:
   - Ensure API base URL is correct
   - Check that Authorization header is being sent
   - Verify request/response data structure matches API expectations

3. **State Management Problems**:
   - Use Redux DevTools to inspect state changes
   - Check if the correct actions are being dispatched
   - Verify that components are connected to the store properly

4. **UI Rendering Issues**:
   - Check component props
   - Verify that conditional rendering logic is correct
   - Use React DevTools to inspect component hierarchy

### Performance Optimization

- Use React.memo for components that render frequently
- Implement pagination for large data sets
- Use lazy loading for images and components when possible
- Minimize unnecessary re-renders by optimizing Redux selectors
