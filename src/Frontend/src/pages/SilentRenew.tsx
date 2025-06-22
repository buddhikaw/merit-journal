import React, { useEffect } from 'react';
import authService from '../services/authService';

const SilentRenew: React.FC = () => {
  useEffect(() => {
    const processSilentRenew = async () => {
      try {
        // Log the silent renew attempt
        console.log('Processing silent renew callback');
        
        // Log the URL for debugging
        console.log('Silent renew URL:', window.location.href);
        
        // Process the silent renew callback
        const user = await authService.completeSilentLogin();
        
        // Log success or failure
        if (user) {
          console.log('Silent token renewal successful');
        } else {
          console.warn('Silent token renewal completed but no user was returned');
        }
      } catch (error) {
        // Enhanced error logging
        console.error('Silent renew error:', error);
        
        if (typeof error === 'object' && error !== null) {
          // Log additional details that might be available
          console.error('Error details:', {
            name: (error as Error).name,
            message: (error as Error).message,
            // @ts-ignore - Check for OIDC specific error properties
            error_description: (error as any).error_description
          });
        }
      }
    };
    
    processSilentRenew();
  }, []);

  // This component is invisible to the user
  return null;
};

export default SilentRenew;
