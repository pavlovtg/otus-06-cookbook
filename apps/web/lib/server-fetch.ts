import { NextRequest, NextResponse } from "next/server";
import { getIronSession } from "iron-session";
import { getSession, getSessionOptions, type SessionData } from "@/lib/session";

/**
 * Обёртка над fetch для Server Components.
 * Автоматически добавляет Authorization из iron-session (cookie).
 */
export async function serverFetch(
  url: string,
  init: RequestInit = {},
): Promise<Response> {
  const headers = new Headers(init.headers as HeadersInit | undefined);
  try {
    const session = await getSession();
    if (session.token) {
      headers.set("Authorization", `Bearer ${session.token}`);
    }
  } catch {
    // вне Server Component контекста — игнорируем
  }
  return fetch(url, { ...init, headers });
}

/**
 * Обёртка над fetch для серверных Route Handlers.
 *
 * Источник `Authorization`:
 * 1) если входящий запрос уже содержит `Authorization` — пробрасываем как есть
 *    (нужно для прямых API-вызовов с токеном, например из e2e/UI-тестов);
 * 2) иначе берём JWT из iron-session (стандартный UI-флоу через cookie).
 */
export async function proxyFetch(
  req: NextRequest,
  url: string,
  init: RequestInit = {},
): Promise<Response> {
  const existingHeaders = new Headers(init.headers as HeadersInit | undefined);

  const incomingAuth = req.headers.get("authorization");
  if (incomingAuth) {
    existingHeaders.set("Authorization", incomingAuth);
  } else {
    const res = NextResponse.next();
    const session = await getIronSession<SessionData>(req, res, getSessionOptions());
    if (session.token) {
      existingHeaders.set("Authorization", `Bearer ${session.token}`);
    }
  }

  return fetch(url, { ...init, headers: existingHeaders });
}
