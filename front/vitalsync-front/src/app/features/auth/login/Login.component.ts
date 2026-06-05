import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { ThemeService } from '../../../core/services/theme.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule],
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
  isExiting = false;

  get isDark() { return this.themeService.isDark; }

  toggleTheme(event: MouseEvent) {
    if (typeof document === 'undefined' || !('startViewTransition' in document)) {
      this.themeService.toggle();
      return;
    }
    const btn = event.currentTarget as HTMLElement;
    const { left, top, width, height } = btn.getBoundingClientRect();
    const x = left + width / 2;
    const y = top + height / 2;
    const radius = Math.hypot(Math.max(x, innerWidth - x), Math.max(y, innerHeight - y));
    const vt = (document as any).startViewTransition(() => this.themeService.toggle());
    vt.ready.then(() => {
      document.documentElement.animate(
        { clipPath: [`circle(0px at ${x}px ${y}px)`, `circle(${radius}px at ${x}px ${y}px)`] },
        { duration: 450, easing: 'ease-out', pseudoElement: '::view-transition-new(root)' }
      );
    });
  }

  navigateToRegister() {
    if (this.isExiting) return;
    this.isExiting = true;
    setTimeout(() => this.router.navigateByUrl('/register'), 390);
  }

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
