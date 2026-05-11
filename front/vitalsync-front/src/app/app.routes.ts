import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login/Login.component').then(m => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./features/auth/register/Register.component').then(m => m.RegisterComponent)
  },
  {
    path: 'dashboard',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/dashboard/Dashboard.component').then(m => m.DashboardComponent)
  },
  {
    path: 'metric-types',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/metric-types/MetricTypes.component').then(m => m.MetricTypesComponent)
  },
];
