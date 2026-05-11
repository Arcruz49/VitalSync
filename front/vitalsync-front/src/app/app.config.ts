import { APP_INITIALIZER, ApplicationConfig, PLATFORM_ID, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withFetch } from '@angular/common/http';
import { isPlatformBrowser } from '@angular/common';
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';
import { of } from 'rxjs';

import { routes } from './app.routes';
import { AuthService } from './core/services/auth.service';

function initAuth(auth: AuthService, platformId: object) {
  return () => isPlatformBrowser(platformId) ? auth.me() : of(null);
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(withFetch()),
    provideClientHydration(withEventReplay()),
    {
      provide: APP_INITIALIZER,
      useFactory: initAuth,
      deps: [AuthService, PLATFORM_ID],
      multi: true,
    },
  ]
};
