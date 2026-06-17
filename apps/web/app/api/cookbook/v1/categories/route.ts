import { NextRequest, NextResponse } from "next/server";
import { proxyFetch } from "@/lib/server-fetch";

const GATEWAY_URL = process.env["GATEWAY_URL"] ?? "http://api-gateway";
const UPSTREAM = `${GATEWAY_URL}/api/cookbook/v1/categories`;

export async function GET(req: NextRequest) {
  const res = await proxyFetch(req, UPSTREAM, { cache: "no-store" });
  const data: unknown = await res.json();
  return NextResponse.json(data, { status: res.status });
}

export async function POST(req: NextRequest) {
  const body: unknown = await req.json();
  const res = await proxyFetch(req, UPSTREAM, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
    cache: "no-store",
  });
  const data: unknown = await res.json();
  return NextResponse.json(data, { status: res.status });
}
