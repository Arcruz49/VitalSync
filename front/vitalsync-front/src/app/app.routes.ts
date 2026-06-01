import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/Login.component').then(m => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () => import('./features/auth/register/Register.component').then(m => m.RegisterComponent)
  },
  {
    path: 'onboarding',
    canActivate: [authGuard],
    loadComponent: () => import('./features/onboarding/Onboarding.component').then(m => m.OnboardingComponent)
  },
  {
    path: 'dashboard',
    canActivate: [authGuard],
    loadComponent: () => import('./features/dashboard/Dashboard.component').then(m => m.DashboardComponent)
  },
  {
    path: 'health-records',
    canActivate: [authGuard],
    loadComponent: () => import('./features/health-records/HealthRecords.component').then(m => m.HealthRecordsComponent)
  },
  {
    path: 'profile',
    canActivate: [authGuard],
    loadComponent: () => import('./features/profile/Profile.component').then(m => m.ProfileComponent)
  },
  { path: '**', redirectTo: 'login' }
];
