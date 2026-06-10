import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { ThemeService } from '../../../core/services/theme.service';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './ForgotPassword.component.html',
  styleUrl: './ForgotPassword.component.scss',
})
export class ForgotPasswordComponent {
  private auth = inject(AuthService);
  private themeService = inject(ThemeService);
  private router = inject(Router);

  email = '';
  isLoading = signal(false);
  sent = signal(false);

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

  navigateToLogin() {
    this.router.navigateByUrl('/login');
  }

  onSubmit() {
    if (!this.email) return;
    this.isLoading.set(true);

    this.auth.forgotPassword(this.email).subscribe({
      next: () => {
        this.sent.set(true);
        this.isLoading.set(false);
      },
      error: () => {
        this.sent.set(true);
        this.isLoading.set(false);
      },
    });
  }
}
