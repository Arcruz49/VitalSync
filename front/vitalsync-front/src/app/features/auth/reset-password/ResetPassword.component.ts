import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { ThemeService } from '../../../core/services/theme.service';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './ResetPassword.component.html',
  styleUrl: './ResetPassword.component.scss',
})
export class ResetPasswordComponent implements OnInit {
  private auth = inject(AuthService);
  private themeService = inject(ThemeService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  token = '';
  password = '';
  confirmPassword = '';
  showPassword = false;
  isLoading = signal(false);
  errorMessage = signal('');
  success = signal(false);

  get isDark() { return this.themeService.isDark; }

  ngOnInit() {
    this.token = this.route.snapshot.queryParams['token'] ?? '';
  }

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

  navigateToLogin() {
    this.router.navigateByUrl('/login');
  }

  onSubmit() {
    if (!this.password || !this.confirmPassword) return;
    if (this.password !== this.confirmPassword) {
      this.errorMessage.set('As senhas não coincidem.');
      return;
    }
    if (this.password.length < 8) {
      this.errorMessage.set('A senha deve ter pelo menos 8 caracteres.');
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set('');

    this.auth.resetPassword(this.token, this.password).subscribe({
      next: () => {
        this.success.set(true);
        this.isLoading.set(false);
        setTimeout(() => this.router.navigateByUrl('/login'), 2500);
      },
      error: (err) => {
        this.errorMessage.set(err.error?.message ?? 'Token inválido ou expirado.');
        this.isLoading.set(false);
      },
    });
  }
}
