import React, { useEffect } from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import { useSelector } from 'react-redux';
import { selectIsAuthenticated } from './features/auth/authSlice';
import authService from './services/authService';

// Pages
import LoginPage from './pages/LoginPage';
import JournalListPage from './pages/JournalListPage';
import JournalEntryPage from './pages/JournalEntryPage';
import CreateJournalEntryPage from './pages/CreateJournalEntryPage';
import EditJournalEntryPage from './pages/EditJournalEntryPage';
import NotFoundPage from './pages/NotFoundPage';
import TestPage from './pages/TestPage';
import DebugIndexPage from './pages/DebugIndexPage';
import BasicPage from './pages/BasicPage';
import AuthCallback from './pages/AuthCallback';
import SilentRenew from './pages/SilentRenew';

// Components
import Layout from './components/Layout';
import DebugComponent from './components/DebugComponent';
import ApiTestComponent from './components/ApiTestComponent';
import ReduxDebugComponent from './components/ReduxDebugComponent';
import LayoutDebugger from './components/LayoutDebugger';
import SimpleDebug from './components/SimpleDebug';
import StandaloneDebug from './components/StandaloneDebug';

// Authentication is now enabled
const BYPASS_AUTH = false;

// Protected route wrapper
const ProtectedRoute = ({ children }: { children: React.ReactNode }) => {
  const isAuthenticated = useSelector(selectIsAuthenticated);
  
  // TEMPORARY: Allow access without authentication in development mode
  if (!isAuthenticated && !BYPASS_AUTH) {
    return <Navigate to="/login" replace />;
  }
  
  return <>{children}</>;
};

function App() {
  // Initialize authentication
  useEffect(() => {
    authService.initializeAuth();
  }, []);

  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/test" element={<TestPage />} />
      <Route path="/debug" element={<DebugComponent />} />
      <Route path="/simple-debug" element={<SimpleDebug />} />
      <Route path="/basic" element={<BasicPage />} />
      <Route path="/standalone-debug" element={<StandaloneDebug />} />
      <Route path="/api-test" element={<ApiTestComponent />} />
      <Route path="/redux-debug" element={<ReduxDebugComponent />} />
      <Route path="/layout-debug" element={<LayoutDebugger />} />
      <Route path="/debug-index" element={<DebugIndexPage />} />

      {/* Auth Routes */}
      <Route path="/authentication/callback" element={<AuthCallback />} />
      <Route path="/authentication/silent-callback" element={<SilentRenew />} />
      
      <Route path="/" element={
        <ProtectedRoute>
          <Layout />
        </ProtectedRoute>
      }>
        <Route index element={<JournalListPage />} />
        <Route path="entries/new" element={<CreateJournalEntryPage />} />
        <Route path="entries/:id" element={<JournalEntryPage />} />
        <Route path="entries/:id/edit" element={<EditJournalEntryPage />} />
      </Route>
      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  );
}

export default App;
