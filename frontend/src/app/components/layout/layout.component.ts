import { Component, Inject, OnInit, Renderer2 } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { CommonModule, DOCUMENT } from '@angular/common';
import { AuthService } from '../../services/auth/auth.service';
import { User } from '../../models/auth/user';
import { Observable } from 'rxjs';
import { NzSwitchModule } from 'ng-zorro-antd/switch';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [
    CommonModule,
    NzLayoutModule,
    RouterOutlet,
    NzIconModule,
    NzMenuModule,
    NzDropDownModule,
    RouterLink,
    NzSwitchModule,
    FormsModule,
  ],
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss'],
})
export class LayoutComponent implements OnInit {
  isCollapsed = false;
  currentUser$: Observable<User | null>;
  isDarkMode = false;
  private readonly THEME_STORAGE_KEY = 'tb-theme';

  constructor(
    private authService: AuthService,
    private router: Router,
    private renderer: Renderer2,
    @Inject(DOCUMENT) private document: Document
  ) {
    this.currentUser$ = this.authService.currentUser;
  }

  ngOnInit(): void {
    if (!this.authService.currentUserValue && this.authService.hasToken()) {
      this.authService.getCurrentUser(true).subscribe();
    }

    const savedTheme = localStorage.getItem(this.THEME_STORAGE_KEY);
    this.isDarkMode = savedTheme
      ? savedTheme === 'dark'
      : window.matchMedia?.('(prefers-color-scheme: dark)').matches;
    this.applyTheme();
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/auth/login']);
  }

  toggleTheme(): void {
    this.isDarkMode = !this.isDarkMode;
    this.applyTheme();
  }

  onThemeChange(value: boolean): void {
    this.isDarkMode = value;
    this.applyTheme();
  }

  private applyTheme(): void {
    const themeClass = 'dark-theme';
    if (this.isDarkMode) {
      this.renderer.addClass(this.document.body, themeClass);
      localStorage.setItem(this.THEME_STORAGE_KEY, 'dark');
    } else {
      this.renderer.removeClass(this.document.body, themeClass);
      localStorage.setItem(this.THEME_STORAGE_KEY, 'light');
    }
  }
}
