import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { loginGuard } from './core/guards/login.guard';
import { roleGuard } from './core/guards/role.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login').then(m => m.Login),
    canActivate: [loginGuard]
  },
  {
    path: '',
    canActivate: [authGuard],
    children: [
      {
        path: '',
        redirectTo: '/dashboard',
        pathMatch: 'full'
      },
      {
        path: 'dashboard',
        loadComponent: () => import('./features/dashboard/dashboard').then(m=>m.Dashboard)
      },
      {
        path: 'user-roles',
        loadComponent: () => import('./features/user-role/components/user-role-list/user-role-list').then(m => m.UserRoleList),
        canActivate: [() => roleGuard(['Admin'])]
      },
      {
        path: 'user-roles/create',
        loadComponent: () => import('./features/user-role/components/user-role-form/user-role-form').then(m => m.UserRoleForm),
        canActivate: [() => roleGuard(['Admin'])]
      },
      {
        path: 'user-roles/edit/:id',
        loadComponent: () => import('./features/user-role/components/user-role-form/user-role-form').then(m => m.UserRoleForm),
        canActivate: [() => roleGuard(['Admin'])]
      }
    ]
  },
  {
    path: '**',
    redirectTo: ''
  }
];