export const environment = {
  production: false,
  apiUrl: 'https://localhost:7023',
  signalrUrl: 'https://localhost:7023/hubs/chat',
  auth0: {
    domain: 'iorder.us.auth0.com',
    clientId: 'aNd8zsy1b7HYxFTcfRXCNbnbGeDr1l9V',
    audience: 'https://api.iorder.com',
    redirectUri: typeof window !== 'undefined' ? window.location.origin : '',
  },
};
