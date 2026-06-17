import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";

vi.mock("@/lib/session", () => ({
  getSession: vi.fn(),
  getSessionOptions: vi.fn().mockReturnValue({
    password: "test-secret-32-chars-long-enough!",
    cookieName: "cookbook_session",
  }),
}));

vi.mock("iron-session", () => ({
  getIronSession: vi.fn(),
}));

vi.mock("next/server", () => ({
  NextRequest: class {},
  NextResponse: {
    next: vi.fn().mockReturnValue({}),
  },
}));

import { serverFetch, proxyFetch } from "@/lib/server-fetch";
import { getSession } from "@/lib/session";
import { getIronSession } from "iron-session";
import type { NextRequest } from "next/server";

beforeEach(() => {
  vi.stubGlobal("fetch", vi.fn().mockResolvedValue(new Response(null, { status: 200 })));
});

afterEach(() => {
  vi.unstubAllGlobals();
  vi.clearAllMocks();
});

describe("serverFetch", () => {
  it("вызывает fetch без Authorization если токена нет", async () => {
    vi.mocked(getSession).mockResolvedValueOnce({ token: undefined } as never);

    await serverFetch("https://api.example.com/test");

    expect(vi.mocked(fetch)).toHaveBeenCalledOnce();
    const [url, init] = vi.mocked(fetch).mock.calls[0] as [string, RequestInit & { headers: Headers }];
    expect(url).toBe("https://api.example.com/test");
    expect((init.headers as Headers).get("Authorization")).toBeNull();
  });

  it("добавляет Authorization Bearer если токен есть", async () => {
    vi.mocked(getSession).mockResolvedValueOnce({ token: "my-jwt-token" } as never);

    await serverFetch("https://api.example.com/test");

    const [, init] = vi.mocked(fetch).mock.calls[0] as [string, RequestInit & { headers: Headers }];
    expect((init.headers as Headers).get("Authorization")).toBe("Bearer my-jwt-token");
  });

  it("игнорирует ошибку getSession и вызывает fetch без Authorization", async () => {
    vi.mocked(getSession).mockRejectedValueOnce(new Error("outside server context"));

    await serverFetch("https://api.example.com/test");

    expect(vi.mocked(fetch)).toHaveBeenCalledOnce();
    const [, init] = vi.mocked(fetch).mock.calls[0] as [string, RequestInit & { headers: Headers }];
    expect((init.headers as Headers).get("Authorization")).toBeNull();
  });
});

describe("proxyFetch", () => {
  function makeRequest(authHeader?: string) {
    return {
      headers: {
        get: (name: string) => (name === "authorization" ? (authHeader ?? null) : null),
      },
    } as unknown as NextRequest;
  }

  it("пробрасывает входящий Authorization если он есть", async () => {
    const req = makeRequest("Bearer incoming-token");

    await proxyFetch(req, "https://api.example.com/proxy");

    const [, init] = vi.mocked(fetch).mock.calls[0] as [string, RequestInit & { headers: Headers }];
    expect((init.headers as Headers).get("Authorization")).toBe("Bearer incoming-token");
    expect(vi.mocked(getIronSession)).not.toHaveBeenCalled();
  });

  it("берёт токен из iron-session если входящего Authorization нет", async () => {
    const req = makeRequest();
    vi.mocked(getIronSession).mockResolvedValueOnce({ token: "session-token" } as never);

    await proxyFetch(req, "https://api.example.com/proxy");

    const [, init] = vi.mocked(fetch).mock.calls[0] as [string, RequestInit & { headers: Headers }];
    expect((init.headers as Headers).get("Authorization")).toBe("Bearer session-token");
  });

  it("не добавляет Authorization если нет ни входящего токена, ни сессии", async () => {
    const req = makeRequest();
    vi.mocked(getIronSession).mockResolvedValueOnce({ token: undefined } as never);

    await proxyFetch(req, "https://api.example.com/proxy");

    const [, init] = vi.mocked(fetch).mock.calls[0] as [string, RequestInit & { headers: Headers }];
    expect((init.headers as Headers).get("Authorization")).toBeNull();
  });
});
