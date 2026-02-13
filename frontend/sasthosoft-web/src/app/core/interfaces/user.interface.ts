export interface User {
  id: number;
  username: string;
  email?: string;
  roleId: number;
  roleName?: string;
  createdAt: Date;
}

export interface CreateUser {
  username: string;
  email?: string;
  password: string;
  roleId: number;
}

export interface UpdateUser {
  username: string;
  email?: string;
  roleId: number;
}