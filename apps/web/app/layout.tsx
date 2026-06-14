import type { Metadata } from "next";
import { Inter } from "next/font/google";
import Link from "next/link";
import { BookIcon, LeafIcon, LayersIcon } from "@/lib/icons";
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
                <Link href="/"><BookIcon size={14} /><span>Рецепты</span></Link>
                <Link href="/ingredients"><LeafIcon size={14} /><span>Ингредиенты</span></Link>
                <Link href="/categories"><LayersIcon size={14} /><span>Категории</span></Link>
              </nav>
            </div>
          </header>
          <main className="main">{children}</main>
        </div>
      </body>
    </html>
  );
}
