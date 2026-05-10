import { Component, computed, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './Register.component.html',
  styleUrl: './Register.component.scss'
})
export class RegisterComponent {
  name = '';
  email = '';
  password = '';
  gender = '';
  birthDate = '';
  acceptTerms = false;
  showPassword = false;
  isLoading = false;

  // Data máxima = hoje (não pode ser nascido no futuro)
  maxDate = new Date().toISOString().split('T')[0];

  togglePassword() {
    this.showPassword = !this.showPassword;
  }

  get passwordStrength(): number {
    const p = this.password;
    let score = 0;
    if (p.length >= 8) score++;
    if (/[A-Z]/.test(p)) score++;
    if (/[0-9]/.test(p)) score++;
    if (/[^A-Za-z0-9]/.test(p)) score++;
    return score;
  }

  get passwordStrengthClass(): string {
    switch (this.passwordStrength) {
      case 1: return 'weak';
      case 2: return 'fair';
      case 3: return 'good';
      case 4: return 'strong';
      default: return '';
    }
  }

  get passwordStrengthLabel(): string {
    switch (this.passwordStrength) {
      case 1: return 'Fraca';
      case 2: return 'Razoável';
      case 3: return 'Boa';
      case 4: return 'Forte';
      default: return '';
    }
  }

  onSubmit() {
    if (!this.name || !this.email || !this.password || !this.gender || !this.birthDate || !this.acceptTerms) return;
    this.isLoading = true;
    // TODO: chamar AuthService.register()
    setTimeout(() => {
      this.isLoading = false;
    }, 1500);
  }
}
