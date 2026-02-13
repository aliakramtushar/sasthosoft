import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, BehaviorSubject, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { LoginRequest, LoginResponse } from '../interfaces/auth.interface';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}/auth`;
  private jwtHelper = new JwtHelperService();
  private currentUserSubject = new BehaviorSubject<LoginResponse | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    this.loadStoredUser();
  }

  private loadStoredUser(): void {
    const token = localStorage.getItem('accessToken'); // ✅ Matches your API response
    const refreshToken = localStorage.getItem('refreshToken'); // ✅ Matches your API response
    const userData = localStorage.getItem('user_data');

    if (token && refreshToken && userData && !this.jwtHelper.isTokenExpired(token)) {
      try {
        const user = JSON.parse(userData);
        this.currentUserSubject.next(user);
      } catch (e) {
        console.error('Error parsing user data', e);
        this.clearStorage();
      }
    } else {
      this.clearStorage();
    }
  }

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, credentials).pipe(
      tap(response => this.handleAuthentication(response)),
      catchError((error: HttpErrorResponse) => {
        return throwError(() => error);
      })
    );
  }

  refreshToken(refreshToken: string): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/refresh`, { refreshToken }).pipe(
      tap(response => this.handleAuthentication(response)),
      catchError((error: HttpErrorResponse) => {
        this.logout();
        return throwError(() => error);
      })
    );
  }

  logout(): void {
    this.http.post(`${this.apiUrl}/logout`, {}).subscribe({
      next: () => {
        this.clearStorageAndRedirect();
      },
      error: () => {
        this.clearStorageAndRedirect();
      }
    });
  }

  private handleAuthentication(response: LoginResponse): void {
    // ✅ FIX: Use accessToken instead of token
    localStorage.setItem('accessToken', response.accessToken);
    localStorage.setItem('refreshToken', response.refreshToken);
    
    // Store the full response including user object
    localStorage.setItem('user_data', JSON.stringify(response));
    this.currentUserSubject.next(response);
  }

  private clearStorageAndRedirect(): void {
    this.clearStorage();
    this.router.navigate(['/login']);
  }

  private clearStorage(): void {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user_data');
    this.currentUserSubject.next(null);
  }

  getToken(): string | null {
    return localStorage.getItem('accessToken'); // ✅ Returns accessToken
  }

  getRefreshToken(): string | null {
    return localStorage.getItem('refreshToken'); // ✅ Returns refreshToken
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    return token ? !this.jwtHelper.isTokenExpired(token) : false;
  }

  getUserRole(): string | null {
    const userData = localStorage.getItem('user_data');
    if (userData) {
      try {
        const user = JSON.parse(userData);
        return user.user?.roleName || user.roleName; // ✅ Handle nested or flat structure
      } catch {
        return null;
      }
    }
    return null;
  }

  getUserId(): number | null {
    const userData = localStorage.getItem('user_data');
    if (userData) {
      try {
        const user = JSON.parse(userData);
        return user.user?.id || user.id; // ✅ Handle nested or flat structure
      } catch {
        return null;
      }
    }
    return null;
  }

  getCurrentUser(): LoginResponse | null {
    return this.currentUserSubject.value;
  }
}