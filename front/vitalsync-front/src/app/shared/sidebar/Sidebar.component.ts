import { Component, inject, Input, Output, EventEmitter } from '@angular/core';
import { RouterLink, RouterLinkActive, Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { ThemeService } from '../../core/services/theme.service';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './Sidebar.component.html',
  styleUrl: './Sidebar.component.scss',
})
export class SidebarComponent {
  auth = inject(AuthService);
  theme = inject(ThemeService);
  private router = inject(Router);

  @Input() alertCount = 0;
  @Input() mobileOpen = false;
  @Output() mobileOpenChange = new EventEmitter<boolean>();

  close() { this.mobileOpenChange.emit(false); }

  toggleTheme(event: MouseEvent) {
    if (typeof document === 'undefined' || !('startViewTransition' in document)) {
      this.theme.toggle();
      return;
    }
    const btn = event.currentTarget as HTMLElement;
    const { left, top, width, height } = btn.getBoundingClientRect();
    const x = left + width / 2;
    const y = top + height / 2;
    const radius = Math.hypot(Math.max(x, innerWidth - x), Math.max(y, innerHeight - y));
    const vt = (document as any).startViewTransition(() => this.theme.toggle());
    vt.ready.then(() => {
      document.documentElement.animate(
        { clipPath: [`circle(0px at ${x}px ${y}px)`, `circle(${radius}px at ${x}px ${y}px)`] },
        { duration: 450, easing: 'ease-out', pseudoElement: '::view-transition-new(root)' }
      );
    });
  }

  get initials(): string {
    return (this.auth.currentUser()?.name ?? '')
      .split(' ').slice(0, 2).map((n: string) => n[0]).join('').toUpperCase();
  }

  logout() {
    this.auth.logout().subscribe({ next: () => this.router.navigateByUrl('/login') });
  }
}
