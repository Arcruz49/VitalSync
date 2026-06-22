import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DecimalPipe } from '@angular/common';
import { forkJoin } from 'rxjs';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { ProfileService } from '../../core/services/profile.service';
import {
  UserProfileResponse, UserConditionResponse, UserMedicationResponse,
  HEALTH_CONDITIONS, MEDICATION_CLASSES, ACTIVITY_LEVELS, HEALTH_GOALS,
  TRAINING_TYPES, HOURS_SEATED, PROFILE_PICS
} from '../../core/models/profile.models';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [DecimalPipe, FormsModule],
  templateUrl: './Profile.component.html',
  styleUrl: './Profile.component.scss',
})
export class ProfileComponent implements OnInit {
  auth = inject(AuthService);
  private profileService = inject(ProfileService);
  private router = inject(Router);

  loading = signal(true);
  profile = signal<UserProfileResponse | null>(null);
  conditions = signal<UserConditionResponse[]>([]);
  medications = signal<UserMedicationResponse[]>([]);

  editingPhysical = false;
  editingRoutine = false;
  editingConditions = false;
  editingMedications = false;
  showAvatarPicker = false;
  selectedPicId = -1;

  savingPhysical = signal(false);
  savingRoutine = signal(false);
  savingConditions = signal(false);
  savingMedications = signal(false);
  savingAvatar = signal(false);

  errorPhysical = signal('');
  errorRoutine = signal('');
  errorConditions = signal('');
  errorMedications = signal('');
  successMsg = signal('');

  confirmingDelete = false;
  deletingAccount = signal(false);

  pfPhysical: any = {};
  pfRoutine: any = {};
  selectedConditions = new Set<number>();
  selectedMedications = new Set<number>();

  allConditions = HEALTH_CONDITIONS;
  allMedications = MEDICATION_CLASSES.filter(m => m.id > 0);
  activityLevels = ACTIVITY_LEVELS;
  goals = HEALTH_GOALS;
  trainingTypes = TRAINING_TYPES;
  hoursSeated = HOURS_SEATED;
  profilePics = PROFILE_PICS;

  get initials() {
    return (this.auth.currentUser()?.name ?? '')
      .split(' ').slice(0, 2).map((n: string) => n[0]).join('').toUpperCase();
  }

  get currentPicSrc() {
    const p = this.profile();
    if (!p || p.profilePic === '_') return null;
    return PROFILE_PICS.find(pic => pic.key === p.profilePic)?.src ?? null;
  }

  private picKeyToId(key: string): number {
    return PROFILE_PICS.find(p => p.key === key)?.id ?? -1;
  }

ngOnInit() { this.load(); }

  load() {
    this.loading.set(true);
    this.profileService.getProfile().subscribe({
      next: (profile) => {
        this.profile.set(profile);
        this.profileService.updatePicFromKey(profile.profilePic);
        forkJoin({
          conditions: this.profileService.getConditions(),
          medications: this.profileService.getMedications(),
        }).subscribe({
          next: ({ conditions, medications }) => {
            this.conditions.set(conditions);
            this.medications.set(medications);
            this.loading.set(false);
          },
          error: () => this.loading.set(false),
        });
      },
      error: () => {
        this.loading.set(false);
        this.buildPhysicalForm();
        this.editingPhysical = true;
      },
    });
  }

  // ── AVATAR PICKER ─────────────────────────────────────
  openAvatarPicker() {
    const p = this.profile();
    this.selectedPicId = p ? this.picKeyToId(p.profilePic) : -1;
    this.showAvatarPicker = true;
  }

  cancelAvatarPicker() {
    this.showAvatarPicker = false;
  }

  saveAvatar() {
    const cur = this.profile()!;
    this.savingAvatar.set(true);
    this.profileService.saveProfile({
      weightKg: cur.weightKg,
      heightCm: cur.heightCm,
      targetWeightKg: cur.targetWeightKg ?? undefined,
      waistCircumferenceCm: cur.waistCircumferenceCm ?? undefined,
      activityLevel: ACTIVITY_LEVELS.find(a => a.apiKey === cur.activityLevel)?.id ?? 3,
      goal: HEALTH_GOALS.find(g => g.apiKey === cur.goal)?.id ?? 6,
      trainingFrequencyDays: cur.trainingFrequencyDays,
      trainingTypes: cur.trainingTypes,
      hoursSeated: HOURS_SEATED.find(h => h.apiKey === cur.hoursSeated)?.id ?? 2,
      habitualSleepHours: cur.habitualSleepHours,
      sleepQuality: cur.sleepQuality,
      profilePic: this.selectedPicId,
    }).subscribe({
      next: (updated) => {
        this.profile.set(updated);
        this.profileService.updatePicFromKey(updated.profilePic);
        this.showAvatarPicker = false;
        this.savingAvatar.set(false);
        this.flash('Ícone atualizado.');
      },
      error: () => this.savingAvatar.set(false),
    });
  }

