import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

export interface MenuItem {
  label: string;
  icon?: string;
  route?: string;
  children?: MenuItem[];
  expanded?: boolean;
}

@Component({
  standalone: true,
  selector: 'app-sidebar',
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    MatExpansionModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule
  ],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.css',
})

export class Sidebar implements OnInit {
  menuItems: MenuItem[] = [
    {
      label: 'Dashboard',
      icon: 'dashboard',
      route: '/dashboard'
    },
    {
      label: 'User Management',
      icon: 'people',
      children: [
        {
          label: 'All Users',
          icon: 'person',
          route: '/users/all'
        },
        {
          label: 'Roles & Permissions',
          icon: 'admin_panel_settings',
          children: [
            {
              label: 'Roles',
              route: '/users/roles'
            },
            {
              label: 'Permissions',
              route: '/users/permissions'
            }
          ]
        },
        {
          label: 'Teams',
          icon: 'groups',
          route: '/users/teams'
        }
      ]
    },
    {
      label: 'Analytics',
      icon: 'analytics',
      children: [
        {
          label: 'Reports',
          icon: 'assessment',
          route: '/analytics/reports'
        },
        {
          label: 'Insights',
          icon: 'insights',
          children: [
            {
              label: 'User Insights',
              route: '/analytics/insights/users'
            },
            {
              label: 'Revenue Insights',
              route: '/analytics/insights/revenue'
            }
          ]
        }
      ]
    },
    {
      label: 'Projects',
      icon: 'folder',
      children: [
        {
          label: 'Active Projects',
          icon: 'play_circle',
          route: '/projects/active'
        },
        {
          label: 'Archived',
          icon: 'archive',
          route: '/projects/archived'
        }
      ]
    },
    {
      label: 'Settings',
      icon: 'settings',
      children: [
        {
          label: 'General',
          icon: 'tune',
          route: '/settings/general'
        },
        {
          label: 'Security',
          icon: 'security',
          route: '/settings/security'
        },
        {
          label: 'Billing',
          icon: 'credit_card',
          route: '/settings/billing'
        }
      ]
    }
  ];

  filteredMenuItems: MenuItem[] = [];
  searchQuery: string = '';
  selectedRoute: string = '';

  constructor(private router: Router) {
    this.filteredMenuItems = this.menuItems;
  }

  ngOnInit() {
    this.selectedRoute = this.router.url;
  }

  filterMenu(): void {
    if (!this.searchQuery.trim()) {
      this.filteredMenuItems = this.menuItems;
      return;
    }

    const query = this.searchQuery.toLowerCase();
    this.filteredMenuItems = this.menuItems.filter(item => 
      this.menuItemMatchesSearch(item, query)
    );
  }

  private menuItemMatchesSearch(item: MenuItem, query: string): boolean {
    const labelMatch = item.label.toLowerCase().includes(query);
    
    if (item.children) {
      const childMatch = item.children.some(child => 
        this.menuItemMatchesSearch(child, query)
      );
      return labelMatch || childMatch;
    }
    
    return labelMatch;
  }

  isMenuItemSelected(route?: string): boolean {
    if (!route) return false;
    return this.selectedRoute === route;
  }

  onMenuItemClick(menuItem: MenuItem): void {
    if (menuItem.route) {
      this.selectedRoute = menuItem.route;
    }
  }

  clearSearch(): void {
    this.searchQuery = '';
    this.filteredMenuItems = this.menuItems;
  }
}