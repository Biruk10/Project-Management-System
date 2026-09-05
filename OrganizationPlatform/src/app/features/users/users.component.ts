import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../core/services/user.service';
import { User } from '../../core/models/user.models';
import { PagedResult } from '../../core/models/api.models';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './users.component.html',
  styleUrl: './users.component.scss'
})
export class UsersComponent implements OnInit {
  result = signal<PagedResult<User> | null>(null);
  page = 1;
  search = '';
  activeFilter = '';

  constructor(private userService: UserService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.userService.getAll({
      page: this.page,
      search: this.search || undefined,
      isActive: this.activeFilter !== '' ? this.activeFilter === 'true' : undefined
    }).subscribe({ next: (d) => this.result.set(d) });
  }

  onSearch(): void {
    this.page = 1;
    this.load();
  }

  changePage(p: number): void {
    this.page = p;
    this.load();
  }

  toggle(user: User): void {
    const action = user.isActive
      ? this.userService.deactivate(user.id)
      : this.userService.activate(user.id);

    action.subscribe({ next: () => this.load() });
  }
}
