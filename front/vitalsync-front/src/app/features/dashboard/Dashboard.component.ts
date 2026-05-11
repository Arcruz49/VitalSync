import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  template: `
    <div style="padding: 2rem; font-family: sans-serif;">
      <h1>Dashboard</h1>
      <p>Logado como: <strong>{{ auth.currentUser()?.name }}</strong></p>
      <button (click)="logout()" style="margin-top: 1rem; padding: 0.5rem 1.5rem; cursor: pointer;">
        Logout
      </button>
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
