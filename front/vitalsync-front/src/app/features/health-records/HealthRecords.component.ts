import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { Observable } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { HealthRecordService } from '../../core/services/health-record.service';
import { MetricTypesService } from '../../core/services/metric-types.service';
import { HealthRecordResponse } from '../../core/models/health-record.models';
import { MetricType } from '../../core/models/metric-type.models';

@Component({
  selector: 'app-health-records',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './HealthRecords.component.html',
  styleUrl: './HealthRecords.component.scss',
})
export class HealthRecordsComponent implements OnInit {
  private hrService = inject(HealthRecordService);
  private mtService = inject(MetricTypesService);

  records = signal<HealthRecordResponse[]>([]);
  metricTypes = signal<MetricType[]>([]);
  loading = signal(false);
  saving = signal(false);
  error = signal('');
  editingId = signal<string | null>(null);
  showForm = false;
  sidebarOpen = false;

  filterMetricTypeId = '';
  filterFrom = '';
  filterTo = '';

  form = { metricTypeId: '' as any, value: '' as any, measuredAt: '', notes: '' };

  selectedMetric = computed(() =>
    this.metricTypes().find(m => m.id === +this.form.metricTypeId) ?? null
  );

  ngOnInit() {
    this.mtService.getAll().subscribe(mt => this.metricTypes.set(mt));
    this.loadRecords();
  }

  onMetricChange() {
    // trigger computed update
  }

  loadRecords() {
    this.loading.set(true);
    const filters: any = {};
    if (this.filterMetricTypeId) filters.metricTypeId = +this.filterMetricTypeId;
    if (this.filterFrom) filters.from = new Date(this.filterFrom).toISOString();
    if (this.filterTo) filters.to = new Date(this.filterTo).toISOString();

    this.hrService.getAll(filters).subscribe({
      next: data => { this.records.set(data); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }

  clearFilters() {
    this.filterMetricTypeId = '';
    this.filterFrom = '';
    this.filterTo = '';
    this.loadRecords();
  }

  save() {
    if (!this.form.metricTypeId || !this.form.value || !this.form.measuredAt) {
      this.error.set('Preencha os campos obrigatórios.');
      return;
    }
    this.error.set('');
    this.saving.set(true);

    const request = {
      metricTypeId: +this.form.metricTypeId,
      value: +this.form.value,
      measuredAt: new Date(this.form.measuredAt).toISOString(),
      notes: this.form.notes || null,
    };

    const op: Observable<unknown> = this.editingId()
      ? this.hrService.update(this.editingId()!, request)
      : this.hrService.create(request);

    op.subscribe({
      next: () => {
        this.saving.set(false);
        this.closeForm();
        this.loadRecords();
      },
      error: () => { this.saving.set(false); this.error.set('Erro ao salvar. Tente novamente.'); },
    });
  }

  startEdit(r: HealthRecordResponse) {
    this.editingId.set(r.id);
    this.showForm = true;
    this.form = {
      metricTypeId: r.metricTypeId,
      value: r.value,
      measuredAt: r.measuredAt.slice(0, 16),
      notes: r.notes ?? '',
    };
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  closeForm() {
    this.showForm = false;
    this.editingId.set(null);
    this.error.set('');
    this.form = { metricTypeId: '', value: '', measuredAt: '', notes: '' };
  }

  remove(id: string) {
    if (!confirm('Excluir este registro?')) return;
    this.hrService.delete(id).subscribe({ next: () => this.loadRecords() });
  }

  formatDate(iso: string) {
    return new Date(iso).toLocaleString('pt-BR', {
      day: '2-digit', month: '2-digit', year: 'numeric',
      hour: '2-digit', minute: '2-digit',
    });
  }
}
