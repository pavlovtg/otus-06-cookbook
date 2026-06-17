import { NextRequest, NextResponse } from "next/server";
import { getIronSession } from "iron-session";
import { getSessionOptions, type SessionData } from "@/lib/session";
import { AuthResponseSchema, RegisterRequestSchema } from "@/lib/schemas/auth";

const GATEWAY_URL = process.env["GATEWAY_URL"] ?? "http://api-gateway";

export async function POST(request: NextRequest): Promise<NextResponse> {
  let body: unknown;
  try {
    body = await request.json();
  } catch {
    return NextResponse.json({ error: "Invalid JSON" }, { status: 400 });
  }

  const parsed = RegisterRequestSchema.safeParse(body);
  if (!parsed.success) {
    return NextResponse.json({ error: parsed.error.flatten() }, { status: 400 });
  }

  const upstream = await fetch(`${GATEWAY_URL}/api/v1/auth/register`, {
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

  const response = NextResponse.json(authResponse);
  const session = await getIronSession<SessionData>(request, response, getSessionOptions());
  session.user = authResponse.user;
  session.token = authResponse.token;
  await session.save();

  return response;
}
