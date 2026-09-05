import { Component, computed, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-platform-navbar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './platform-navbar.component.html',
  styleUrl: './platform-navbar.component.scss'
})
export class PlatformNavbarComponent implements OnDestroy {
  now = new Date();
  private timer: ReturnType<typeof setInterval>;

  initials = computed(() => {
    const u = this.authService.currentUser();
    return u ? `${u.firstName[0]}${u.lastName[0]}`.toUpperCase() : '?';
  });

  constructor(public authService: AuthService) {
    this.timer = setInterval(() => this.now = new Date(), 30_000);
  }

  ngOnDestroy(): void {
    clearInterval(this.timer);
  }
}
