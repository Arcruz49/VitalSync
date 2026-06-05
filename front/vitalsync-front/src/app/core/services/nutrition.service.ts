import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { NutritionRequest, NutritionResponse, NutritionSummaryResponse } from '../models/nutrition.models';

@Injectable({ providedIn: 'root' })
export class NutritionService {
  private readonly base = '/api/nutrition';

  constructor(private http: HttpClient) {}

  getSummary(date: string) {
    const params = new HttpParams().set('date', date);
    return this.http.get<NutritionSummaryResponse>(`${this.base}/summary`, { params, withCredentials: true });
  }

  add(request: NutritionRequest) {
    return this.http.post<NutritionResponse>(this.base, request, { withCredentials: true });
  }

  delete(id: string) {
    return this.http.delete<void>(`${this.base}/${id}`, { withCredentials: true });
  }
}
