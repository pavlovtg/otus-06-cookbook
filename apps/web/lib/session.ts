import { getIronSession, type IronSession, type SessionOptions } from "iron-session";
import { cookies } from "next/headers";
import type { UserDto } from "@/lib/schemas/auth";

export interface SessionData {
  user?: UserDto;
  token?: string;
}

export function getSessionOptions(): SessionOptions {
  const secret = process.env["Session__Secret"];
  if (!secret) {
    throw new Error("Session__Secret environment variable is not set");
  }
  return {
    password: secret,
    cookieName: "cookbook_session",
    cookieOptions: {
      secure: process.env["Session__CookieSecure"] === "true",
      httpOnly: true,
      sameSite: "lax",
    },
  };
}

export async function getSession(): Promise<IronSession<SessionData>> {
  const cookieStore = await cookies();
  return getIronSession<SessionData>(cookieStore, getSessionOptions());
}
