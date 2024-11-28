 import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

const oAuthConfig = {
  issuer: 'https://localhost:44305/',
  redirectUri: baseUrl,
  clientId: 'TvTracker_App',
  responseType: 'code',
  scope: 'offline_access TvTracker',
  requireHttps: true,
};

export const environment = {
  production: false,
  application: {
    baseUrl,
    name: 'TvTracker',
  },
  oAuthConfig,
  apis: {
    default: {
      url: 'https://localhost:44305',
      rootNamespace: 'TvTracker',
    },
    AbpAccountPublic: {
      url: oAuthConfig.issuer,
      rootNamespace: 'AbpAccountPublic',
    },
  },
} as Environment;
