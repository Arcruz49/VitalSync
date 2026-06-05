import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { trigger, transition, style, animate, group, query } from '@angular/animations';
import { ProfileService } from '../../core/services/profile.service';
import {
  HEALTH_CONDITIONS, MEDICATION_CLASSES, ACTIVITY_LEVELS, HEALTH_GOALS,
  TRAINING_TYPES, HOURS_SEATED
} from '../../core/models/profile.models';

@Component({
  selector: 'app-onboarding',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './Onboarding.component.html',
  styleUrl: './Onboarding.component.scss',
  animations: [
    trigger('stepTransition', [
      transition(':increment', [
        group([
          query(':leave', [
            style({ position: 'absolute', top: 0, left: 0, right: 0 }),
            animate('180ms ease-in', style({ opacity: 0, transform: 'translateX(-32px)' }))
          ], { optional: true }),
          query(':enter', [
            style({ opacity: 0, transform: 'translateX(40px)' }),
            animate('350ms 60ms cubic-bezier(0.16, 1, 0.3, 1)', style({ opacity: 1, transform: 'translateX(0)' }))
          ], { optional: true }),
        ])
      ]),
      transition(':decrement', [
        group([
          query(':leave', [
            style({ position: 'absolute', top: 0, left: 0, right: 0 }),
            animate('180ms ease-in', style({ opacity: 0, transform: 'translateX(32px)' }))
          ], { optional: true }),
          query(':enter', [
            style({ opacity: 0, transform: 'translateX(-40px)' }),
            animate('350ms 60ms cubic-bezier(0.16, 1, 0.3, 1)', style({ opacity: 1, transform: 'translateX(0)' }))
          ], { optional: true }),
        ])
      ]),
    ])
  ]
})
export class OnboardingComponent {
  private profileService = inject(ProfileService);
  private router = inject(Router);

  step = 1;
  saving = false;
  errorMsg = '';

  stepNames = ['Dados físicos', 'Rotina', 'Saúde', 'Medicamentos'];
  goals = HEALTH_GOALS;
  activityLevels = ACTIVITY_LEVELS;
  trainingTypes = TRAINING_TYPES;
  hoursSeated = HOURS_SEATED;
  conditions = HEALTH_CONDITIONS;
  medications = MEDICATION_CLASSES;

  selectedConditions = new Set<number>();
  selectedMedications = new Set<number>();

  f = {
    weightKg: null as any,
    heightCm: null as any,
    targetWeightKg: null as any,
    waistCircumferenceCm: null as any,
    goal: 0,
    activityLevel: 0,
    trainingFrequencyDays: 3,
    trainingTypes: [] as string[],
    hoursSeated: 2,
    habitualSleepHours: 7,
    sleepQuality: 3,
    notes: '',
  };

  toggleTraining(t: string) {
    const i = this.f.trainingTypes.indexOf(t);
    if (i >= 0) this.f.trainingTypes.splice(i, 1);
    else this.f.trainingTypes.push(t);
  }

  toggleCondition(id: number) {
    if (this.selectedConditions.has(id)) this.selectedConditions.delete(id);
    else this.selectedConditions.add(id);
  }

  toggleMed(id: number) {
    if (this.selectedMedications.has(id)) {
      this.selectedMedications.delete(id);
      return;
    }
    if (id === 0) {
      this.selectedMedications.clear();
    } else {
      this.selectedMedications.delete(0);
    }
    this.selectedMedications.add(id);
  }

  next() {
    this.errorMsg = '';
    if (this.step === 1) {
      if (!this.f.weightKg || !this.f.heightCm || !this.f.goal || !this.f.activityLevel) {
        this.errorMsg = 'Preencha peso, altura, objetivo e nível de atividade.';
        return;
      }
    }
    this.step++;
  }

  prev() { this.step--; this.errorMsg = ''; }

  finish() {
    this.saving = true;
    this.errorMsg = '';

    const profileReq = {
      weightKg: this.f.weightKg,
      heightCm: this.f.heightCm,
      targetWeightKg: this.f.targetWeightKg || undefined,
      waistCircumferenceCm: this.f.waistCircumferenceCm || undefined,
      activityLevel: this.f.activityLevel,
      goal: this.f.goal,
      trainingFrequencyDays: this.f.trainingFrequencyDays,
      trainingTypes: this.f.trainingTypes,
      hoursSeated: this.f.hoursSeated,
      habitualSleepHours: this.f.habitualSleepHours,
      sleepQuality: this.f.sleepQuality,
    };

    const conditions = Array.from(this.selectedConditions).map(id => ({ conditionId: id }));
    const medications = Array.from(this.selectedMedications)
      .filter(id => id > 0)
      .map(id => ({ medicationClass: id, notes: '' }));

    this.profileService.saveProfile(profileReq).subscribe({
      next: () => {
        const calls: any[] = [];
        if (conditions.length > 0) calls.push(this.profileService.saveConditions(conditions));
        if (medications.length > 0) calls.push(this.profileService.saveMedications(medications));
        calls.push(this.profileService.recalculateRanges());

        if (calls.length > 0) {
          forkJoin(calls).subscribe({
            next: () => this.router.navigateByUrl('/dashboard'),
            error: () => this.router.navigateByUrl('/dashboard'),
          });
        } else {
          this.router.navigateByUrl('/dashboard');
        }
      },
      error: (err) => {
        this.errorMsg = err.error?.message ?? 'Erro ao salvar perfil. Tente novamente.';
        this.saving = false;
      }
    });
  }
}
