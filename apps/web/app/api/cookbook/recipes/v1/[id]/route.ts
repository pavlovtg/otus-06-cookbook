import { NextRequest, NextResponse } from "next/server";

const GATEWAY_URL = process.env["GATEWAY_URL"] ?? "http://api-gateway";
const upstream = (id: string) =>
  `${GATEWAY_URL}/api/cookbook/recipes/v1/${id}`;

interface Params {
  params: Promise<{ id: string }>;
}

export async function GET(_req: NextRequest, { params }: Params) {
  const { id } = await params;
  const res = await fetch(upstream(id), { cache: "no-store" });
  const data: unknown = await res.json();
  return NextResponse.json(data, { status: res.status });
}

export async function PUT(req: NextRequest, { params }: Params) {
  const { id } = await params;
  const body: unknown = await req.json();
  const res = await fetch(upstream(id), {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
    cache: "no-store",
  });
  return new NextResponse(null, { status: res.status });
}

export async function DELETE(_req: NextRequest, { params }: Params) {
  const { id } = await params;
  const res = await fetch(upstream(id), {
    method: "DELETE",
    cache: "no-store",
  });
  return new NextResponse(null, { status: res.status });
}
