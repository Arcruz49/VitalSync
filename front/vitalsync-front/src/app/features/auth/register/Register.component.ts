import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { ThemeService } from '../../../core/services/theme.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './Register.component.html',
  styleUrl: './Register.component.scss',
})
export class RegisterComponent {
  private auth = inject(AuthService);
  private themeService = inject(ThemeService);
  private router = inject(Router);

  name = '';
  lastName = '';
  email = '';
  birthDate = '';
  birthDateDisplay = '';
  gender = '';
  password = '';
  showPassword = false;
  isLoading = signal(false);
  errorMessage = signal('');
  isExiting = false;

  updateBirthDateDisplay(ymd: string) {
    if (!ymd) { this.birthDateDisplay = ''; return; }
    const [y, m, d] = ymd.split('-');
    this.birthDateDisplay = `${d}/${m}/${y}`;
  }

  get isDark() { return this.themeService.isDark; }

  passwordStrength = computed(() => {
    if (this.password.length === 0) return 0;
    let score = 0;
    if (this.password.length >= 8) score++;
    if (/[A-Z]/.test(this.password)) score++;
    if (/[0-9]/.test(this.password)) score++;
    if (/[^A-Za-z0-9]/.test(this.password)) score++;
    return score;
  });

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
    if (this.isExiting) return;
    this.isExiting = true;
    setTimeout(() => this.router.navigateByUrl('/login'), 390);
  }

  onSubmit() {
    const fullName = `${this.name} ${this.lastName}`.trim();
    if (!fullName || !this.email || !this.password || !this.birthDate || !this.gender) {
      this.errorMessage.set('Preencha todos os campos obrigatórios.');
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set('');

    this.auth.register({
      name: fullName,
      email: this.email,
      password: this.password,
      gender: this.gender,
      birthDate: this.birthDate,
    }).subscribe({
      next: () => this.router.navigateByUrl('/onboarding'),
      error: (err) => {
        this.errorMessage.set(err.error?.message ?? 'Erro ao criar conta. Tente novamente.');
        this.isLoading.set(false);
      }
    });
  }
}
