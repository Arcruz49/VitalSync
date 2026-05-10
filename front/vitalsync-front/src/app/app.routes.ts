import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login/Login.component').then(m => m.LoginComponent)
  },
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  {
  path: 'register',
  loadComponent: () =>
    import('./features/auth/register/Register.component').then(m => m.RegisterComponent)
  }
];
