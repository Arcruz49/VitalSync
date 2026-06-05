import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { Observable } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { RouterLink, ActivatedRoute } from '@angular/router';
import { HealthRecordService } from '../../core/services/health-record.service';
import { MetricTypesService } from '../../core/services/metric-types.service';
import { ProfileService } from '../../core/services/profile.service';
import { HealthRecordResponse } from '../../core/models/health-record.models';
import { MetricType } from '../../core/models/metric-type.models';
import { PersonalRangeResponse } from '../../core/models/profile.models';
import {
  isoToDisplayDateTime,
  isoToPickerDateTime,
  pickerDateToISO,
  pickerDateTimeToISO,
} from '../../core/utils/date.utils';

const METRIC_COLORS: Record<number, { color: string; bg: string }> = {
  1:  { color: '#EF4444', bg: '#FEE2E2' },
  2:  { color: '#EF4444', bg: '#FEE2E2' },
  3:  { color: '#F59E0B', bg: '#FEF3C7' },
  4:  { color: '#EC4899', bg: '#FCE7F3' },
  5:  { color: '#10B981', bg: '#D1FAE5' },
  6:  { color: '#06B6D4', bg: '#CFFAFE' },
  7:  { color: '#8B5CF6', bg: '#EDE9FE' },
  8:  { color: '#F59E0B', bg: '#FEF3C7' },
  9:  { color: '#EF4444', bg: '#FEE2E2' },
  10: { color: '#F59E0B', bg: '#FEF3C7' },
  11: { color: '#F97316', bg: '#FFEDD5' },
  12: { color: '#6366F1', bg: '#E0E7FF' },
};

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
  private profileService = inject(ProfileService);
  private route = inject(ActivatedRoute);

  records = signal<HealthRecordResponse[]>([]);
  metricTypes = signal<MetricType[]>([]);
  ranges = signal<PersonalRangeResponse[]>([]);
  loading = signal(false);
  saving = signal(false);
  error = signal('');
  editingId = signal<string | null>(null);
  showForm = false;
  expandedInsights = new Set<string>();

  filterMetricTypeId = '';
  filterFrom = '';
  filterTo = '';
  filterFromDisplay = '';
  filterToDisplay = '';

  updateFilterDisplay(which: 'from' | 'to', ymd: string) {
    const formatted = ymd ? (() => { const [y, m, d] = ymd.split('-'); return `${d}/${m}/${y}`; })() : '';
    if (which === 'from') this.filterFromDisplay = formatted;
    else this.filterToDisplay = formatted;
  }

  form: { metricTypeId: any; value: any; measuredAt: string; notes: string } =
    { metricTypeId: '', value: '', measuredAt: '', notes: '' };

  selectedMetric = computed(() =>
    this.metricTypes().find(m => m.id === +this.form.metricTypeId) ?? null
  );

  rangeLabel = computed(() => {
    const m = this.selectedMetric();
    if (!m) return '';
    const range = this.ranges().find(r => r.metricTypeId === m.id);
    if (range) return `${range.minNormal}–${range.maxNormal} ${m.unit}`;
    if (m.minNormal !== null && m.maxNormal !== null) return `${m.minNormal}–${m.maxNormal} ${m.unit}`;
    return m.unit;
  });

  ngOnInit() {
    this.mtService.getAll().subscribe(mt => this.metricTypes.set(mt));
    this.profileService.getPersonalRanges().subscribe({ next: r => this.ranges.set(r), error: () => {} });

    this.route.queryParams.subscribe(p => {
      if (p['metricTypeId']) this.filterMetricTypeId = p['metricTypeId'];
      this.load();
    });
  }

  onMetricChange() {}

  load() {
    this.loading.set(true);
    const filters: any = {};
    if (this.filterMetricTypeId) filters.metricTypeId = +this.filterMetricTypeId;
    if (this.filterFrom) { const iso = pickerDateToISO(this.filterFrom); if (iso) filters.from = iso; }
    if (this.filterTo)   { const iso = pickerDateToISO(this.filterTo);   if (iso) filters.to   = iso; }

    this.hrService.getAll(filters).subscribe({
      next: data => { this.records.set(data); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }

  clearFilters() {
    this.filterMetricTypeId = ''; this.filterFrom = ''; this.filterTo = '';
    this.filterFromDisplay = ''; this.filterToDisplay = '';
    this.load();
  }

  save() {
    if (!this.form.metricTypeId || !this.form.value || !this.form.measuredAt) {
      this.error.set('Preencha todos os campos obrigatórios.');
      return;
    }
    this.error.set('');
    this.saving.set(true);

    const req = {
      metricTypeId: +this.form.metricTypeId,
      value: +this.form.value,
      measuredAt: pickerDateTimeToISO(this.form.measuredAt),
      notes: this.form.notes || null,
    };

    const op: Observable<unknown> = this.editingId()
      ? this.hrService.update(this.editingId()!, req)
      : this.hrService.create(req);

    op.subscribe({
      next: () => { this.saving.set(false); this.closeForm(); this.load(); },
      error: () => { this.saving.set(false); this.error.set('Erro ao salvar.'); }
    });
  }

  startEdit(r: HealthRecordResponse) {
    this.editingId.set(r.id);
    this.showForm = true;
    this.form = {
      metricTypeId: r.metricTypeId,
      value: r.value,
      measuredAt: isoToPickerDateTime(r.measuredAt),
      notes: r.notes ?? '',
    };
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  closeForm() {
    this.showForm = false; this.editingId.set(null); this.error.set('');
    this.form = { metricTypeId: '', value: '', measuredAt: '', notes: '' };
  }

  pendingDeleteId = signal<string | null>(null);

  remove(id: string) { this.pendingDeleteId.set(id); }
  cancelDelete() { this.pendingDeleteId.set(null); }
  confirmDelete() {
    const id = this.pendingDeleteId();
    if (!id) return;
    this.pendingDeleteId.set(null);
    this.hrService.delete(id).subscribe({ next: () => this.load() });
  }

  toggleInsight(id: string) {
    if (this.expandedInsights.has(id)) this.expandedInsights.delete(id);
    else this.expandedInsights.add(id);
  }

  formatDate(iso: string) {
    return isoToDisplayDateTime(iso);
  }

  getColor(id: number): string { return (METRIC_COLORS[id] ?? { color: '#6366F1' }).color; }
  getColorBg(id: number): string { return (METRIC_COLORS[id] ?? { bg: '#E0E7FF' }).bg; }
}
