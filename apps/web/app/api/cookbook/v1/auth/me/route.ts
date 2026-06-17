import { NextResponse } from "next/server";
import { getSession } from "@/lib/session";

export async function GET(): Promise<NextResponse> {
  const session = await getSession();

  if (!session.user) {
    return NextResponse.json({ error: "Unauthorized" }, { status: 401 });
  }

  return NextResponse.json(session.user);
}
