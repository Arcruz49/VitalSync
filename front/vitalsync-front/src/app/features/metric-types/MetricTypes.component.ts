import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MetricTypesService } from '../../core/services/metric-types.service';
import { MetricType } from '../../core/models/metric-type.models';

@Component({
  selector: 'app-metric-types',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './MetricTypes.component.html',
})
export class MetricTypesComponent implements OnInit {
  private service = inject(MetricTypesService);
  private router = inject(Router);

  metrics = signal<MetricType[]>([]);
  isLoading = signal(true);
  errorMessage = signal('');

  ngOnInit() {
    this.service.getAll().subscribe({
      next: (data) => {
        this.metrics.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('Erro ao carregar tipos de métricas.');
        this.isLoading.set(false);
      }
    });
  }

  goBack() {
    this.router.navigateByUrl('/dashboard');
  }
}
