import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { AuthService } from '../../core/services/auth.service';
import { HealthRecordService } from '../../core/services/health-record.service';
import { MetricTypesService } from '../../core/services/metric-types.service';
import { HealthRecordResponse } from '../../core/models/health-record.models';
import { MetricType } from '../../core/models/metric-type.models';

interface MetricSummary {
  metricTypeId: number;
  name: string;
  unit: string;
  lastValue: number;
  lastDate: string;
  minNormal: number | null;
  maxNormal: number | null;
  hasAlert: boolean;
  isCritical: boolean;
  rangePercent: number;
  iconEmoji: string;
  iconBg: string;
}

const METRIC_ICONS: Record<string, { emoji: string; bg: string }> = {
  'blood-pressure': { emoji: '🩺', bg: '#FEF2F2' },
  'droplet':        { emoji: '💧', bg: '#EFF6FF' },
  'heart-pulse':    { emoji: '❤️', bg: '#FFF1F2' },
  'scale':          { emoji: '⚖️', bg: '#F0FDF4' },
  'lungs':          { emoji: '🫁', bg: '#F0FDFA' },
  'moon':           { emoji: '🌙', bg: '#F5F3FF' },
  'smile':          { emoji: '😊', bg: '#FFFBEB' },
};

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './Dashboard.component.html',
  styleUrl: './Dashboard.component.scss',
})
export class DashboardComponent implements OnInit {
  auth = inject(AuthService);
  private hrService = inject(HealthRecordService);
  private mtService = inject(MetricTypesService);
  private router = inject(Router);

  loading = signal(true);
  metricSummary = signal<MetricSummary[]>([]);
  sidebarOpen = false;

  get greeting(): string {
    const h = new Date().getHours();
    if (h < 12) return 'Bom dia';
    if (h < 18) return 'Boa tarde';
    return 'Boa noite';
  }

  get firstName(): string {
    return this.auth.currentUser()?.name?.split(' ')[0] ?? '';
  }

  get userInitials(): string {
    const name = this.auth.currentUser()?.name ?? '';
    return name.split(' ').slice(0, 2).map(n => n[0]).join('').toUpperCase();
  }

  ngOnInit() {
    forkJoin({
      records: this.hrService.getAll({}),
      types: this.mtService.getAll(),
    }).subscribe({
      next: ({ records, types }) => {
        this.metricSummary.set(this.buildSummary(records, types));
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  private buildSummary(records: HealthRecordResponse[], types: MetricType[]): MetricSummary[] {
    const latest = new Map<number, HealthRecordResponse>();
    for (const r of records) {
      const existing = latest.get(r.metricTypeId);
      if (!existing || new Date(r.measuredAt) > new Date(existing.measuredAt)) {
        latest.set(r.metricTypeId, r);
      }
    }

    return Array.from(latest.values()).map(r => {
      const type = types.find(t => t.id === r.metricTypeId);
      const icon = METRIC_ICONS[type?.icon ?? ''] ?? { emoji: '📊', bg: '#F8FAFC' };

      const hasMin = type?.minNormal !== null && type?.minNormal !== undefined;
      const hasMax = type?.maxNormal !== null && type?.maxNormal !== undefined;
      const hasAlert = (hasMax && r.value > type!.maxNormal!) || (hasMin && r.value < type!.minNormal!);

      let isCritical = false;
      if (hasAlert && type) {
        const limit = r.value > (type.maxNormal ?? 0) ? type.maxNormal! : type.minNormal!;
        isCritical = Math.abs((r.value - limit) / limit * 100) >= 20;
      }

      let rangePercent = 50;
      if (hasMin && hasMax && type) {
        const range = type.maxNormal! - type.minNormal!;
        const padding = range * 0.5;
        const total = range + padding * 2;
        rangePercent = Math.min(100, Math.max(0,
          ((r.value - (type.minNormal! - padding)) / total) * 100
        ));
      }

      return {
        metricTypeId: r.metricTypeId,
        name: r.metricTypeName,
        unit: r.unit,
        lastValue: r.value,
        lastDate: new Date(r.measuredAt).toLocaleDateString('pt-BR', { day: '2-digit', month: 'short', hour: '2-digit', minute: '2-digit' }),
        minNormal: type?.minNormal ?? null,
        maxNormal: type?.maxNormal ?? null,
        hasAlert,
        isCritical,
        rangePercent,
        iconEmoji: icon.emoji,
        iconBg: icon.bg,
      };
    });
  }

  goToRecords(metricTypeId: number) {
    this.router.navigate(['/health-records'], { queryParams: { metricTypeId } });
  }

  logout() {
    this.auth.logout().subscribe({ next: () => this.router.navigateByUrl('/login') });
  }
}
