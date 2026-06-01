import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink, Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { ThemeService } from '../../../core/services/theme.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './Login.component.html',
  styleUrl: './Login.component.scss',
})
export class LoginComponent {
  private auth = inject(AuthService);
  private themeService = inject(ThemeService);
  private router = inject(Router);

  email = '';
  password = '';
  showPassword = false;
  isLoading = signal(false);
  errorMessage = signal('');

  get isDark() { return this.themeService.isDark; }

  toggleTheme() { this.themeService.toggle(); }

  onSubmit() {
    if (!this.email || !this.password) return;
    this.isLoading.set(true);
    this.errorMessage.set('');

    this.auth.login({ email: this.email, password: this.password }).subscribe({
      next: () => this.router.navigateByUrl('/dashboard'),
      error: (err) => {
        this.errorMessage.set(err.error?.message ?? 'E-mail ou senha incorretos.');
        this.isLoading.set(false);
      }
    });
  }
}
