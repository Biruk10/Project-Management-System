import { Component, signal } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/auth/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, RouterLink],
  template: `
    <div class="auth-shell">
      <div class="auth-left">
        <div class="auth-brand">
          <div class="auth-logo">⬡</div>
          <span class="auth-logo-name">OrgPlatform</span>
        </div>
        <div class="auth-hero">
          <h2>Manage your organization with clarity</h2>
          <p>Projects, tasks, budgets and teams — all in one place.</p>
          <div class="auth-features">
            <div class="feat">✦ Multi-tenant isolation</div>
            <div class="feat">✦ Role-based access control</div>
            <div class="feat">✦ Real-time dashboards</div>
          </div>
        </div>
      </div>

      <div class="auth-right">
        <div class="auth-card">
          <div class="auth-card-header">
            <h1>Welcome back</h1>
            <p>Sign in to your organization</p>
          </div>

          @if (error()) {
            <div class="alert alert-error">⚠ {{ error() }}</div>
          }

          <form [formGroup]="form" (ngSubmit)="onSubmit()" class="auth-form">
            <div class="field">
              <label for="email">Email address</label>
              <input id="email" type="email" formControlName="email"
                placeholder="you@company.com" autocomplete="email" />
            </div>

            <div class="field">
              <label for="password">Password</label>
              <input id="password" type="password" formControlName="password"
                placeholder="••••••••" autocomplete="current-password" />
            </div>

            <button type="submit" class="btn-primary submit-btn" [disabled]="loading() || form.invalid">
              @if (loading()) {
                <span class="btn-spinner"></span> Signing in...
              } @else {
                Sign in →
              }
            </button>
          </form>

          <p class="auth-footer-text">
            New organization? <a routerLink="/auth/register">Register here</a>
          </p>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .auth-shell {
      min-height: 100vh;
      display: flex;
    }

    .auth-left {
      flex: 1;
      background: linear-gradient(145deg, #0f172a 0%, #1e1b4b 50%, #312e81 100%);
      display: flex;
      flex-direction: column;
      justify-content: space-between;
      padding: 40px 48px;
      position: relative;
      overflow: hidden;

      &::before {
        content: '';
        position: absolute;
        width: 400px; height: 400px;
        border-radius: 50%;
        background: radial-gradient(circle, rgba(99,102,241,0.25) 0%, transparent 70%);
        top: -100px; right: -100px;
      }

      &::after {
        content: '';
        position: absolute;
        width: 300px; height: 300px;
        border-radius: 50%;
        background: radial-gradient(circle, rgba(139,92,246,0.2) 0%, transparent 70%);
        bottom: 50px; left: -50px;
      }
    }

    .auth-brand {
      display: flex;
      align-items: center;
      gap: 10px;
      z-index: 1;
    }

    .auth-logo {
      width: 42px; height: 42px;
      background: linear-gradient(135deg, #6366f1, #8b5cf6);
      border-radius: 12px;
      display: flex; align-items: center; justify-content: center;
      font-size: 22px; color: #fff;
    }

    .auth-logo-name {
      font-size: 18px;
      font-weight: 800;
      color: #fff;
      letter-spacing: -0.3px;
    }

    .auth-hero {
      z-index: 1;

      h2 {
        font-size: 32px;
        font-weight: 800;
        color: #fff;
        line-height: 1.2;
        margin-bottom: 14px;
        letter-spacing: -0.5px;
      }

      p {
        font-size: 15px;
        color: #94a3b8;
        margin-bottom: 28px;
        line-height: 1.6;
      }
    }

    .auth-features {
      display: flex;
      flex-direction: column;
      gap: 10px;
    }

    .feat {
      font-size: 13px;
      color: #a5b4fc;
      font-weight: 500;
    }

    .auth-right {
      width: 480px;
      display: flex;
      align-items: center;
      justify-content: center;
      background: var(--page-bg);
      padding: 40px;
    }

    .auth-card {
      width: 100%;
      max-width: 380px;
    }

    .auth-card-header {
      margin-bottom: 28px;

      h1 {
        font-size: 26px;
        font-weight: 800;
        color: var(--gray-900);
        letter-spacing: -0.5px;
        margin-bottom: 6px;
      }

      p {
        font-size: 14px;
        color: var(--gray-500);
      }
    }

    .auth-form {
      display: flex;
      flex-direction: column;
      gap: 16px;
    }

    .submit-btn {
      width: 100%;
      justify-content: center;
      padding: 12px;
      font-size: 14px;
      margin-top: 4px;
    }

    .btn-spinner {
      width: 14px; height: 14px;
      border: 2px solid rgba(255,255,255,0.4);
      border-top-color: #fff;
      border-radius: 50%;
      animation: spin 0.6s linear infinite;
      display: inline-block;
    }

    .auth-footer-text {
      text-align: center;
      margin-top: 20px;
      font-size: 13px;
      color: var(--gray-500);

      a {
        color: var(--primary);
        font-weight: 600;
        &:hover { text-decoration: underline; }
      }
    }

    .alert { margin-bottom: 16px; }

    @media (max-width: 768px) {
      .auth-left { display: none; }
      .auth-right { width: 100%; }
    }
  `]
})
export class LoginComponent {
  form: FormGroup;
  loading = signal(false);
  error = signal<string | null>(null);

  constructor(private fb: FormBuilder, private authService: AuthService, private router: Router) {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    this.loading.set(true);
    this.error.set(null);

    this.authService.login(this.form.value).subscribe({
      next: (response) => {
        const isSystemAdmin = response.user.roles.includes('SystemAdmin');
        this.router.navigate([isSystemAdmin ? '/admin' : '/dashboard']);
      },
      error: (err) => {
        this.error.set(err.error?.error ?? 'Invalid credentials. Please try again.');
        this.loading.set(false);
      }
    });
  }
}
