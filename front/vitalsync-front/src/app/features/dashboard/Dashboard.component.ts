import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div style="padding: 2rem; font-family: sans-serif;">
      <h1>Dashboard</h1>
      <p>Logado como: <strong>{{ auth.currentUser()?.name }}</strong></p>
      <nav style="margin-top: 1rem; display: flex; gap: 1rem;">
        <a routerLink="/metric-types" style="padding: 0.5rem 1.5rem; background: #0D9488; color: white; text-decoration: none; border-radius: 4px;">
          Tipos de Métricas
        </a>
        <button (click)="logout()" style="padding: 0.5rem 1.5rem; cursor: pointer;">
          Logout
        </button>
      </nav>
    </div>
  `,
})
export class DashboardComponent {
  auth = inject(AuthService);
  private router = inject(Router);

  logout() {
    this.auth.logout().subscribe({
      next: () => this.router.navigateByUrl('/login'),
    });
  }
}
