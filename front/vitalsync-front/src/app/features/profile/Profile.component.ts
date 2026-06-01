import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { forkJoin } from 'rxjs';
import { AuthService } from '../../core/services/auth.service';
import { ProfileService } from '../../core/services/profile.service';
import { SidebarComponent } from '../../shared/sidebar/Sidebar.component';
import {
  UserProfileResponse, UserConditionResponse, UserMedicationResponse,
  HEALTH_CONDITIONS, MEDICATION_CLASSES, ACTIVITY_LEVELS, HEALTH_GOALS,
  TRAINING_TYPES, HOURS_SEATED
} from '../../core/models/profile.models';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [RouterLink, SidebarComponent, DecimalPipe, FormsModule],
  templateUrl: './Profile.component.html',
  styleUrl: './Profile.component.scss',
})
export class ProfileComponent implements OnInit {
  auth = inject(AuthService);
  private profileService = inject(ProfileService);

  loading = signal(true);
  saving = signal(false);
  saveError = signal('');
  successMsg = signal('');

  profile = signal<UserProfileResponse | null>(null);
  conditions = signal<UserConditionResponse[]>([]);
  medications = signal<UserMedicationResponse[]>([]);

  // Edit panels open/close
  editingProfile = false;
  editingConditions = false;
  editingMedications = false;

  // Static data
  allConditions = HEALTH_CONDITIONS;
  allMedications = MEDICATION_CLASSES;
  activityLevels = ACTIVITY_LEVELS;
  goals = HEALTH_GOALS;
  trainingTypes = TRAINING_TYPES;
  hoursSeated = HOURS_SEATED;

  // Edit forms
  pf: any = {};
  selectedConditions = new Set<number>();
  selectedMedications = new Set<number>();

  get initials() {
    return (this.auth.currentUser()?.name ?? '')
      .split(' ').slice(0, 2).map((n: string) => n[0]).join('').toUpperCase();
  }

  heartRateZones = computed(() => {
    const maxHr = 188;
    return [
      { zone: 1, label: 'Recuperação',   pct: [0.50, 0.60], color: '#059669', bg: '#F0FDF4' },
      { zone: 2, label: 'Aeróbica base', pct: [0.60, 0.70], color: '#0D9488', bg: '#F0FDFA' },
      { zone: 3, label: 'Aeróbica',      pct: [0.70, 0.80], color: '#D97706', bg: '#FFFBEB' },
      { zone: 4, label: 'Limiar',        pct: [0.80, 0.90], color: '#EA580C', bg: '#FFF7ED' },
      { zone: 5, label: 'Máximo',        pct: [0.90, 1.00], color: '#DC2626', bg: '#FEF2F2' },
    ].map(z => ({ ...z, min: Math.round(maxHr * z.pct[0]), max: Math.round(maxHr * z.pct[1]) }));
  });

  ngOnInit() { this.load(); }

