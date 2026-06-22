import { NextRequest, NextResponse } from "next/server";
import { proxyFetch } from "@/lib/server-fetch";

const GATEWAY_URL = process.env["GATEWAY_URL"] ?? "http://api-gateway";
const upstream = (id: string, search: string) =>
  `${GATEWAY_URL}/api/cookbook/v1/recipes/${id}/comments${search ? `?${search}` : ""}`;

interface Params {
  params: Promise<{ id: string }>;
}

export async function GET(req: NextRequest, { params }: Params) {
  const { id } = await params;
  const search = req.nextUrl.searchParams.toString();
  const res = await proxyFetch(req, upstream(id, search), {
    method: "GET",
    cache: "no-store",
  });
  const text = await res.text();
  if (!text) {
    return new NextResponse(null, { status: res.status });
  }
  const data: unknown = JSON.parse(text);
  return NextResponse.json(data, { status: res.status });
}

export async function POST(req: NextRequest, { params }: Params) {
  const { id } = await params;
  const body: unknown = await req.json();
  const res = await proxyFetch(req, upstream(id, ""), {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
    cache: "no-store",
  });
  const text = await res.text();
  if (!text) {
    return new NextResponse(null, { status: res.status });
  }
  const data: unknown = JSON.parse(text);
  return NextResponse.json(data, { status: res.status });
}
