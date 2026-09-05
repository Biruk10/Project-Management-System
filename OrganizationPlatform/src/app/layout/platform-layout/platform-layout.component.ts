import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { PlatformSidebarComponent } from './platform-sidebar.component';
import { PlatformNavbarComponent } from './platform-navbar.component';

@Component({
  selector: 'app-platform-layout',
  standalone: true,
  imports: [RouterOutlet, PlatformSidebarComponent, PlatformNavbarComponent],
  templateUrl: './platform-layout.component.html',
  styleUrl: './platform-layout.component.scss'
})
export class PlatformLayoutComponent {}
