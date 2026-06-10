import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { AuthLayoutComponent } from './shared/auth-layout/AuthLayout.component';

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
    path: 'forgot-password',
    loadComponent: () => import('./features/auth/forgot-password/ForgotPassword.component').then(m => m.ForgotPasswordComponent)
  },
  {
    path: 'reset-password',
    loadComponent: () => import('./features/auth/reset-password/ResetPassword.component').then(m => m.ResetPasswordComponent)
  },
  {
    path: 'onboarding',
    canActivate: [authGuard],
    loadComponent: () => import('./features/onboarding/Onboarding.component').then(m => m.OnboardingComponent)
  },
  {
    path: '',
    component: AuthLayoutComponent,
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        data: { title: 'VitalSync.' },
        loadComponent: () => import('./features/dashboard/Dashboard.component').then(m => m.DashboardComponent)
      },
      {
        path: 'health-records',
        data: { title: 'Registros' },
        loadComponent: () => import('./features/health-records/HealthRecords.component').then(m => m.HealthRecordsComponent)
      },
      {
        path: 'profile',
        data: { title: 'Perfil' },
        loadComponent: () => import('./features/profile/Profile.component').then(m => m.ProfileComponent)
      },
      {
        path: 'alerts',
        data: { title: 'Alertas' },
        loadComponent: () => import('./features/alerts/Alerts.component').then(m => m.AlertsComponent)
      },
      {
        path: 'nutrition',
        data: { title: 'Nutrição' },
        loadComponent: () => import('./features/nutrition/Nutrition.component').then(m => m.NutritionComponent)
      },
      {
        path: 'reports',
        data: { title: 'Relatórios' },
        loadComponent: () => import('./features/reports/Reports.component').then(m => m.ReportsComponent)
      },
    ]
  },
  { path: '**', redirectTo: 'login' }
];