  // ── SECTION 1: PHYSICAL + GOALS ──────────────────────
  private buildPhysicalForm() {
    const p = this.profile();
    this.pfPhysical = {
      weightKg: p?.weightKg ?? null,
      heightCm: p?.heightCm ?? null,
      targetWeightKg: p?.targetWeightKg ?? null,
      waistCircumferenceCm: p?.waistCircumferenceCm ?? null,
      activityLevel: p ? (ACTIVITY_LEVELS.find(a => a.apiKey === p.activityLevel)?.id ?? 0) : 0,
      goal: p ? (HEALTH_GOALS.find(g => g.apiKey === p.goal)?.id ?? 0) : 0,
    };
  }

  openEditPhysical() {
    this.buildPhysicalForm();
    this.errorPhysical.set('');
    this.editingPhysical = true;
  }

  cancelPhysical() {
    this.editingPhysical = false;
    this.errorPhysical.set('');
  }

  savePhysical() {
    if (!this.pfPhysical.weightKg || !this.pfPhysical.heightCm) {
      this.errorPhysical.set('Peso e altura são obrigatórios.');
      return;
    }
    if (!this.pfPhysical.activityLevel || !this.pfPhysical.goal) {
      this.errorPhysical.set('Selecione objetivo e nível de atividade.');
      return;
    }
    this.savingPhysical.set(true);
    this.errorPhysical.set('');

    const cur = this.profile();
    this.profileService.saveProfile({
      weightKg: this.pfPhysical.weightKg,
      heightCm: this.pfPhysical.heightCm,
      targetWeightKg: this.pfPhysical.targetWeightKg || undefined,
      waistCircumferenceCm: this.pfPhysical.waistCircumferenceCm || undefined,
      activityLevel: this.pfPhysical.activityLevel,
      goal: this.pfPhysical.goal,
      trainingFrequencyDays: cur?.trainingFrequencyDays ?? 3,
      trainingTypes: cur?.trainingTypes ?? [],
      hoursSeated: cur ? (HOURS_SEATED.find(h => h.apiKey === cur.hoursSeated)?.id ?? 2) : 2,
      habitualSleepHours: cur?.habitualSleepHours ?? 7,
      sleepQuality: cur?.sleepQuality ?? 3,
      profilePic: cur ? this.picKeyToId(cur.profilePic) : -1,
    }).subscribe({
      next: (updated) => {
        this.profile.set(updated);
        this.profileService.updatePicFromKey(updated.profilePic);
        this.editingPhysical = false;
        this.savingPhysical.set(false);
        this.flash('Dados físicos atualizados.');
        this.profileService.recalculateRanges().subscribe();
      },
      error: (e) => {
        this.errorPhysical.set(e.error?.message ?? 'Erro ao salvar.');
        this.savingPhysical.set(false);
      },
    });
  }

  // ── SECTION 2: ROUTINE ───────────────────────────────
  private buildRoutineForm() {
    const p = this.profile()!;
    this.pfRoutine = {
      trainingFrequencyDays: p.trainingFrequencyDays,
      trainingTypes: [...(p.trainingTypes ?? [])],
      hoursSeated: HOURS_SEATED.find(h => h.apiKey === p.hoursSeated)?.id ?? 2,
      habitualSleepHours: p.habitualSleepHours,
      sleepQuality: p.sleepQuality,
    };
  }

  openEditRoutine() {
    this.buildRoutineForm();
    this.errorRoutine.set('');
    this.editingRoutine = true;
  }

  cancelRoutine() {
    this.editingRoutine = false;
    this.errorRoutine.set('');
  }

  toggleTraining(t: string) {
    const i = this.pfRoutine.trainingTypes.indexOf(t);
    if (i >= 0) this.pfRoutine.trainingTypes.splice(i, 1);
    else this.pfRoutine.trainingTypes.push(t);
  }

