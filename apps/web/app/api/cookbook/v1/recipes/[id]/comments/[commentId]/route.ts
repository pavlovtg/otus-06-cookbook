import { NextRequest, NextResponse } from "next/server";
import { proxyFetch } from "@/lib/server-fetch";

const GATEWAY_URL = process.env["GATEWAY_URL"] ?? "http://api-gateway";
const upstream = (id: string, commentId: string) =>
  `${GATEWAY_URL}/api/cookbook/v1/recipes/${id}/comments/${commentId}`;

interface Params {
  params: Promise<{ id: string; commentId: string }>;
}

export async function DELETE(req: NextRequest, { params }: Params) {
  const { id, commentId } = await params;
  const res = await proxyFetch(req, upstream(id, commentId), {
    method: "DELETE",
    cache: "no-store",
  });
  return new NextResponse(null, { status: res.status });
}
