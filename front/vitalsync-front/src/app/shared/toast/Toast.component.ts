import { Component, inject } from '@angular/core';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-toast',
  standalone: true,
  templateUrl: './Toast.component.html',
  styleUrl: './Toast.component.scss',
})
export class ToastComponent {
  toastService = inject(ToastService);
}
