import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../core/services/api.service';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './reports.component.html',
  styleUrl: './reports.component.scss'
})
export class ReportsComponent implements OnInit {
  tab = 'projects';
  loading = signal(false);
  reportData = signal<any[]>([]);
  taskReport = signal<any>(null);

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.loadReport();
  }

  switchTab(t: string): void {
    this.tab = t;
    this.loadReport();
  }

  loadReport(): void {
    this.loading.set(true);

    if (this.tab === 'projects') {
      this.api.get<any[]>('/reports/projects').subscribe({
        next: (d) => { this.reportData.set(d); this.loading.set(false); },
        error: () => this.loading.set(false)
      });
    } else if (this.tab === 'tasks') {
      this.api.get<any>('/reports/tasks').subscribe({
        next: (d) => { this.taskReport.set(d); this.loading.set(false); },
        error: () => this.loading.set(false)
      });
    } else {
      this.loading.set(false);
    }
  }
}
