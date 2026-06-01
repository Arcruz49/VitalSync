import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { ToastService } from '../services/toast.service';

const SILENT_URLS = ['/auth/me'];
const SILENT_STATUSES = [401, 404];

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const toast = inject(ToastService);

  return next(req).pipe(
    catchError((err: HttpErrorResponse) => {
      const isSilentUrl = SILENT_URLS.some(u => req.url.includes(u));
      const isSilentStatus = SILENT_STATUSES.includes(err.status);

      if (!isSilentUrl && !isSilentStatus) {
        const message = err.error?.message ?? defaultMessage(err.status);
        toast.show(message, err.status >= 500 ? 'error' : 'warning');
      }

      return throwError(() => err);
    })
  );
};

function defaultMessage(status: number): string {
  if (status === 0) return 'Sem conexão com o servidor.';
  if (status === 400) return 'Requisição inválida.';
  if (status === 403) return 'Você não tem permissão para essa ação.';
  if (status === 422) return 'Dados inválidos. Verifique os campos.';
  if (status >= 500) return 'Erro interno do servidor. Tente novamente.';
  return `Erro inesperado (${status}).`;
}
