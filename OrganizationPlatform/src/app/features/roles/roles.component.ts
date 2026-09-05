import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../core/services/api.service';

interface Role {
  id: number;
  name: string;
  description?: string;
  isSystemRole: boolean;
  permissions: string[];
}

@Component({
  selector: 'app-roles',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './roles.component.html',
  styleUrl: './roles.component.scss'
})
export class RolesComponent implements OnInit {
  roles = signal<Role[]>([]);

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.api.get<Role[]>('/roles').subscribe({
      next: (data) => this.roles.set(data)
    });
  }
}
