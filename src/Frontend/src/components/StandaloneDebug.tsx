import React, { useState, useEffect } from 'react';

/**
 * A standalone debug component that doesn't rely on Redux or other external dependencies
 */
const StandaloneDebug: React.FC = () => {
  const [loadTime, setLoadTime] = useState<string>(new Date().toISOString());
  const [clickCount, setClickCount] = useState<number>(0);
  const [error, setError] = useState<string | null>(null);
  
  useEffect(() => {
    try {
      console.log('StandaloneDebug component mounted successfully');
    } catch (e) {
      setError(`Error in useEffect: ${e instanceof Error ? e.message : String(e)}`);
    }
  }, []);

  const handleClick = () => {
    try {
      setClickCount(prev => prev + 1);
      console.log('Button clicked, new count:', clickCount + 1);
    } catch (e) {
      setError(`Error in click handler: ${e instanceof Error ? e.message : String(e)}`);
    }
  };

  return (
    <div style={{
      padding: '20px',
      margin: '20px auto',
      maxWidth: '800px',
      fontFamily: 'Arial, sans-serif',
      backgroundColor: '#f5f5f5',
      borderRadius: '8px',
      boxShadow: '0 2px 4px rgba(0,0,0,0.2)'
    }}>
      <h1 style={{ color: '#333', borderBottom: '1px solid #ddd', paddingBottom: '10px' }}>
        Standalone Debug Component
      </h1>
      
      <div style={{ backgroundColor: '#e1ffe1', padding: '15px', borderRadius: '5px', marginBottom: '20px' }}>
        <p><strong>Status:</strong> If you can see this, basic React rendering is working!</p>
        <p><strong>Component loaded at:</strong> {loadTime}</p>
        <p><strong>Click count:</strong> {clickCount}</p>
      </div>
      
      {error && (
        <div style={{ backgroundColor: '#ffebee', color: '#c62828', padding: '15px', borderRadius: '5px', marginBottom: '20px' }}>
          <h3>Error Detected:</h3>
          <pre style={{ whiteSpace: 'pre-wrap', overflowX: 'auto' }}>{error}</pre>
        </div>
      )}
      
      <div style={{ marginTop: '20px' }}>
        <button 
          onClick={handleClick}
          style={{
            backgroundColor: '#4caf50',
            color: 'white',
            border: 'none',
            padding: '10px 20px',
            fontSize: '16px',
            borderRadius: '4px',
            cursor: 'pointer'
          }}
        >
          Test React State ({clickCount} clicks)
        </button>
      </div>
      
      <div style={{ marginTop: '20px', padding: '15px', backgroundColor: '#fff', borderRadius: '5px' }}>
        <h3>Navigation Options:</h3>
        <ul>
          <li><a href="/" style={{ color: '#2196f3' }}>Home</a></li>
          <li><a href="/simple-debug" style={{ color: '#2196f3' }}>Simple Debug</a></li>
          <li><a href="/basic" style={{ color: '#2196f3' }}>Basic Page</a></li>
        </ul>
      </div>
      
      <div style={{ marginTop: '20px', fontSize: '14px', color: '#666', borderTop: '1px solid #ddd', paddingTop: '10px' }}>
        This component is completely self-contained and does not rely on Redux or other external state management.
      </div>
    </div>
  );
};

export default StandaloneDebug;
