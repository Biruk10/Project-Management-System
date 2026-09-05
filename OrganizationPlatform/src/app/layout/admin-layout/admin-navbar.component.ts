import { Component, computed, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-admin-navbar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './admin-navbar.component.html',
  styleUrl: './admin-navbar.component.scss'
})
export class AdminNavbarComponent implements OnDestroy {
  now = new Date();
  private timer: ReturnType<typeof setInterval>;

  initials = computed(() => {
    const u = this.authService.currentUser();
    if (!u) return '?';
    return `${u.firstName[0]}${u.lastName[0]}`.toUpperCase();
  });

  constructor(public authService: AuthService) {
    this.timer = setInterval(() => this.now = new Date(), 30_000);
  }

  ngOnDestroy(): void {
    clearInterval(this.timer);
  }
}
