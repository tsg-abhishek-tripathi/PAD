import { Configuration, PopupRequest, PublicClientApplication } from '@azure/msal-browser';

// MSAL configuration
export const msalConfig: Configuration = {
  auth: {
    clientId: process.env.REACT_APP_CLIENT_ID || "YOUR_CLIENT_ID",
    authority: `https://login.microsoftonline.com/${process.env.REACT_APP_TENANT_ID || "YOUR_TENANT_ID"}`,
    redirectUri: "https://726729e2-0245-4990-b707-7eddbbbf3c34-00-220kpho6o8ybn.sisko.replit.dev",
  },
  cache: {
    cacheLocation: 'sessionStorage', // This configures where your cache will be stored
    storeAuthStateInCookie: false, // Set this to "true" if you are having issues on IE11 or Edge
    secureCookies: false, // Set this to "true" if you are having issues on IE11 or Edge
  },
  system: {
    loggerOptions: {
      loggerCallback: (level, message, containsPii) => {
        if (containsPii) {
          return;
        }
        switch (level) {
          case 'Error':
            console.error(message);
            break;
          case 'Info':
            console.info(message);
            break;
          case 'Verbose':
            console.debug(message);
            break;
          case 'Warning':
            console.warn(message);
            break;
        }
      },
    },
  },
};

// Add scopes here for ID token to be used at Microsoft identity platform endpoints.
export const loginRequest: PopupRequest = {
  scopes: ["User.Read", "api://YOUR_API_CLIENT_ID/access_as_user"]
};

// Add the endpoints here for Microsoft Graph API services you'd like to use.
export const graphConfig = {
  graphMeEndpoint: 'https://graph.microsoft.com/v1.0/me'
};

// API scopes for PAD API
export const apiRequest: PopupRequest = {
  scopes: ["api://YOUR_API_CLIENT_ID/access_as_user"]
};

// Environment-specific configuration
export const apiConfig = {
  baseUrl: process.env.REACT_APP_API_BASE_URL || 'https://726729e2-0245-4990-b707-7eddbbbf3c34-00-220kpho6o8ybn.sisko.replit.dev/api',
  timeout: 30000
};

// User role mappings
export const UserRole = {
  SYSTEM_ADMIN: 'SystemAdmin',
  OFFICE_STAFFING: 'OfficeStaffing',
  PPK_GLOBAL_LEAD: 'PPKGlobalLead',
  PPK_REGIONAL_LEAD: 'PPKRegionalLead',
  PPK_PROGRAM_TEAM: 'PPKProgramTeam',
  STAFFING_SYSTEM_SUPPORT: 'StaffingSystemSupport'
} as const;

export type UserRoleType = typeof UserRole[keyof typeof UserRole];

// Create MSAL instance
export const msalInstance = new PublicClientApplication(msalConfig); 