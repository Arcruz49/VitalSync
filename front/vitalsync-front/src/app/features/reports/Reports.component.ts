import { Component, inject, signal, OnInit, OnDestroy } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { ReportService } from '../../core/services/report.service';
import { ToastService } from '../../core/services/toast.service';
import { WeeklyReportResponse } from '../../core/models/report.models';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [DecimalPipe],
  templateUrl: './Reports.component.html',
  styleUrl: './Reports.component.scss',
})
export class ReportsComponent implements OnInit, OnDestroy {
  private reportService = inject(ReportService);
  private toast = inject(ToastService);

  reports = signal<WeeklyReportResponse[]>([]);
  loading = signal(true);
  generating = signal(false);
  expandedId = signal<string | null>(null);
  selectedWeekStart = signal(currentMondayISO());

  private pollInterval: ReturnType<typeof setInterval> | null = null;

  ngOnInit() {
    this.loadReports();
  }

  ngOnDestroy() {
    this.stopPolling();
  }

  loadReports() {
    this.loading.set(true);
    this.reportService.getAll().subscribe({
      next: data => {
        this.reports.set(data);
        this.loading.set(false);
        this.managePoll();
      },
      error: () => this.loading.set(false),
    });
  }

  generate() {
    if (this.generateButtonDisabled()) return;
    this.generating.set(true);
    this.reportService.generate({ weekStart: this.selectedWeekStart() }).subscribe({
      next: newReport => {
        this.generating.set(false);
        this.reports.update(list => [
          newReport,
          ...list.filter(r => r.weekStart.slice(0, 10) !== newReport.weekStart.slice(0, 10)),
        ]);
        this.toast.show('Relatório em geração!', 'success');
        this.managePoll();
      },
      error: () => {
        this.generating.set(false);
        this.toast.show('Erro ao gerar relatório', 'error');
      },
    });
  }

  toggleExpand(id: string) {
    this.expandedId.set(this.expandedId() === id ? null : id);
  }

  onWeekChange(event: Event) {
    this.selectedWeekStart.set((event.target as HTMLInputElement).value);
  }

  currentWeekReport(): WeeklyReportResponse | undefined {
    return this.reports().find(r => r.weekStart.slice(0, 10) === this.selectedWeekStart());
  }

  generateButtonDisabled(): boolean {
    const r = this.currentWeekReport();
    return this.generating() || r?.status === 'Completed' || r?.status === 'Pending';
  }

  generateButtonText(): string {
    if (this.generating()) return 'Gerando...';
    const r = this.currentWeekReport();
    if (r?.status === 'Completed') return 'Relatório já gerado';
    if (r?.status === 'Pending') return 'Gerando relatório...';
    return 'Gerar relatório';
  }

  isPendingWeek(): boolean {
    return this.currentWeekReport()?.status === 'Pending';
  }

  statusClass(status: string): string {
    if (status === 'Completed') return 'status-badge normal';
    if (status === 'Pending') return 'status-badge warning';
    return 'status-badge critical';
  }

  statusLabel(status: string): string {
    if (status === 'Completed') return 'Concluído';
    if (status === 'Pending') return 'Gerando';
    return 'Falhou';
  }

  trendIcon(trend: string): string {
    if (trend === 'improving') return '↑';
    if (trend === 'worsening') return '↓';
    return '→';
  }

  formatWeekRange(start: string, end: string): string {
    const s = new Date(start.slice(0, 10) + 'T12:00:00');
    const e = new Date(end.slice(0, 10) + 'T12:00:00');
    return `${s.toLocaleDateString('pt-BR', { day: '2-digit', month: '2-digit' })} a ${e.toLocaleDateString('pt-BR', { day: '2-digit', month: '2-digit' })}`;
  }

  private managePoll() {
    const hasPending = this.reports().some(r => r.status === 'Pending');
    if (hasPending && !this.pollInterval) {
      this.startPolling();
    } else if (!hasPending) {
      this.stopPolling();
    }
  }

  private startPolling() {
    this.pollInterval = setInterval(() => {
      this.reportService.getAll().subscribe({
        next: data => {
          this.reports.set(data);
          if (!data.some(r => r.status === 'Pending')) this.stopPolling();
        },
      });
    }, 5000);
  }

  private stopPolling() {
    if (this.pollInterval) {
      clearInterval(this.pollInterval);
      this.pollInterval = null;
    }
  }
}

function currentMondayISO(): string {
  const d = new Date();
  const day = d.getDay();
  const diff = day === 0 ? -6 : 1 - day;
  d.setDate(d.getDate() + diff);
  return d.toISOString().slice(0, 10);
}
