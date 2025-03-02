import api from './api';
import { AxiosResponse } from 'axios';

// Types
export interface User {
  id: string;
  userName: string;
  email: string;
  firstName: string;
  lastName: string;
  companyName?: string;
  companyAddress?: string;
  companyRegistrationNumber?: string;
  companyContact?: string;
}

export interface Role {
  id: string;
  name: string;
}

export interface CreateRoleRequest {
  roleName: string;
}

export interface AdminUpdateUserRequest {
  companyName?: string;
  companyAddress?: string;
  companyRegistrationNumber?: string;
  companyContact?: string;
}

class AdminService {
  private baseUrl = 'v1/admin';

  // User Management
  public async getAllUsers(): Promise<User[]> {
    const response: AxiosResponse<User[]> = await api.get(`${this.baseUrl}/users`);
    return response.data;
  }

  public async getUserById(userId: string): Promise<User> {
    const response: AxiosResponse<User> = await api.get(`${this.baseUrl}/users/${userId}`);
    return response.data;
  }

  public async updateUser(userId: string, updateData: AdminUpdateUserRequest): Promise<void> {
    await api.put(`${this.baseUrl}/users/${userId}`, updateData);
  }

  public async deleteUser(userId: string): Promise<void> {
    await api.delete(`${this.baseUrl}/users/${userId}`);
  }

  // Role Management
  public async getAllRoles(): Promise<Role[]> {
    const response: AxiosResponse<Role[]> = await api.get(`${this.baseUrl}/roles`);
    return response.data;
  }

  public async createRole(roleName: string): Promise<Role> {
    const response: AxiosResponse<Role> = await api.post(`${this.baseUrl}/roles/create`, { roleName });
    return response.data;
  }

  public async deleteRole(roleId: string): Promise<void> {
    await api.delete(`${this.baseUrl}/roles/${roleId}`);
  }
}

export default new AdminService(); 