import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-platform-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './platform-sidebar.component.html',
  styleUrl: './platform-sidebar.component.scss'
})
export class PlatformSidebarComponent {
  constructor(public authService: AuthService) {}
}
