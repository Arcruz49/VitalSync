import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap, catchError, of } from 'rxjs';
import { LoginRequest, RegisterRequest, AuthResponse } from '../models/auth.models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly base = '/api/auth';

  currentUser = signal<{ name: string; email: string } | null>(null);

  constructor(private http: HttpClient) {}

  login(request: LoginRequest) {
    return this.http
      .post<AuthResponse>(`${this.base}/login`, request, { withCredentials: true })
      .pipe(tap(res => this.currentUser.set({ name: res.name, email: res.email })));
  }

  register(request: RegisterRequest) {
    return this.http
      .post<AuthResponse>(`${this.base}/register`, request, { withCredentials: true })
      .pipe(tap(res => this.currentUser.set({ name: res.name, email: res.email })));
  }

  me() {
    return this.http
      .get<{ id: string; name: string }>(`${this.base}/me`, { withCredentials: true })
      .pipe(
        tap(res => this.currentUser.set({ name: res.name, email: '' })),
        catchError(() => {
          this.currentUser.set(null);
          return of(null);
        })
      );
  }

  logout() {
    return this.http
      .post(`${this.base}/logout`, {}, { withCredentials: true })
      .pipe(tap(() => this.currentUser.set(null)));
  }

  forgotPassword(email: string) {
    return this.http.post(`${this.base}/forgot-password`, null, {
      params: { email },
      withCredentials: true,
      responseType: 'text' as const,
    });
  }

  resetPassword(token: string, password: string) {
    return this.http.post(`${this.base}/reset-password`, { token, password }, {
      withCredentials: true,
    });
  }

  deleteAccount() {
    return this.http
      .delete(`${this.base}`, { withCredentials: true })
      .pipe(tap(() => this.currentUser.set(null)));
  }
}
