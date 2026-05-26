export interface LoginRequest {
  username: string;
  password: string;
}

export interface RegisterRequest {
  nume: string;
  prenume: string;
  username: string;
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  expiresIn?: number;
  userId?: string;
}
