export const metadata = {
  title: "Кулинарная книга",
  description: "Коллекция рецептов",
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="ru">
      <body>{children}</body>
    </html>
  );
}
