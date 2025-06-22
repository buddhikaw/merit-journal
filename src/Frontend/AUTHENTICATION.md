# Google Authentication Setup Guide

This document guides you through setting up Google authentication for Merit Journal.

## Prerequisites

- A Google account
- Access to [Google Developer Console](https://console.developers.google.com/)

## Steps to Set Up Google Authentication

### 1. Create a Google OAuth 2.0 Client ID

1. Go to the [Google Developer Console](https://console.developers.google.com/)
2. Create a new project or select an existing one
3. Navigate to "APIs & Services" > "Credentials"
4. Click "Create Credentials" > "OAuth client ID"
5. If prompted, configure the OAuth consent screen:
   - User Type: Select "External" (or "Internal" if using Google Workspace)
   - App name: "Merit Journal"
   - User support email: Your email
   - Developer contact information: Your email
   - Click "Save and Continue"
   - Add scopes: "email", "profile", "openid"
   - Click "Save and Continue" and complete the remaining steps
6. Return to "Create OAuth client ID"
7. Application type: Select "Web application"
8. Name: "Merit Journal"

### 2. Configure Authorized JavaScript Origins and Redirect URIs

#### Authorized JavaScript Origins
In the Google Cloud Console, add these origins:

For development:
- `http://localhost:3000`
- `http://127.0.0.1:3000`

For production:
- `https://your-production-domain.com`

#### Authorized Redirect URIs
Add these exact URIs for authentication callbacks:

For development:
- `http://localhost:3000/authentication/callback` (required)
- `http://localhost:3000/authentication/silent-callback` (for token refresh)

Important: The silent-callback URI is used for token refresh. If it's not included in your Google Console configuration, automatic token renewal will not work, and users will need to re-login when their tokens expire.

For production:
- `https://your-production-domain.com/authentication/callback`
- `https://your-production-domain.com/authentication/silent-callback`

**Important Notes:**
1. The paths `/authentication/callback` and `/authentication/silent-callback` must match exactly with what's configured in the authConfig.ts file
2. Include both localhost and 127.0.0.1 versions for development to avoid potential issues
3. HTTPS is required for production environments
4. Add any additional development URLs if you're testing on different ports or domains

Click "Create" to generate your credentials.

### 3. Get Your Client ID and Configure OAuth Settings

After creating your OAuth client, you'll receive a **Client ID**, which is a public identifier for your application. It should look something like `123456789012-abcdefghijklmnopqrstuvwxyz123456.apps.googleusercontent.com`.

#### Important: Authorization Flow Settings
This application uses the **Authorization Code flow with PKCE** (Proof Key for Code Exchange). Make sure:
- Your OAuth client is set to "Web application" type
- No client secret is needed (PKCE is used instead)
- The response_type is set to "code" in the application

### 4. Configure Environment Variables

Create or update the `.env.local` file in the Frontend directory with your Google client ID:

```
# Google Authentication settings
VITE_GOOGLE_CLIENT_ID=your-client-id.apps.googleusercontent.com

# Use mock authentication in development mode
# Set to 'false' once you've configured your Google client ID
VITE_USE_MOCK_AUTH=false

# API Configuration
VITE_API_URL=https://localhost:5001/api
```

## Using Google OAuth JSON Credentials

When you create an OAuth client in Google Cloud Console, you can download the client configuration as a JSON file. It looks like:

```json
{
	"web": {
		"client_id": "456033594477-nb9gevqkl9oqfp5fpfu6d2nv5fvgo6hb.apps.googleusercontent.com",
		"project_id": "merit-journal",
		"auth_uri": "https://accounts.google.com/o/oauth2/auth",
		"token_uri": "https://oauth2.googleapis.com/token",
		"auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
		"client_secret": "test-secret",
		"redirect_uris": [
			"http://localhost:3000/authentication/callback"
		],
		"javascript_origins": [
			"http://localhost:3000"
		]
	}
}
```

To use this with Merit Journal:

1. **Extract the client ID** and add it to your `.env.local` file:
   ```
   VITE_GOOGLE_CLIENT_ID=456033594477-nb9gevqkl9oqfp5fpfu6d2nv5fvgo6hb.apps.googleusercontent.com
   ```

2. **Update your redirect URIs** in Google Cloud Console to include:
   ```
   http://localhost:3000/authentication/silent-callback
   ```

3. **Client Secret**: Despite using PKCE, Google's OAuth implementation still requires the client secret for the token exchange. Add it to your `.env.local` file:
   ```
   VITE_GOOGLE_CLIENT_SECRET=test-secret
   ```

4. **For production**: Add your production URLs to both `redirect_uris` and `javascript_origins` arrays.

## Testing Your Google Authentication

1. Set `VITE_USE_MOCK_AUTH=false` in your `.env.local` file
2. Run the application and navigate to the login page
3. Click the "Sign in with Google" button
4. You should be redirected to the Google sign-in page
5. After signing in, you should be redirected back to your application

## Testing Redirect URIs

To verify your redirect URIs are correctly configured:

1. **Set up your environment**:
   - Ensure your Google client ID is added to `.env.local`
   - Set `VITE_USE_MOCK_AUTH=false` to use real Google authentication

2. **Start your development server**:
   ```bash
   cd src/Frontend
   npm run start
   ```

3. **Manual URI verification**:
   - Navigate to your login page
   - Click the Google Sign-in button
   - When redirected to Google, check the URL in your browser address bar
   - It should contain `redirect_uri=http%3A%2F%2Flocalhost%3A3000%2Fauthentication%2Fcallback` (URL-encoded)
   - After logging in, you should be redirected back to your application

4. **Common redirect errors**:
   - If you see "Error 400: redirect_uri_mismatch", your redirect URI doesn't match what's authorized
   - If you're redirected but see a blank page, check the browser console for errors
   - If silent renewal fails, check the networking tab in developer tools for requests to the silent callback endpoint

## Handling Backend Authentication

For the backend API to validate Google tokens:

1. Use the Google OAuth 2.0 token verification endpoint or libraries
2. Store the following in your backend configuration:
   - Google API Client ID (same as the frontend client ID)
   - No client secret is required for frontend token validation

## Troubleshooting

### Common Authentication Issues:

#### Redirect URI Mismatch Errors
If you see errors like "redirect_uri_mismatch" or "The redirect URI in the request, http://localhost:3000/authentication/callback, does not match the ones authorized for the OAuth client":

1. Double-check your Google Cloud Console settings:
   - Verify the exact URLs in Authorized redirect URIs (https vs http, www vs non-www)   - Check for trailing slashes - `http://localhost:3000/authentication/callback/` is different from `http://localhost:3000/authentication/callback`
   - Ensure all ports match exactly (3000 is configured in the Vite config)

2. Verify your application code:
   - Check that the `redirect_uri` in authConfig.ts matches exactly what's in Google Console
   - Ensure `silent_redirect_uri` is also correctly set for token renewal

3. Try using `127.0.0.1` instead of `localhost` if one isn't working

#### Authorization Flow Issues
If you see errors like "Only the Authorization Code flow (with PKCE) is supported":
- Make sure `response_type` is set to `'code'` in authConfig.ts (not 'token' or 'id_token token')
- Google now requires Authorization Code flow with PKCE for security reasons
- The client type must be "Web application" in Google Cloud Console
- No client secret is needed as PKCE is used for security

#### "client_secret is missing" Error
If you see this specific error:

1. **Include the Client Secret**:
   - Google requires a client secret even when using PKCE
   - Add the client secret to your `.env.local` file:
     ```
     VITE_GOOGLE_CLIENT_SECRET=your-client-secret-here
     ```
   - Update `authConfig.ts` to include the client secret:
     ```typescript
     client_id: import.meta.env.VITE_GOOGLE_CLIENT_ID || '',
     client_secret: import.meta.env.VITE_GOOGLE_CLIENT_SECRET || '',
     ```

2. **Check your OAuth Client Type**:
   - In Google Cloud Console, ensure your client is set as "Web application" type, NOT "Desktop" or "Other"
   - Delete your existing OAuth client and create a new one if needed
   
3. **Verify proper OIDC configuration**:
   - Make sure all metadata endpoints are correctly specified in authConfig.ts
   - Ensure the authority is set to `https://accounts.google.com`

4. **Network Debugging**:
   - Use Chrome DevTools Network tab to inspect the request to Google
   - Verify the POST request to https://oauth2.googleapis.com/token includes the client_secret parameter
   - Ensure that `code_challenge` and `code_challenge_method` are included for PKCE

5. **Clear Cached Tokens**:
   - Clear your browser cookies
   - Open DevTools > Application > Local Storage and clear all entries

#### Other Common Issues
- Check browser console for detailed error messages
- Ensure your OAuth consent screen is properly configured
- Confirm the client ID is correctly set in your environment variables
- Check that you're using HTTPS in production (required for Google OAuth)
- Verify that popup blockers are not preventing the Google login window
- Clear browser cookies and local storage if you're testing configuration changes

## Advanced Configuration

For advanced scenarios like custom scopes or token handling:

- Refer to [Google's OAuth 2.0 documentation](https://developers.google.com/identity/protocols/oauth2)
- Check the [oidc-client-ts documentation](https://github.com/authts/oidc-client-ts) for additional configuration options

## Security Considerations

1. Always validate tokens on your backend before granting access to protected resources
2. Set appropriate token expiration times
3. Implement proper CORS policies on your backend API
4. Use HTTPS for all production traffic
5. Regularly audit and rotate credentials if needed
