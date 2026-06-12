import { NextRequest, NextResponse } from "next/server";

const GATEWAY_URL = process.env["GATEWAY_URL"] ?? "http://api-gateway";
const UPSTREAM = `${GATEWAY_URL}/api/cookbook/v1/recipes`;

export async function GET() {
  const res = await fetch(UPSTREAM, { cache: "no-store" });
  const data: unknown = await res.json();
  return NextResponse.json(data, { status: res.status });
}

export async function POST(req: NextRequest) {
  const body: unknown = await req.json();
  const res = await fetch(UPSTREAM, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
    cache: "no-store",
  });
  const data: unknown = await res.json();
  return NextResponse.json(data, { status: res.status });
}