  saveRoutine() {
    this.savingRoutine.set(true);
    this.errorRoutine.set('');

    const cur = this.profile()!;
    this.profileService.saveProfile({
      weightKg: cur.weightKg,
      heightCm: cur.heightCm,
      targetWeightKg: cur.targetWeightKg ?? undefined,
      waistCircumferenceCm: cur.waistCircumferenceCm ?? undefined,
      activityLevel: ACTIVITY_LEVELS.find(a => a.apiKey === cur.activityLevel)?.id ?? 3,
      goal: HEALTH_GOALS.find(g => g.apiKey === cur.goal)?.id ?? 6,
      trainingFrequencyDays: this.pfRoutine.trainingFrequencyDays,
      trainingTypes: this.pfRoutine.trainingTypes,
      hoursSeated: this.pfRoutine.hoursSeated,
      habitualSleepHours: this.pfRoutine.habitualSleepHours,
      sleepQuality: this.pfRoutine.sleepQuality,
      profilePic: this.picKeyToId(cur.profilePic),
    }).subscribe({
      next: (updated) => {
        this.profile.set(updated);
        this.profileService.updatePicFromKey(updated.profilePic);
        this.editingRoutine = false;
        this.savingRoutine.set(false);
        this.flash('Rotina de treino atualizada.');
        this.profileService.recalculateRanges().subscribe();
      },
      error: (e) => {
        this.errorRoutine.set(e.error?.message ?? 'Erro ao salvar.');
        this.savingRoutine.set(false);
      },
    });
  }

  // ── SECTION 3: CONDITIONS ────────────────────────────
  openEditConditions() {
    this.selectedConditions = new Set(this.conditions().map(c => c.conditionId));
    this.errorConditions.set('');
    this.editingConditions = true;
  }

  cancelConditions() {
    this.editingConditions = false;
    this.errorConditions.set('');
  }

  toggleCondition(id: number) {
    if (this.selectedConditions.has(id)) this.selectedConditions.delete(id);
    else this.selectedConditions.add(id);
  }

  saveConditions() {
    this.savingConditions.set(true);
    const list = Array.from(this.selectedConditions).map(id => ({ conditionId: id }));
    this.profileService.saveConditions(list).subscribe({
      next: () => {
        this.profileService.getConditions().subscribe(c => this.conditions.set(c));
        this.editingConditions = false;
        this.savingConditions.set(false);
        this.flash('Condições atualizadas.');
        this.profileService.recalculateRanges().subscribe();
      },
      error: () => {
        this.errorConditions.set('Erro ao salvar condições.');
        this.savingConditions.set(false);
      },
    });
  }

  // ── SECTION 4: MEDICATIONS ───────────────────────────
  openEditMedications() {
    const keyToId: Record<string, number> = {};
    MEDICATION_CLASSES.forEach(m => { keyToId[m.apiKey] = m.id; });
    this.selectedMedications = new Set(
      this.medications().map(m => keyToId[m.medicationClass] ?? 0).filter(id => id > 0)
    );
    this.errorMedications.set('');
    this.editingMedications = true;
  }

  cancelMedications() {
    this.editingMedications = false;
    this.errorMedications.set('');
  }

  toggleMed(id: number) {
    if (this.selectedMedications.has(id)) this.selectedMedications.delete(id);
    else this.selectedMedications.add(id);
  }

  saveMedications() {
    this.savingMedications.set(true);
    const list = Array.from(this.selectedMedications)
      .filter(id => id > 0)
      .map(id => ({ medicationClass: id, notes: '' }));
    this.profileService.saveMedications(list).subscribe({
      next: () => {
        this.profileService.getMedications().subscribe(m => this.medications.set(m));
        this.editingMedications = false;
        this.savingMedications.set(false);
        this.flash('Medicamentos atualizados.');
        this.profileService.recalculateRanges().subscribe();
      },
      error: () => {
        this.errorMedications.set('Erro ao salvar medicamentos.');
        this.savingMedications.set(false);
      },
    });
  }

  // ── HELPERS ──────────────────────────────────────────
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

  deleteAccount() {
    this.deletingAccount.set(true);
    this.auth.deleteAccount().subscribe({
      next: () => this.router.navigateByUrl('/login'),
      error: () => {
        this.deletingAccount.set(false);
        this.confirmingDelete = false;
      },
    });
  }

  private flash(msg: string) {
    this.successMsg.set(msg);
    setTimeout(() => this.successMsg.set(''), 3000);
  }
}
