import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet, ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { filter, map } from 'rxjs/operators';
import { AuthService } from '../../core/services/auth.service';
import { AlertService } from '../../core/services/alert.service';
import { SidebarComponent } from '../sidebar/Sidebar.component';

@Component({
  selector: 'app-auth-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, SidebarComponent],
  templateUrl: './AuthLayout.component.html',
  styleUrl: './AuthLayout.component.scss',
})
export class AuthLayoutComponent implements OnInit {
  private auth = inject(AuthService);
  private alertService = inject(AlertService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  sidebarOpen = false;
  alertCount = signal(0);
  pageTitle = signal('VitalSync.');

  get initials() {
    return (this.auth.currentUser()?.name ?? '')
      .split(' ').slice(0, 2).map((n: string) => n[0]).join('').toUpperCase();
  }

  ngOnInit() {
    this.alertService.getAll().subscribe({
      next: alerts => this.alertCount.set(alerts.length),
      error: () => {},
    });

    this.router.events.pipe(
      filter(e => e instanceof NavigationEnd),
      map(() => {
        let r = this.route;
        while (r.firstChild) r = r.firstChild;
        return r.snapshot.data['title'] ?? 'VitalSync.';
      }),
    ).subscribe(title => this.pageTitle.set(title));

    // set title on first load
    let r = this.route;
    while (r.firstChild) r = r.firstChild;
    this.pageTitle.set(r.snapshot.data['title'] ?? 'VitalSync.');
  }
}
