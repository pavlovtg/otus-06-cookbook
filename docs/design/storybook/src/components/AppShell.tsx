import type { ReactNode } from 'react';

export type NavItem = { id: string; label: string };

export type AppShellProps = {
  active?: string;
  nav?: NavItem[];
  user?: { name: string } | null;
  onNav?: (id: string) => void;
  onLogin?: () => void;
  onLogout?: () => void;
  children?: ReactNode;
};

const DEFAULT_NAV: NavItem[] = [
  { id: 'recipes', label: 'Рецепты' },
  { id: 'favorites', label: 'Избранное' },
  { id: 'mealplan', label: 'План меню' },
  { id: 'shopping', label: 'Список покупок' },
  { id: 'dashboard', label: 'Дашборд' },
];

export function AppShell({ active = 'recipes', nav = DEFAULT_NAV, user, onNav, onLogin, onLogout, children }: AppShellProps) {
  return (
    <>
      {/* style: header */}
      <header className="app-header">
        <div className="app-header-inner">
          <div className="brand">🍳 Книга рецептов</div>
          {/* style: nav */}
          <nav className="app-nav">
            {nav.map((n) => (
              <a key={n.id} href={'#' + n.id} className={n.id === active ? 'active' : ''}
                onClick={(e) => { e.preventDefault(); onNav?.(n.id); }}>{n.label}</a>
            ))}
          </nav>
          <div className="user-box">
            {user ? (
              <>
                <span>👤 {user.name}</span>
                <button className="btn sm" onClick={onLogout}>Выйти</button>
              </>
            ) : (
              <button className="btn sm primary" onClick={onLogin}>Войти</button>
            )}
          </div>
        </div>
      </header>
      <main className="app-main">{children}</main>
    </>
  );
}
