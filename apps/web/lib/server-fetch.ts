import { NextRequest, NextResponse } from "next/server";
import { getIronSession } from "iron-session";
import { sessionOptions, type SessionData } from "@/lib/session";

/**
 * Обёртка над fetch для серверных Route Handlers.
 * Автоматически добавляет `Authorization: Bearer <token>` из iron-session,
 * если токен присутствует в сессии.
 */
export async function proxyFetch(
  req: NextRequest,
  url: string,
  init: RequestInit = {},
): Promise<Response> {
  const res = NextResponse.next();
  const session = await getIronSession<SessionData>(req, res, sessionOptions);
  const token = session.token;

  const existingHeaders = new Headers(init.headers as HeadersInit | undefined);
  if (token) {
    existingHeaders.set("Authorization", `Bearer ${token}`);
  }

  return fetch(url, { ...init, headers: existingHeaders });
}
