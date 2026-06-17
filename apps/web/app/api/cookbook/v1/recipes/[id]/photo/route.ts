import { NextRequest, NextResponse } from "next/server";
import { proxyFetch } from "@/lib/server-fetch";

const GATEWAY_URL = process.env["GATEWAY_URL"] ?? "http://api-gateway";
const upstream = (id: string) =>
  `${GATEWAY_URL}/api/cookbook/v1/recipes/${id}/photo`;

interface Params {
  params: Promise<{ id: string }>;
}

export async function POST(req: NextRequest, { params }: Params) {
  const { id } = await params;
  const formData = await req.formData();
  const res = await proxyFetch(req, upstream(id), {
    method: "POST",
    body: formData,
    cache: "no-store",
  });
  const data: unknown = await res.json();
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