  load() {
    this.loading.set(true);
    forkJoin({
      profile: this.profileService.getProfile(),
      conditions: this.profileService.getConditions(),
      medications: this.profileService.getMedications(),
    }).subscribe({
      next: ({ profile, conditions, medications }) => {
        this.profile.set(profile);
        this.conditions.set(conditions);
        this.medications.set(medications);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  // ── EDIT PROFILE ─────────────────────────────────────
  openEditProfile() {
    const p = this.profile();
    if (!p) return;
    this.pf = {
      weightKg: p.weightKg,
      heightCm: p.heightCm,
      targetWeightKg: p.targetWeightKg ?? null,
      waistCircumferenceCm: p.waistCircumferenceCm ?? null,
      goal: HEALTH_GOALS.find(g => g.name === p.goal)?.id ?? 6,
      activityLevel: ACTIVITY_LEVELS.find(a => a.name === p.activityLevel)?.id ?? 3,
      trainingFrequencyDays: p.trainingFrequencyDays,
      trainingTypes: [...(p.trainingTypes ?? [])],
      hoursSeated: HOURS_SEATED.find(h => h.label === p.hoursSeated)?.id ?? 2,
      habitualSleepHours: p.habitualSleepHours,
      sleepQuality: p.sleepQuality,
    };
    this.editingProfile = true;
    this.saveError.set('');
  }

  toggleTraining(t: string) {
    const i = this.pf.trainingTypes.indexOf(t);
    if (i >= 0) this.pf.trainingTypes.splice(i, 1);
    else this.pf.trainingTypes.push(t);
  }

  saveProfile() {
    if (!this.pf.weightKg || !this.pf.heightCm) {
      this.saveError.set('Peso e altura são obrigatórios.');
      return;
    }
    this.saving.set(true);
    this.saveError.set('');

    this.profileService.saveProfile(this.pf).subscribe({
      next: (updated) => {
        this.profile.set(updated);
        this.editingProfile = false;
        this.saving.set(false);
        this.flash('Perfil atualizado com sucesso.');
        this.profileService.recalculateRanges().subscribe();
      },
      error: (e) => {
        this.saveError.set(e.error?.message ?? 'Erro ao salvar. Tente novamente.');
        this.saving.set(false);
      }
    });
  }

  // ── EDIT CONDITIONS ───────────────────────────────────
  openEditConditions() {
    this.selectedConditions = new Set(this.conditions().map(c => c.conditionId));
    this.editingConditions = true;
    this.saveError.set('');
  }

  toggleCondition(id: number) {
    if (this.selectedConditions.has(id)) this.selectedConditions.delete(id);
    else this.selectedConditions.add(id);
  }

  saveConditions() {
    this.saving.set(true);
    const list = Array.from(this.selectedConditions).map(id => ({ conditionId: id }));
    this.profileService.saveConditions(list).subscribe({
      next: () => {
        this.profileService.getConditions().subscribe(c => this.conditions.set(c));
        this.editingConditions = false;
        this.saving.set(false);
        this.flash('Condições atualizadas.');
        this.profileService.recalculateRanges().subscribe();
      },
      error: () => { this.saveError.set('Erro ao salvar condições.'); this.saving.set(false); }
    });
  }

  // ── EDIT MEDICATIONS ──────────────────────────────────
  openEditMedications() {
    const NAMES: Record<string, number> = {};
    MEDICATION_CLASSES.forEach(m => { NAMES[m.name] = m.id; });
    this.selectedMedications = new Set(
      this.medications().map(m => NAMES[m.medicationClass] ?? 0).filter(id => id > 0)
    );
    this.editingMedications = true;
    this.saveError.set('');
  }

  toggleMed(id: number) {
    if (this.selectedMedications.has(id)) this.selectedMedications.delete(id);
    else this.selectedMedications.add(id);
  }

  saveMedications() {
    this.saving.set(true);
    const list = Array.from(this.selectedMedications).filter(id => id > 0).map(id => ({ medicationClass: id, notes: '' }));
    this.profileService.saveMedications(list).subscribe({
      next: () => {
        this.profileService.getMedications().subscribe(m => this.medications.set(m));
        this.editingMedications = false;
        this.saving.set(false);
        this.flash('Medicamentos atualizados.');
        this.profileService.recalculateRanges().subscribe();
      },
      error: () => { this.saveError.set('Erro ao salvar medicamentos.'); this.saving.set(false); }
    });
  }

  bmiClass(bmi: number) {
    if (bmi < 18.5) return 'low';
    if (bmi < 25) return 'normal';
    if (bmi < 30) return 'warning';
    return 'critical';
  }

  bmiLabel(bmi: number) {
    if (bmi < 18.5) return 'Abaixo do peso';
    if (bmi < 25) return 'Normal';
    if (bmi < 30) return 'Sobrepeso';
    if (bmi < 35) return 'Obesidade I';
    return 'Obesidade II';
  }

  private flash(msg: string) {
    this.successMsg.set(msg);
    setTimeout(() => this.successMsg.set(''), 3000);
  }
}
