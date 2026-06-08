import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { UpperCasePipe } from '@angular/common';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../../core/services/auth.service';
import { HealthRecordService } from '../../core/services/health-record.service';
import { MetricTypesService } from '../../core/services/metric-types.service';
import { ProfileService } from '../../core/services/profile.service';
import { HealthRecordResponse, AIAnalysisResult } from '../../core/models/health-record.models';
import { MetricType } from '../../core/models/metric-type.models';
import { PersonalRangeResponse } from '../../core/models/profile.models';

interface MetricCard {
  metricTypeId: number;
  name: string;
  unit: string;
  displayValue: string;
  date: string;
  status: 'normal' | 'warning' | 'critical';
  statusLabel: string;
  minNormal: number | null;
  maxNormal: number | null;
  markerPercent: number;
  personalRange?: string;
  trendLabel?: string;
  trendBad?: boolean;
  goalLabel?: string | null;
  progressPercent?: number | null;
}

const PRIORITY_METRICS = [1, 2, 3, 4, 5, 7, 11, 6, 9, 10, 8, 12];

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [RouterLink, UpperCasePipe],
  templateUrl: './Dashboard.component.html',
  styleUrl: './Dashboard.component.scss',
})
export class DashboardComponent implements OnInit {
  auth = inject(AuthService);
  private hrService = inject(HealthRecordService);
  private mtService = inject(MetricTypesService);
  private profileService = inject(ProfileService);
  private router = inject(Router);

  loading = signal(true);
  alertCount = signal(0);
  metricSummary = signal<MetricCard[]>([]);
  recentActivity = signal<any[]>([]);
  latestInsight = signal<AIAnalysisResult | null>(null);
  tipsOpen = false;

  hero = computed(() => this.metricSummary()[0] ?? null);
  mediumMetrics = computed(() => this.metricSummary().slice(1, 3));
  smallMetrics = computed(() => this.metricSummary().slice(3, 6));
  secondaryMetrics = computed(() => this.metricSummary().slice(6));

  get firstName() { return this.auth.currentUser()?.name?.split(' ')[0] ?? ''; }
  get initials() {
    return (this.auth.currentUser()?.name ?? '')
      .split(' ').slice(0, 2).map((n: string) => n[0]).join('').toUpperCase();
  }
  get today() {
    return new Date().toLocaleDateString('pt-BR', { weekday: 'long', day: 'numeric', month: 'long' });
  }

  ngOnInit() {
    forkJoin({
      records: this.hrService.getAll({}),
      types: this.mtService.getAll(),
      ranges: this.profileService.getPersonalRanges().pipe(catchError(() => of([]))),
    }).subscribe({
      next: ({ records, types, ranges }) => {
        const summary = this.buildSummary(records, types, ranges);
        this.metricSummary.set(summary);
        this.alertCount.set(summary.filter(m => m.status !== 'normal').length);
        this.recentActivity.set(this.buildActivity(records, types, ranges));
        // records já vêm ordenados do mais recente ao mais antigo
        const withInsight = records.find(r => r.aiAnalysisResult);
        if (withInsight?.aiAnalysisResult) this.latestInsight.set(withInsight.aiAnalysisResult);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  private buildSummary(records: HealthRecordResponse[], types: MetricType[], ranges: PersonalRangeResponse[]): MetricCard[] {
    const latest = new Map<number, HealthRecordResponse>();
    for (const r of records) {
      const ex = latest.get(r.metricTypeId);
      if (!ex || new Date(r.measuredAt) > new Date(ex.measuredAt)) latest.set(r.metricTypeId, r);
    }

    const cards: MetricCard[] = [];
    for (const id of PRIORITY_METRICS) {
      const r = latest.get(id);
      if (!r) continue;
      const type = types.find(t => t.id === id);
      const range = ranges.find(rg => rg.metricTypeId === id);
      const min = range?.minNormal ?? type?.minNormal ?? null;
      // maxNormal=0 means the metric has no upper bound (null was stored as 0 in PersonalRange)
      const rawMax = range?.maxNormal ?? type?.maxNormal ?? null;
      const max = rawMax === 0 ? null : rawMax;

      const status = this.getStatus(r.value, min, max);
      const rangeLabel = range
        ? (max !== null ? `${range.minNormal}–${max} ${r.unit}` : `≥ ${range.minNormal} ${r.unit}`)
        : undefined;
      cards.push({
        metricTypeId: id,
        name: r.metricTypeName,
        unit: r.unit,
        displayValue: r.value % 1 === 0 ? r.value.toString() : r.value.toFixed(1),
        date: new Date(r.measuredAt).toLocaleDateString('pt-BR', { day: '2-digit', month: 'short', hour: '2-digit', minute: '2-digit' }),
        status,
        statusLabel: status === 'normal' ? 'Normal' : status === 'warning' ? 'Atenção' : 'Crítico',
        minNormal: min,
        maxNormal: max,
        markerPercent: this.getMarker(r.value, min, max),
        personalRange: rangeLabel,
      });
    }

    // Sort: critical first, then warning, then normal
    return cards.sort((a, b) => {
      const order = { critical: 0, warning: 1, normal: 2 };
      return order[a.status] - order[b.status];
    });
  }

  private buildActivity(records: HealthRecordResponse[], types: MetricType[], ranges: PersonalRangeResponse[]) {
    return [...records]
      .sort((a, b) => new Date(b.measuredAt).getTime() - new Date(a.measuredAt).getTime())
      .slice(0, 5)
      .map(r => {
        const range = ranges.find(rg => rg.metricTypeId === r.metricTypeId);
        const type = types.find(t => t.id === r.metricTypeId);
        const min = range?.minNormal ?? type?.minNormal ?? null;
        const rawMax = range?.maxNormal ?? type?.maxNormal ?? null;
        const max = rawMax === 0 ? null : rawMax;
        const status = this.getStatus(r.value, min, max);
        return {
          id: r.id,
          metricTypeId: r.metricTypeId,
          metricTypeName: r.metricTypeName,
          unit: r.unit,
          value: r.value % 1 === 0 ? r.value : r.value.toFixed(1),
          date: new Date(r.measuredAt).toLocaleDateString('pt-BR', { day: '2-digit', month: 'short', hour: '2-digit', minute: '2-digit' }),
          status,
          statusLabel: status === 'normal' ? 'Normal' : status === 'warning' ? 'Atenção' : 'Crítico',
        };
      });
  }

  private getStatus(value: number, min: number | null, max: number | null): 'normal' | 'warning' | 'critical' {
    if (min === null && max === null) return 'normal';
    const aboveMax = max !== null && value > max;
    const belowMin = min !== null && value < min;
    if (!aboveMax && !belowMin) return 'normal';
    const limit = aboveMax ? max! : min!;
    const deviation = Math.abs((value - limit) / limit * 100);
    return deviation >= 20 ? 'critical' : 'warning';
  }

  private getMarker(value: number, min: number | null, max: number | null): number {
    if (min === null || max === null) return 50;
    const padding = (max - min) * 0.4;
    const low = min - padding;
    const high = max + padding;
    return Math.min(96, Math.max(4, ((value - low) / (high - low)) * 100));
  }

  goToRecords(id: number) {
    this.router.navigate(['/health-records'], { queryParams: { metricTypeId: id } });
  }
}
