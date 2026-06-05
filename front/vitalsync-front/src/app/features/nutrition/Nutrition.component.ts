import { Component, inject, signal, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DecimalPipe } from '@angular/common';
import { NutritionService } from '../../core/services/nutrition.service';
import { ToastService } from '../../core/services/toast.service';
import { NutritionSummaryResponse, MEAL_TYPES } from '../../core/models/nutrition.models';
import { nowPickerDateTime, pickerDateTimeToISO } from '../../core/utils/date.utils';

@Component({
  selector: 'app-nutrition',
  standalone: true,
  imports: [FormsModule, DecimalPipe],
  templateUrl: './Nutrition.component.html',
  styleUrl: './Nutrition.component.scss',
})
export class NutritionComponent implements OnInit {
  private nutritionService = inject(NutritionService);
  private toast = inject(ToastService);

  summary = signal<NutritionSummaryResponse | null>(null);
  loading = signal(true);
  selectedDate = signal(todayISO());

  showModal = signal(false);
  submitting = signal(false);
  dragging = signal(false);

  readonly mealTypes = MEAL_TYPES;

  form = newForm();

  ngOnInit() {
    this.loadSummary();
  }

  loadSummary() {
    this.loading.set(true);
    this.nutritionService.getSummary(this.selectedDate()).subscribe({
      next: data => { this.summary.set(data); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }

  changeDate(offset: number) {
    const d = new Date(this.selectedDate() + 'T12:00:00');
    d.setDate(d.getDate() + offset);
    this.selectedDate.set(d.toISOString().slice(0, 10));
    this.loadSummary();
  }

  isToday(): boolean {
    return this.selectedDate() === todayISO();
  }

  dateLabel(): string {
    if (this.isToday()) return 'Hoje';
    const yesterday = new Date();
    yesterday.setDate(yesterday.getDate() - 1);
    if (this.selectedDate() === yesterday.toISOString().slice(0, 10)) return 'Ontem';
    return new Date(this.selectedDate() + 'T12:00:00').toLocaleDateString('pt-BR', { day: '2-digit', month: 'long' });
  }

  progress(consumed: number, goal: number): number {
    if (!goal) return 0;
    return Math.min((consumed / goal) * 100, 100);
  }

  progressClass(consumed: number, goal: number): string {
    if (!goal) return '';
    const pct = (consumed / goal) * 100;
    if (pct >= 100) return 'over';
    if (pct >= 85) return 'near';
    return '';
  }

  openModal() {
    this.form = newForm();
    this.showModal.set(true);
  }

  closeModal() {
    if (this.submitting()) return;
    this.showModal.set(false);
  }

  onFileChange(event: Event) {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (file) this.loadImage(file);
  }

  onDrop(event: DragEvent) {
    event.preventDefault();
    this.dragging.set(false);
    const file = event.dataTransfer?.files[0];
    if (file && file.type.startsWith('image/')) this.loadImage(file);
  }

  onDragOver(event: DragEvent) {
    event.preventDefault();
    this.dragging.set(true);
  }

  onDragLeave() {
    this.dragging.set(false);
  }

  private loadImage(file: File) {
    const reader = new FileReader();
    reader.onload = () => {
      const result = reader.result as string;
      this.form.imagePreview = result;
      this.form.imageBase64 = result.split(',')[1];
    };
    reader.readAsDataURL(file);
  }

  submit() {
    if (!this.form.imageBase64) {
      this.toast.show('Selecione uma imagem da refeição', 'warning');
      return;
    }
    this.submitting.set(true);
    this.nutritionService.add({
      mealType: +this.form.mealType,
      notes: this.form.notes || undefined,
      measuredAt: pickerDateTimeToISO(this.form.measuredAt),
      imageBase64: this.form.imageBase64,
    }).subscribe({
      next: () => {
        this.submitting.set(false);
        this.closeModal();
        this.loadSummary();
        this.toast.show('Refeição registrada!', 'success');
      },
      error: () => { this.submitting.set(false); },
    });
  }

  pendingDeleteId = signal<string | null>(null);

  deleteRecord(id: string) { this.pendingDeleteId.set(id); }
  cancelDelete() { this.pendingDeleteId.set(null); }
  confirmDelete() {
    const id = this.pendingDeleteId();
    if (!id) return;
    this.pendingDeleteId.set(null);
    this.nutritionService.delete(id).subscribe({
      next: () => { this.loadSummary(); this.toast.show('Registro excluído', 'success'); },
    });
  }

  mealLabel(key: string): string {
    return MEAL_TYPES.find(m => m.key === key)?.label ?? key;
  }

  formatTime(iso: string): string {
    const d = new Date(iso);
    return `${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}`;
  }

  confidenceLabel(c: number): string {
    if (c >= 0.8) return 'Alta confiança';
    if (c >= 0.5) return 'Média confiança';
    return 'Baixa confiança';
  }

  confidenceClass(c: number): string {
    if (c >= 0.8) return 'high';
    if (c >= 0.5) return 'medium';
    return 'low';
  }
}

function todayISO(): string {
  return new Date().toISOString().slice(0, 10);
}

function newForm() {
  return {
    mealType: 1,
    notes: '',
    measuredAt: nowPickerDateTime(),
    imageBase64: '',
    imagePreview: '',
  };
}
