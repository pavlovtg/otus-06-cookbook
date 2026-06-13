import * as React from 'react';
import { BookIcon, CalendarIcon, CartIcon, LeafIcon, LayersIcon, ChartIcon } from '../icons';
import { Button } from '../components/Button';

export type Role = 'guest' | 'user' | 'admin';

export interface HeaderProps {
  role?: Role;
  userName?: string;
  active?: string;
  onLogout?: () => void;
}
export function Header({ role = 'guest', userName, active = '#/recipes', onLogout }: HeaderProps) {
  const items = [
    { hash: '#/recipes', label: 'Рецепты', icon: <BookIcon size={14} />, requires: undefined },
    { hash: '#/planner', label: 'Планировщик', icon: <CalendarIcon size={14} />, requires: 'user' },
    { hash: '#/shopping-list', label: 'Список покупок', icon: <CartIcon size={14} />, requires: 'user' },
    { hash: '#/ingredients', label: 'Ингредиенты', icon: <LeafIcon size={14} />, requires: 'user' },
    { hash: '#/categories', label: 'Категории', icon: <LayersIcon size={14} />, requires: 'admin' },
    { hash: '#/dashboard', label: 'Дашборд', icon: <ChartIcon size={14} />, requires: undefined },
  ] as const;
  const visible = items.filter((i) => {
    if (i.requires === 'user') return role === 'user' || role === 'admin';
    if (i.requires === 'admin') return role === 'admin';
    return true;
  });
  return (
    <header className="header">
      <div className="header-inner">
        <a className="brand" href="#/recipes">
          <span className="brand-mark">C</span>
          <span>Cookbook</span>
        </a>
        <nav className="nav">
          {visible.map((i) => (
            <a key={i.hash} href={i.hash} className={active.startsWith(i.hash) ? 'is-active' : ''}>
              {i.icon}
              <span>{i.label}</span>
            </a>
          ))}
        </nav>
        <div>
          {role === 'guest' ? (
            <div style={{ display: 'flex', gap: 8 }}>
              <Button variant="ghost" size="sm">Войти</Button>
              <Button variant="primary" size="sm">Регистрация</Button>
            </div>
          ) : (
            <div className="user-chip">
              <span className="avatar">{(userName || '?').slice(0, 1)}</span>
              <span>{userName}</span>
              {role === 'admin' && <span className="role-tag">admin</span>}
              <Button variant="ghost" size="sm" onClick={onLogout}>Выйти</Button>
            </div>
          )}
        </div>
      </div>
    </header>
  );
}
