export const environment = {
  production: true,
  apiUrl: 'https://api.iorder.com.br/api',
  signalrUrl: 'https://api.iorder.com.br/hubs/chat',
  auth0: {
    domain: 'iorder.us.auth0.com',
    clientId: 'YOUR_CLIENT_ID',
    audience: 'https://api.iorder.com',
    redirectUri: typeof window !== 'undefined' ? window.location.origin : '',
  },
};
