import { z } from "zod";

export const LoginRequestSchema = z.object({
  email: z.string().email(),
  password: z.string().min(8).max(200),
});

export type LoginRequest = z.infer<typeof LoginRequestSchema>;

export const RegisterRequestSchema = z.object({
  displayName: z.string().min(3).max(200),
  email: z.string().email(),
  password: z.string().min(8).max(200),
});

export type RegisterRequest = z.infer<typeof RegisterRequestSchema>;

export const UserDtoSchema = z.object({
  id: z.string().uuid(),
  email: z.string().email(),
  displayName: z.string(),
  role: z.enum(["user", "admin"]),
});

export type UserDto = z.infer<typeof UserDtoSchema>;

export const AuthResponseSchema = z.object({
  token: z.string(),
  user: UserDtoSchema,
});

export type AuthResponse = z.infer<typeof AuthResponseSchema>;
