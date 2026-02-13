import { Component, OnInit, inject, signal, Signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { HttpErrorResponse } from '@angular/common/http';
import { UserRoleService } from '../../services/user-role.service';
import { UserRole } from '../../../../core/interfaces/user-role.interface';
import { ConfirmDialog } from '../../../../shared/components/confirm-dialog/confirm-dialog';
import { RouterLink } from '@angular/router'; 
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner'; // This imports mat-progress-spinner

@Component({
  selector: 'app-user-role-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatDialogModule,
    MatSnackBarModule,
    MatTooltipModule,
    RouterLink,
    MatProgressSpinnerModule
  ],
  templateUrl: './user-role-list.html',
  styleUrls: ['./user-role-list.css']
})
export class UserRoleList implements OnInit {
  roles: UserRole[] = [];
  displayedColumns: string[] = ['id', 'name', 'actions'];
   // Fix: Properly type the signal
  private isLoadingSignal = signal<boolean>(false);
  public isLoading = this.isLoadingSignal.asReadonly(); // This is a Signal<boolean>, not a boolean


  constructor(
    private userRoleService: UserRoleService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadRoles();
  }

  loadRoles(): void {
    this.isLoadingSignal.set(true); // Use the writable signal to set value
    this.userRoleService.getAllRoles().subscribe({
      next: (data) => {
        this.roles = data;
        this.isLoadingSignal.set(false);
      },
      error: (error: HttpErrorResponse) => {
        this.isLoadingSignal.set(true); // Use the writable signal to set value
        this.showNotification('Error loading roles', 'error');
        console.error('Error loading roles:', error);
      }
    });
  }

  deleteRole(role: UserRole): void {
    const dialogRef = this.dialog.open(ConfirmDialog, {
      width: '400px',
      data: {
        title: 'Confirm Delete',
        message: `Are you sure you want to delete role "${role.name}"?`,
        confirmText: 'Delete',
        cancelText: 'Cancel'
      }
    });

    dialogRef.afterClosed().subscribe({
      next: (result) => {
        if (result) {
          this.userRoleService.deleteRole(role.id).subscribe({
            next: () => {
              this.loadRoles();
              this.showNotification(`Role "${role.name}" deleted successfully`, 'success');
            },
            error: (error: HttpErrorResponse) => {
              const errorMessage = error.error?.message || 'Error deleting role';
              this.showNotification(errorMessage, 'error');
              console.error('Error deleting role:', error);
            }
          });
        }
      }
    });
  }

  private showNotification(message: string, type: 'success' | 'error'): void {
    this.snackBar.open(message, 'Close', {
      duration: 3000,
      horizontalPosition: 'end',
      verticalPosition: 'top',
      panelClass: type === 'success' ? ['bg-success', 'text-white'] : ['bg-danger', 'text-white']
    });
  }
}