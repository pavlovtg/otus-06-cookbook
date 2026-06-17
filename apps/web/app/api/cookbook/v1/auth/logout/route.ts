import { NextResponse } from "next/server";
import { getSession } from "@/lib/session";

const GATEWAY_URL = process.env["GATEWAY_URL"] ?? "http://api-gateway";

export async function POST(): Promise<NextResponse> {
  const session = await getSession();

  if (session.token) {
    await fetch(`${GATEWAY_URL}/api/v1/auth/logout`, {
      method: "POST",
      headers: { Authorization: `Bearer ${session.token}` },
      cache: "no-store",
    }).catch(() => {
      // best-effort: очищаем сессию даже если gateway недоступен
    });
  }

  session.destroy();

  return NextResponse.json({ ok: true });
}
