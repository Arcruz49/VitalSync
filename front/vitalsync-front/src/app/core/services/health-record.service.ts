import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { HealthRecordRequest, HealthRecordResponse, SearchHealthRecordRequest } from '../models/health-record.models';

@Injectable({ providedIn: 'root' })
export class HealthRecordService {
  private readonly base = '/health-record';

  constructor(private http: HttpClient) {}

  getAll(filters: SearchHealthRecordRequest = {}) {
    let params = new HttpParams();
    if (filters.metricTypeId) params = params.set('metricTypeId', filters.metricTypeId);
    if (filters.from) params = params.set('from', filters.from);
    if (filters.to) params = params.set('to', filters.to);
    return this.http.get<HealthRecordResponse[]>(this.base, { params, withCredentials: true });
  }

  getById(id: string) {
    return this.http.get<HealthRecordResponse>(`${this.base}/${id}`, { withCredentials: true });
  }

  create(request: HealthRecordRequest) {
    return this.http.post<HealthRecordResponse>(this.base, request, { withCredentials: true });
  }

  update(id: string, request: HealthRecordRequest) {
    return this.http.put<void>(`${this.base}/${id}`, request, { withCredentials: true });
  }

  delete(id: string) {
    return this.http.delete<void>(`${this.base}/${id}`, { withCredentials: true });
  }
}
