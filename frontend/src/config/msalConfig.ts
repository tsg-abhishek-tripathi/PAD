import { Configuration, PopupRequest, PublicClientApplication } from '@azure/msal-browser';

// MSAL configuration
export const msalConfig: Configuration = {
  auth: {
    clientId: process.env.REACT_APP_AZURE_CLIENT_ID || "6b2f8156-2bfb-4963-bd0d-9e8e8ccc41b2",
    authority: `https://login.microsoftonline.com/${process.env.REACT_APP_AZURE_TENANT_ID || "eb120e12-65f1-477a-be8c-fe4f65926724"}`,
    redirectUri: process.env.REACT_APP_REDIRECT_URI || window.location.origin,
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
          case 0: // Error
            console.error(message);
            break;
          case 1: // Info
            console.info(message);
            break;
          case 2: // Verbose
            console.debug(message);
            break;
          case 3: // Warning
            console.warn(message);
            break;
        }
      },
    },
  },
};

// Add scopes here for ID token to be used at Microsoft identity platform endpoints.
export const loginRequest: PopupRequest = {
  scopes: ["User.Read", process.env.REACT_APP_API_SCOPE || "api://e7fcbcbc-56ff-4bf5-8255-0cd77f38512a/access_as_user"]
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
  baseUrl: process.env.REACT_APP_API_BASE_URL || 'http://localhost:5000/api',
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