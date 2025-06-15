import React from 'react';

const SimpleDebug: React.FC = () => {
  return (
    <div style={{ padding: '20px', margin: '20px', border: '2px solid blue' }}>
      <h1>Simple Debug Page</h1>
      <p>If you can see this, basic React rendering is working.</p>
      
      <div style={{ marginTop: '20px' }}>
        <h2>Debug Links:</h2>
        <ul>
          <li><a href="/debug">Regular Debug</a></li>
          <li><a href="/api-test">API Test</a></li>
          <li><a href="/redux-debug">Redux Debug</a></li>
          <li><a href="/layout-debug">Layout Debug</a></li>
          <li><a href="/debug-index">Debug Index</a></li>
        </ul>
      </div>
      
      <div style={{ marginTop: '20px', padding: '10px', border: '1px solid #ccc' }}>
        <button 
          onClick={() => {
            console.log('Debug button clicked');
            alert('React events are working!');
          }}
          style={{ padding: '8px 16px', background: '#007bff', color: 'white', border: 'none' }}
        >
          Test React Events
        </button>
      </div>
    </div>
  );
};

export default SimpleDebug;
