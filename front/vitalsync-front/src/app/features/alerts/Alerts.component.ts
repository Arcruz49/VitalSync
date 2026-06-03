import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AlertService } from '../../core/services/alert.service';
import { AlertResponse } from '../../core/models/alert.models';

@Component({
  selector: 'app-alerts',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './Alerts.component.html',
  styleUrl: './Alerts.component.scss',
})
export class AlertsComponent implements OnInit {
  private alertService = inject(AlertService);

  alerts = signal<AlertResponse[]>([]);
  loading = signal(true);

  ngOnInit() {
    this.alertService.getAll().subscribe({
      next: data => { this.alerts.set(data); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }

  get critical() { return this.alerts().filter(a => a.severity === 'Critical'); }
  get warning() { return this.alerts().filter(a => a.severity === 'Warning'); }

  formatDate(iso: string) {
    return new Date(iso).toLocaleString('pt-BR', {
      day: '2-digit', month: '2-digit', year: 'numeric',
      hour: '2-digit', minute: '2-digit',
    });
  }
}
