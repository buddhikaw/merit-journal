# Merit Journal Frontend

The frontend application for Merit Journal built with React, TypeScript, and Material-UI.

## Technologies

- React 18
- TypeScript
- Redux Toolkit for state management
- Material-UI (MUI) for UI components
- Vite as build tool
- React Router for navigation
- Authentication with OIDC

## Features

- Create, read, update, and delete journal entries
- Tag journal entries for better organization
- Attach images to journal entries
- User authentication and authorization
- Responsive design for all devices

## Prerequisites

- Node.js (v18 or higher)
- npm or yarn

## Getting Started

### Installation

Install the dependencies:

```bash
npm install
# or
yarn
```

### Development

Start the development server:

```bash
npm run dev
# or
yarn dev
```

The application will be available at `http://localhost:3000`.

### Building for Production

```bash
npm run build
# or
yarn build
```

The build output will be in the `dist` directory.

### Environment Variables

Create a `.env` file in the root directory with the following variables:

```
VITE_API_BASE_URL=https://localhost:5001
VITE_AUTH_ENABLED=true
```

For local development without authentication, you can set `VITE_AUTH_ENABLED=false`.

## Project Structure

```
src/
├── app/              # Application setup (store configuration)
├── components/       # Reusable UI components
├── features/         # Feature-based modules
│   ├── auth/         # Authentication related components and logic
│   ├── journal/      # Journal entry related components and logic
│   └── ...
├── pages/            # Page components
├── services/         # API services
├── types/            # TypeScript type definitions
├── utils/            # Utility functions
├── App.tsx           # Main App component
└── index.tsx         # Entry point
```

## State Management

The application uses Redux Toolkit for state management. Each feature has its own slice that manages its state, actions, and reducers.

## API Integration

API calls are handled through services that use the Fetch API. The base URL for API calls is configured through environment variables.

## Authentication

The application uses OIDC for authentication. Authentication can be disabled for local development by setting the appropriate environment variable.

## Docker Support

The application includes Docker support. To build and run with Docker:

```bash
docker build -t meritjournal-frontend .
docker run -p 3000:80 meritjournal-frontend
```
