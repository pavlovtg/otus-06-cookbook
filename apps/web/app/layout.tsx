import type { Metadata } from "next";
import { Inter } from "next/font/google";
import Link from "next/link";
import "./globals.css";

const inter = Inter({ subsets: ["latin", "cyrillic"] });

export const metadata: Metadata = {
  title: "Кулинарная книга",
  description: "Коллекция рецептов",
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
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
                <Link href="/">Рецепты</Link>
                <Link href="/ingredients">Ингредиенты</Link>
              </nav>
            </div>
          </header>
          <main className="main">{children}</main>
        </div>
      </body>
    </html>
  );
}
