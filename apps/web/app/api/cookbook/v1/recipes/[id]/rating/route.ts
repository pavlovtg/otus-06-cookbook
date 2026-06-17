import { NextRequest, NextResponse } from "next/server";
import { proxyFetch } from "@/lib/server-fetch";

const GATEWAY_URL = process.env["GATEWAY_URL"] ?? "http://api-gateway";
const upstream = (id: string) =>
  `${GATEWAY_URL}/api/cookbook/v1/recipes/${id}/rating`;

interface Params {
  params: Promise<{ id: string }>;
}

export async function PUT(req: NextRequest, { params }: Params) {
  const { id } = await params;
  const body: unknown = await req.json();
  const res = await proxyFetch(req, upstream(id), {
    method: "PUT",
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

export async function DELETE(req: NextRequest, { params }: Params) {
  const { id } = await params;
  const res = await proxyFetch(req, upstream(id), {
    method: "DELETE",
    cache: "no-store",
  });
  return new NextResponse(null, { status: res.status });
}
