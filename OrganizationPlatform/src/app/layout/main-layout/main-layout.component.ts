import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from '../navbar/navbar.component';
import { SidebarComponent } from '../sidebar/sidebar.component';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [RouterOutlet, NavbarComponent, SidebarComponent],
  template: `
    <div class="shell">
      <app-sidebar />
      <div class="shell-body">
        <app-navbar />
        <main class="shell-main">
          <div class="shell-content animate-in">
            <router-outlet />
          </div>
        </main>
      </div>
    </div>
  `,
  styles: [`
    .shell {
      display: flex;
      height: 100vh;
      overflow: hidden;
      background: var(--page-bg);
    }

    .shell-body {
      flex: 1;
      display: flex;
      flex-direction: column;
      overflow: hidden;
      min-width: 0;
    }

    .shell-main {
      flex: 1;
      overflow-y: auto;
      overflow-x: hidden;
    }

    .shell-content {
      padding: 28px 32px;
      max-width: 1400px;
      margin: 0 auto;
    }
  `]
})
export class MainLayoutComponent {}
