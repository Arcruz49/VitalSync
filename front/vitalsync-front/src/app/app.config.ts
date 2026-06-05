import { APP_INITIALIZER, ApplicationConfig, PLATFORM_ID, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { errorInterceptor } from './core/interceptors/error.interceptor';
import { isPlatformBrowser } from '@angular/common';
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';
import { provideAnimations } from '@angular/platform-browser/animations';
import { of } from 'rxjs';

import { routes } from './app.routes';
import { AuthService } from './core/services/auth.service';
import { ThemeService } from './core/services/theme.service';

function initApp(auth: AuthService, theme: ThemeService, platformId: object) {
  return () => {
    if (isPlatformBrowser(platformId)) theme.init();
    return isPlatformBrowser(platformId) ? auth.me() : of(null);
  };
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(withFetch(), withInterceptors([errorInterceptor])),
    provideClientHydration(withEventReplay()),
    provideAnimations(),
    {
      provide: APP_INITIALIZER,
      useFactory: initApp,
      deps: [AuthService, ThemeService, PLATFORM_ID],
      multi: true,
    },
  ]
};
