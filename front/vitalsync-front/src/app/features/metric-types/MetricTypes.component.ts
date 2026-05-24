import { Component, inject, signal, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MetricTypesService } from '../../core/services/metric-types.service';
import { MetricType } from '../../core/models/metric-type.models';

@Component({
  selector: 'app-metric-types',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './MetricTypes.component.html',
  styleUrl: './MetricTypes.component.scss',
})
export class MetricTypesComponent implements OnInit {
  private service = inject(MetricTypesService);

  metrics = signal<MetricType[]>([]);
  isLoading = signal(true);
  errorMessage = signal('');

  ngOnInit() {
    this.service.getAll().subscribe({
      next: data => { this.metrics.set(data); this.isLoading.set(false); },
      error: () => { this.errorMessage.set('Erro ao carregar métricas.'); this.isLoading.set(false); },
    });
  }
}
