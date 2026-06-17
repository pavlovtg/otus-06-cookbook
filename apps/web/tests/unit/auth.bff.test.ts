import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { login, register, logout, getMe } from "@/lib/bff/auth";

const mockUser = {
  id: "11111111-0000-0000-0000-000000000001",
  email: "user@example.com",
  displayName: "Иван",
  role: "user" as const,
};

const mockAuthResponse = {
  token: "test-jwt-token",
  user: mockUser,
};

beforeEach(() => {
  vi.stubGlobal("fetch", vi.fn());
});

afterEach(() => {
  vi.unstubAllGlobals();
});

// ── login ─────────────────────────────────────────────────────────────────────

describe("login", () => {
  it("отправляет POST и возвращает AuthResponse", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(
      new Response(JSON.stringify(mockAuthResponse), { status: 200 })
    );

    const result = await login({ email: "user@example.com", password: "password123" });

    expect(vi.mocked(fetch)).toHaveBeenCalledWith(
      expect.stringContaining("/auth/login"),
      expect.objectContaining({ method: "POST" })
    );
    expect(result.token).toBe("test-jwt-token");
    expect(result.user.email).toBe("user@example.com");
  });

  it("выбрасывает ошибку при 401", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 401 }));

    await expect(login({ email: "bad@example.com", password: "wrongpass" })).rejects.toThrow("401");
  });

  it("выбрасывает ошибку при 500", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 500 }));

    await expect(login({ email: "user@example.com", password: "password123" })).rejects.toThrow("500");
  });
});

// ── register ──────────────────────────────────────────────────────────────────

describe("register", () => {
  it("отправляет POST и возвращает AuthResponse", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(
      new Response(JSON.stringify(mockAuthResponse), { status: 200 })
    );

    const result = await register({
      displayName: "Иван",
      email: "user@example.com",
      password: "password123",
    });

    expect(vi.mocked(fetch)).toHaveBeenCalledWith(
      expect.stringContaining("/auth/register"),
      expect.objectContaining({ method: "POST" })
    );
    expect(result.user.displayName).toBe("Иван");
  });

  it("выбрасывает ошибку при 409 (email занят)", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 409 }));

    await expect(
      register({ displayName: "Иван", email: "taken@example.com", password: "password123" })
    ).rejects.toThrow("409");
  });

  it("выбрасывает ошибку при 400", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 400 }));

    await expect(
      register({ displayName: "Иван", email: "user@example.com", password: "password123" })
    ).rejects.toThrow("400");
  });
});

// ── logout ────────────────────────────────────────────────────────────────────

describe("logout", () => {
  it("отправляет POST на /auth/logout", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 200 }));

    await logout();

    expect(vi.mocked(fetch)).toHaveBeenCalledWith(
      expect.stringContaining("/auth/logout"),
      expect.objectContaining({ method: "POST" })
    );
  });

  it("выбрасывает ошибку при неуспешном ответе", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 500 }));

    await expect(logout()).rejects.toThrow("500");
  });
});

// ── getMe ─────────────────────────────────────────────────────────────────────

describe("getMe", () => {
  it("возвращает UserDto", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(
      new Response(JSON.stringify(mockUser), { status: 200 })
    );

    const result = await getMe();

    expect(vi.mocked(fetch)).toHaveBeenCalledWith(
      expect.stringContaining("/auth/me"),
      expect.objectContaining({ cache: "no-store" })
    );
    expect(result.id).toBe(mockUser.id);
    expect(result.role).toBe("user");
  });

  it("выбрасывает ошибку при 401", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 401 }));

    await expect(getMe()).rejects.toThrow("401");
  });
});
