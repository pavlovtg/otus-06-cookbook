import { describe, it, expect } from "vitest";
import {
  LoginRequestSchema,
  RegisterRequestSchema,
  UserDtoSchema,
  AuthResponseSchema,
} from "@/lib/schemas/auth";

// ── LoginRequestSchema ────────────────────────────────────────────────────────

const validLogin = {
  email: "user@example.com",
  password: "password123",
};

describe("LoginRequestSchema", () => {
  it("парсит корректный объект", () => {
    expect(LoginRequestSchema.safeParse(validLogin).success).toBe(true);
  });

  it("отклоняет некорректный email", () => {
    expect(LoginRequestSchema.safeParse({ ...validLogin, email: "not-email" }).success).toBe(false);
  });

  it("отклоняет пароль короче 8 символов", () => {
    expect(LoginRequestSchema.safeParse({ ...validLogin, password: "short" }).success).toBe(false);
  });

  it("отклоняет пароль длиннее 200 символов", () => {
    expect(LoginRequestSchema.safeParse({ ...validLogin, password: "a".repeat(201) }).success).toBe(false);
  });

  it("отклоняет отсутствующий email", () => {
    const { email: _, ...rest } = validLogin;
    expect(LoginRequestSchema.safeParse(rest).success).toBe(false);
  });

  it("отклоняет отсутствующий password", () => {
    const { password: _, ...rest } = validLogin;
    expect(LoginRequestSchema.safeParse(rest).success).toBe(false);
  });
});

// ── RegisterRequestSchema ─────────────────────────────────────────────────────

const validRegister = {
  displayName: "Иван Иванов",
  email: "ivan@example.com",
  password: "securepass",
};

describe("RegisterRequestSchema", () => {
  it("парсит корректный объект", () => {
    expect(RegisterRequestSchema.safeParse(validRegister).success).toBe(true);
  });

  it("отклоняет displayName короче 3 символов", () => {
    expect(RegisterRequestSchema.safeParse({ ...validRegister, displayName: "ab" }).success).toBe(false);
  });

  it("отклоняет displayName длиннее 200 символов", () => {
    expect(RegisterRequestSchema.safeParse({ ...validRegister, displayName: "a".repeat(201) }).success).toBe(false);
  });

  it("отклоняет некорректный email", () => {
    expect(RegisterRequestSchema.safeParse({ ...validRegister, email: "bad" }).success).toBe(false);
  });

  it("отклоняет пароль короче 8 символов", () => {
    expect(RegisterRequestSchema.safeParse({ ...validRegister, password: "1234567" }).success).toBe(false);
  });

  it("отклоняет отсутствующий displayName", () => {
    const { displayName: _, ...rest } = validRegister;
    expect(RegisterRequestSchema.safeParse(rest).success).toBe(false);
  });
});

// ── UserDtoSchema ─────────────────────────────────────────────────────────────

const validUser = {
  id: "11111111-0000-0000-0000-000000000001",
  email: "user@example.com",
  displayName: "Иван",
  role: "user" as const,
};

describe("UserDtoSchema", () => {
  it("парсит корректный объект с ролью user", () => {
    expect(UserDtoSchema.safeParse(validUser).success).toBe(true);
  });

  it("парсит корректный объект с ролью admin", () => {
    expect(UserDtoSchema.safeParse({ ...validUser, role: "admin" }).success).toBe(true);
  });

  it("отклоняет некорректный uuid", () => {
    expect(UserDtoSchema.safeParse({ ...validUser, id: "not-uuid" }).success).toBe(false);
  });

  it("отклоняет некорректный email", () => {
    expect(UserDtoSchema.safeParse({ ...validUser, email: "bad" }).success).toBe(false);
  });

  it("отклоняет неизвестную роль", () => {
    expect(UserDtoSchema.safeParse({ ...validUser, role: "moderator" }).success).toBe(false);
  });

  it("отклоняет отсутствующий displayName", () => {
    const { displayName: _, ...rest } = validUser;
    expect(UserDtoSchema.safeParse(rest).success).toBe(false);
  });
});

// ── AuthResponseSchema ────────────────────────────────────────────────────────

const validAuthResponse = {
  token: "eyJhbGciOiJIUzI1NiJ9.payload.signature",
  user: validUser,
};

describe("AuthResponseSchema", () => {
  it("парсит корректный объект", () => {
    expect(AuthResponseSchema.safeParse(validAuthResponse).success).toBe(true);
  });

  it("отклоняет отсутствующий token", () => {
    const { token: _, ...rest } = validAuthResponse;
    expect(AuthResponseSchema.safeParse(rest).success).toBe(false);
  });

  it("отклоняет отсутствующий user", () => {
    const { user: _, ...rest } = validAuthResponse;
    expect(AuthResponseSchema.safeParse(rest).success).toBe(false);
  });

  it("отклоняет некорректный user (плохой uuid)", () => {
    expect(AuthResponseSchema.safeParse({
      ...validAuthResponse,
      user: { ...validUser, id: "bad" },
    }).success).toBe(false);
  });
});
