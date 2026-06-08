import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { WeeklyReportRequest, WeeklyReportResponse } from '../models/report.models';

@Injectable({ providedIn: 'root' })
export class ReportService {
  private readonly base = '/api/reports';

  constructor(private http: HttpClient) {}

  getAll() {
    return this.http.get<WeeklyReportResponse[]>(this.base, { withCredentials: true });
  }

  generate(request: WeeklyReportRequest) {
    return this.http.post<WeeklyReportResponse>(`${this.base}/generate`, request, { withCredentials: true });
  }

  getById(id: string) {
    return this.http.get<WeeklyReportResponse>(`${this.base}/${id}`, { withCredentials: true });
  }
}
