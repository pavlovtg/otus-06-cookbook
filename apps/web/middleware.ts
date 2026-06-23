import { NextRequest, NextResponse } from "next/server";
import { getIronSession } from "iron-session";
import { getSessionOptions, type SessionData } from "@/lib/session";

const PROTECTED_PATHS = [/^\/recipes\/new$/, /^\/recipes\/[^/]+\/edit$/];
const ADMIN_PATHS = [/^\/categories(\/.*)?$/];

export async function middleware(request: NextRequest): Promise<NextResponse> {
  const { pathname } = request.nextUrl;

  const isAdminPath = ADMIN_PATHS.some((re) => re.test(pathname));
  const isProtected = isAdminPath || PROTECTED_PATHS.some((re) => re.test(pathname));

  if (!isProtected) {
    return NextResponse.next();
  }

  const response = NextResponse.next();
  const session = await getIronSession<SessionData>(request, response, getSessionOptions());

  if (!session.user) {
    const loginUrl = new URL("/login", request.url);
    loginUrl.searchParams.set("from", pathname);
    return NextResponse.redirect(loginUrl);
  }

  if (isAdminPath && session.user.role !== "admin") {
    return NextResponse.redirect(new URL("/", request.url));
  }

  return response;
}

export const config = {
  matcher: ["/recipes/new", "/recipes/:id/edit", "/categories", "/categories/:path*"],
};
