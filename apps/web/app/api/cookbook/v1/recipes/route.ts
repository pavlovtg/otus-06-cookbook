import { NextRequest, NextResponse } from "next/server";
import { proxyFetch } from "@/lib/server-fetch";

const GATEWAY_URL = process.env["GATEWAY_URL"] ?? "http://api-gateway";
const UPSTREAM = `${GATEWAY_URL}/api/cookbook/v1/recipes`;

export async function GET(req: NextRequest) {
  const { searchParams } = req.nextUrl;
  const page = searchParams.get("page");
  const pageSize = searchParams.get("pageSize");
  const q = searchParams.get("q");
  const sort = searchParams.get("sort");

  const upstreamUrl = new URL(UPSTREAM);
  if (page) upstreamUrl.searchParams.set("page", page);
  if (pageSize) upstreamUrl.searchParams.set("pageSize", pageSize);
  if (q) upstreamUrl.searchParams.set("q", q);
  if (sort) upstreamUrl.searchParams.set("sort", sort);

  const res = await fetch(upstreamUrl.toString(), { cache: "no-store" });
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
