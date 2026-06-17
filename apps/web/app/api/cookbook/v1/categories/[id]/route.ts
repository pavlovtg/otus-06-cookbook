import { NextRequest, NextResponse } from "next/server";
import { proxyFetch } from "@/lib/server-fetch";

const GATEWAY_URL = process.env["GATEWAY_URL"] ?? "http://api-gateway";
const UPSTREAM = `${GATEWAY_URL}/api/cookbook/v1/categories`;

export async function PUT(
  req: NextRequest,
  { params }: { params: Promise<{ id: string }> },
) {
  const { id } = await params;
  const body: unknown = await req.json();
  const res = await proxyFetch(req, `${UPSTREAM}/${id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
    cache: "no-store",
  });
  if (res.status === 204) return new NextResponse(null, { status: 204 });
  const data: unknown = await res.json();
  return NextResponse.json(data, { status: res.status });
}

export async function DELETE(
  req: NextRequest,
  { params }: { params: Promise<{ id: string }> },
) {
  const { id } = await params;
  const res = await proxyFetch(req, `${UPSTREAM}/${id}`, {
    method: "DELETE",
    cache: "no-store",
  });
  if (res.status === 204) return new NextResponse(null, { status: 204 });
  const data: unknown = await res.json();
  return NextResponse.json(data, { status: res.status });
}
