import { Component, signal } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/auth/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, RouterLink],
  template: `
    <div class="register-shell">
      <div class="register-header">
        <div class="auth-brand">
          <div class="auth-logo">⬡</div>
          <span class="auth-logo-name">OrgPlatform</span>
        </div>
        <a routerLink="/auth/login" class="back-link">← Back to sign in</a>
      </div>

      <div class="register-body">
        <div class="register-card">
          <div class="register-card-header">
            <h1>Create your organization</h1>
            <p>Get started in under 2 minutes</p>
          </div>

          @if (error()) {
            <div class="alert alert-error">⚠ {{ error() }}</div>
          }

          <form [formGroup]="form" (ngSubmit)="onSubmit()" class="register-form">
            <div class="form-section">
              <div class="form-section-title">
                <span class="section-badge">1</span>
                Organization Details
              </div>
              <div class="form-row">
                <div class="field">
                  <label>Organization Name *</label>
                  <input type="text" formControlName="name" placeholder="Acme Corporation" />
                </div>
                <div class="field">
                  <label>Contact Email *</label>
                  <input type="email" formControlName="email" placeholder="contact@acme.com" />
                </div>
              </div>
              <div class="form-row">
                <div class="field">
                  <label>Phone</label>
                  <input type="text" formControlName="phone" placeholder="+1 555 000 0000" />
                </div>
                <div class="field">
                  <label>Timezone</label>
                  <input type="text" formControlName="timeZone" placeholder="UTC" />
                </div>
              </div>
            </div>

            <div class="form-section">
              <div class="form-section-title">
                <span class="section-badge">2</span>
                Administrator Account
              </div>
              <div class="form-row">
                <div class="field">
                  <label>First Name *</label>
                  <input type="text" formControlName="adminFirstName" placeholder="John" />
                </div>
                <div class="field">
                  <label>Last Name *</label>
                  <input type="text" formControlName="adminLastName" placeholder="Doe" />
                </div>
              </div>
              <div class="field">
                <label>Admin Email *</label>
                <input type="email" formControlName="adminEmail" placeholder="admin@acme.com" autocomplete="email" />
              </div>
              <div class="field">
                <label>Password *</label>
                <input type="password" formControlName="adminPassword" placeholder="Minimum 8 characters" autocomplete="new-password" />
              </div>
            </div>

            <button type="submit" class="btn-primary submit-btn" [disabled]="loading() || form.invalid">
              @if (loading()) {
                <span class="btn-spinner"></span> Creating organization...
              } @else {
                Create organization →
              }
            </button>
          </form>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .register-shell {
      min-height: 100vh;
      background: var(--page-bg);
      display: flex;
      flex-direction: column;
    }

    .register-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 20px 40px;
      background: #fff;
      border-bottom: 1px solid var(--gray-200);
    }

    .auth-brand { display: flex; align-items: center; gap: 10px; }
    .auth-logo {
      width: 38px; height: 38px;
      background: linear-gradient(135deg, #6366f1, #8b5cf6);
      border-radius: 10px;
      display: flex; align-items: center; justify-content: center;
      font-size: 20px; color: #fff;
    }
    .auth-logo-name { font-size: 16px; font-weight: 800; color: var(--gray-900); }

    .back-link {
      font-size: 13px;
      color: var(--primary);
      font-weight: 600;
      &:hover { text-decoration: underline; }
    }

    .register-body {
      flex: 1;
      display: flex;
      align-items: flex-start;
      justify-content: center;
      padding: 40px 20px;
    }

    .register-card {
      width: 100%;
      max-width: 640px;
    }

    .register-card-header {
      margin-bottom: 28px;
      h1 { font-size: 26px; font-weight: 800; color: var(--gray-900); letter-spacing: -0.5px; margin-bottom: 6px; }
      p  { font-size: 14px; color: var(--gray-500); }
    }

    .register-form { display: flex; flex-direction: column; gap: 8px; }

    .form-section {
      background: #fff;
      border: 1px solid var(--gray-200);
      border-radius: var(--border-radius);
      padding: 20px 22px;
      margin-bottom: 12px;
      display: flex;
      flex-direction: column;
      gap: 14px;
    }

    .form-section-title {
      display: flex;
      align-items: center;
      gap: 10px;
      font-size: 13px;
      font-weight: 700;
      color: var(--gray-700);
    }

    .section-badge {
      width: 22px; height: 22px;
      background: var(--primary);
      color: #fff;
      border-radius: 50%;
      display: flex; align-items: center; justify-content: center;
      font-size: 11px; font-weight: 700;
    }

    .form-row {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 14px;
    }

    .submit-btn {
      width: 100%;
      justify-content: center;
      padding: 12px;
      font-size: 14px;
    }

    .btn-spinner {
      width: 14px; height: 14px;
      border: 2px solid rgba(255,255,255,0.4);
      border-top-color: #fff;
      border-radius: 50%;
      animation: spin 0.6s linear infinite;
      display: inline-block;
    }

    .alert { margin-bottom: 16px; }

    @media (max-width: 640px) {
      .form-row { grid-template-columns: 1fr; }
      .register-header { padding: 16px 20px; }
    }
  `]
})
export class RegisterComponent {
  form: FormGroup;
  loading = signal(false);
  error = signal<string | null>(null);

  constructor(private fb: FormBuilder, private authService: AuthService, private router: Router) {
    this.form = this.fb.group({
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phone: [''],
      timeZone: ['UTC'],
      adminFirstName: ['', Validators.required],
      adminLastName: ['', Validators.required],
      adminEmail: ['', [Validators.required, Validators.email]],
      adminPassword: ['', [Validators.required, Validators.minLength(8)]]
    });
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    this.loading.set(true);
    this.error.set(null);

    this.authService.register(this.form.value).subscribe({
      next: () => this.router.navigate(['/dashboard']),
      error: (err) => {
        this.error.set(err.error?.error ?? 'Registration failed. Please try again.');
        this.loading.set(false);
      }
    });
  }
}
