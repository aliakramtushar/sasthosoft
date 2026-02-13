import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { UserRole, CreateUserRole, UpdateUserRole } from '../interfaces/user-role.interface';

@Injectable({
  providedIn: 'root'
})
export class UserRoleService {
  private apiUrl = `${environment.apiUrl}/userroles`;

  constructor(private http: HttpClient) { }

  getAllRoles(): Observable<UserRole[]> {
    return this.http.get<UserRole[]>(this.apiUrl);
  }

  getRoleById(id: number): Observable<UserRole> {
    return this.http.get<UserRole>(`${this.apiUrl}/${id}`);
  }

  createRole(role: CreateUserRole): Observable<UserRole> {
    return this.http.post<UserRole>(this.apiUrl, role);
  }

  updateRole(id: number, role: UpdateUserRole): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, role);
  }

  deleteRole(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}