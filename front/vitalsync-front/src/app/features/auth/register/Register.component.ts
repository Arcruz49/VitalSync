import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink, Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './Register.component.html',
  styleUrl: './Register.component.scss',
})
export class RegisterComponent {
  private auth = inject(AuthService);
  private router = inject(Router);

  name = '';
  lastName = '';
  email = '';
  birthDate = '';
  gender = '';
  password = '';
  showPassword = false;
  isLoading = signal(false);
  errorMessage = signal('');

  passwordStrength = computed(() => {
    if (this.password.length === 0) return 0;
    let score = 0;
    if (this.password.length >= 8) score++;
    if (/[A-Z]/.test(this.password)) score++;
    if (/[0-9]/.test(this.password)) score++;
    if (/[^A-Za-z0-9]/.test(this.password)) score++;
    return score;
  });

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
