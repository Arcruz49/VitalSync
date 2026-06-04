import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AlertResponse } from '../models/alert.models';

@Injectable({ providedIn: 'root' })
export class AlertService {
  constructor(private http: HttpClient) {}

  getAll() {
    return this.http.get<AlertResponse[]>('/api/alerts', { withCredentials: true });
  }
}
