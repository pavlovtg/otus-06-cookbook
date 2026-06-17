import { NextRequest, NextResponse } from "next/server";
import { cookies } from "next/headers";
import { getIronSession } from "iron-session";
import { getSessionOptions, type SessionData } from "@/lib/session";
import { AuthResponseSchema, LoginRequestSchema } from "@/lib/schemas/auth";

const GATEWAY_URL = process.env["GATEWAY_URL"] ?? "http://api-gateway";

export async function POST(request: NextRequest): Promise<NextResponse> {
  let body: unknown;
  try {
    body = await request.json();
  } catch {
    return NextResponse.json({ error: "Invalid JSON" }, { status: 400 });
  }

  const parsed = LoginRequestSchema.safeParse(body);
  if (!parsed.success) {
    return NextResponse.json({ error: parsed.error.flatten() }, { status: 400 });
  }

  const upstream = await fetch(`${GATEWAY_URL}/api/cookbook/v1/auth/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(parsed.data),
    cache: "no-store",
  });

  if (!upstream.ok) {
    const text = await upstream.text();
    return NextResponse.json({ error: text }, { status: upstream.status });
  }

  const data: unknown = await upstream.json();
  const authResponse = AuthResponseSchema.parse(data);

  const cookieStore = await cookies();
  const session = await getIronSession<SessionData>(cookieStore, getSessionOptions());
  session.user = authResponse.user;
  session.token = authResponse.token;
  await session.save();

  return NextResponse.json(authResponse);
}
