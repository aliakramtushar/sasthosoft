export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  username: string;
  email?: string;
  role: string;
  roleId: number;
  userId: number;
}

export interface RefreshTokenRequest {
  refreshToken: string;
}