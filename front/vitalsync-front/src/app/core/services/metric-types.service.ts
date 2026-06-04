import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MetricType } from '../models/metric-type.models';

@Injectable({ providedIn: 'root' })
export class MetricTypesService {
  private readonly base = '/api/metrics';

  constructor(private http: HttpClient) {}

  getAll() {
    return this.http.get<MetricType[]>(this.base, { withCredentials: true });
  }
}
