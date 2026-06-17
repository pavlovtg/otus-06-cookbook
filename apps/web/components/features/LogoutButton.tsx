"use client";

import { useRouter } from "next/navigation";
import { logout } from "@/lib/bff/auth";

export function LogoutButton() {
  const router = useRouter();

  async function handleLogout() {
    await logout();
    router.push("/");
    router.refresh();
  }

  return (
    <button className="btn btn-ghost btn-sm" onClick={handleLogout}>
      Выйти
    </button>
  );
}
