export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  name: string;
  email: string;
  password: string;
  gender: string;
  birthDate: string;
}

export interface AuthResponse {
  name: string;
  email: string;
  token: string;
}
