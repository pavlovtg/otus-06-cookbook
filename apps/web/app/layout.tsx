import type { Metadata } from "next";
import { Inter } from "next/font/google";
import Link from "next/link";
import { BookIcon, LeafIcon, LayersIcon, CalendarIcon } from "@/lib/icons";
import { getSession } from "@/lib/session";
import { LogoutButton } from "@/components/features/LogoutButton";
import "./globals.css";

const inter = Inter({ subsets: ["latin", "cyrillic"] });

export const metadata: Metadata = {
  title: "Кулинарная книга",
  description: "Коллекция рецептов",
};

export default async function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const session = await getSession();
  const user = session.user ?? null;

  return (
    <html lang="ru" className={inter.className}>
      <body>
        <div className="app">
          <header className="header">
            <div className="header-inner">
              <div className="brand">
                <div className="brand-mark">К</div>
                Кулинарная книга
              </div>
              <nav className="nav">
                <Link href="/"><BookIcon size={14} /><span>Рецепты</span></Link>
                {user && (
                  <Link href="/planner"><CalendarIcon size={14} /><span>Планировщик</span></Link>
                )}
                <Link href="/ingredients"><LeafIcon size={14} /><span>Ингредиенты</span></Link>
                <Link href="/categories"><LayersIcon size={14} /><span>Категории</span></Link>
              </nav>
              <div id="user-slot">
                {user ? (
                  <div className="user-chip">
                    <span className="avatar">
                      {user.displayName.slice(0, 1).toUpperCase()}
                    </span>
                    <span>{user.displayName}</span>
                    {user.role === "admin" && (
                      <span className="role-tag">admin</span>
                    )}
                    <LogoutButton />
                  </div>
                ) : (
                  <div style={{ display: "flex", gap: 8 }}>
                    <Link href="/login" className="btn btn-ghost btn-sm">
                      Войти
                    </Link>
                    <Link href="/register" className="btn btn-primary btn-sm">
                      Регистрация
                    </Link>
                  </div>
                )}
              </div>
            </div>
          </header>
          <main className="main">{children}</main>
        </div>
      </body>
    </html>
  );
}
