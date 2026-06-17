import {
  AuthResponseSchema,
  UserDtoSchema,
  type AuthResponse,
  type LoginRequest,
  type RegisterRequest,
  type UserDto,
} from "@/lib/schemas/auth";

const CLIENT_BASE = `/api/cookbook/v1/auth`;

export async function login(data: LoginRequest): Promise<AuthResponse> {
  const response = await fetch(`${CLIENT_BASE}/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error(`Login failed: ${response.status}`);
  }

  const result: unknown = await response.json();
  return AuthResponseSchema.parse(result);
}

export async function register(data: RegisterRequest): Promise<AuthResponse> {
  const response = await fetch(`${CLIENT_BASE}/register`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error(`Register failed: ${response.status}`);
  }

  const result: unknown = await response.json();
  return AuthResponseSchema.parse(result);
}

export async function logout(): Promise<void> {
  const response = await fetch(`${CLIENT_BASE}/logout`, {
    method: "POST",
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error(`Logout failed: ${response.status}`);
  }
}

export async function getMe(): Promise<UserDto> {
  const response = await fetch(`${CLIENT_BASE}/me`, {
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error(`getMe failed: ${response.status}`);
  }

  const result: unknown = await response.json();
  return UserDtoSchema.parse(result);
}
