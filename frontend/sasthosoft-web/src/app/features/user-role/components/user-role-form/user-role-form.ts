import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { HttpErrorResponse } from '@angular/common/http';
import { Subject, takeUntil } from 'rxjs';
import { UserRoleService } from '../../services/user-role.service';
import { UserRole } from '../../../../core/interfaces/user-role.interface';

@Component({
  selector: 'app-user-role-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './user-role-form.html',
  styleUrls: ['./user-role-form.css']
})
export class UserRoleForm implements OnInit, OnDestroy {
  roleForm!: FormGroup;
  isEditMode = false;
  roleId?: number;
  isLoading = false;
  private destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private userRoleService: UserRoleService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.initializeForm();
    this.checkEditMode();
  }

  private initializeForm(): void {
    this.roleForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]]
    });
  }

  private checkEditMode(): void {
    this.route.params
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (params) => {
          if (params['id']) {
            this.isEditMode = true;
            this.roleId = +params['id'];
            this.loadRole(this.roleId);
          }
        },
        error: (error: HttpErrorResponse) => {
          console.error('Error reading route params:', error);
          this.router.navigate(['/user-roles']);
        }
      });
  }

  private loadRole(id: number): void {
    this.isLoading = true;
    this.userRoleService.getRoleById(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (role: UserRole) => {
          this.roleForm.patchValue({
            name: role.name
          });
          this.isLoading = false;
        },
        error: (error: HttpErrorResponse) => {
          this.isLoading = false;
          this.showNotification('Error loading role', 'error');
          console.error('Error loading role:', error);
          this.router.navigate(['/user-roles']);
        }
      });
  }

  // onSubmit(): void {
  //   if (this.roleForm.invalid) {
  //     return;
  //   }

  //   this.isLoading = true;
  //   const roleData = { name: this.roleForm.value.name };

  //   const request = this.isEditMode && this.roleId
  //     ? this.userRoleService.updateRole(this.roleId, roleData)
  //     : this.userRoleService.createRole(roleData);

  //   request
  //     .pipe(takeUntil(this.destroy$))
  //     .subscribe({
  //       next: () => {
  //         this.isLoading = false;
  //         this.showNotification(
  //           `Role ${this.isEditMode ? 'updated' : 'created'} successfully`,
  //           'success'
  //         );
  //         this.router.navigate(['/user-roles']);
  //       },
  //       error: (error: HttpErrorResponse) => {
  //         this.isLoading = false;
  //         const errorMessage = error.error?.message || 
  //                            `Error ${this.isEditMode ? 'updating' : 'creating'} role`;
  //         this.showNotification(errorMessage, 'error');
  //         console.error('Error saving role:', error);
  //       }
  //     });
  // }

  private showNotification(message: string, type: 'success' | 'error'): void {
    this.snackBar.open(message, 'Close', {
      duration: 3000,
      horizontalPosition: 'end',
      verticalPosition: 'top',
      panelClass: type === 'success' ? ['bg-success', 'text-white'] : ['bg-danger', 'text-white']
    });
  }

  public goBack(): void {
  this.router.navigate(['/user-roles']);
}

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}