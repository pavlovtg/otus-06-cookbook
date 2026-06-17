import type { Meta, StoryObj } from '@storybook/react';
import * as React from 'react';
import { HeartIcon, HeartFillIcon } from '../icons';

const meta: Meta = {
  title: 'Domain/RecipesSidebar',
  parameters: { layout: 'centered' },
};
export default meta;
type S = StoryObj;

// ─── Mode switcher (Все / Избранное) ─────────────────────────────────────────

function ModeAside() {
  const [mode, setMode] = React.useState<'all' | 'favorites'>('all');

  const modes = [
    { value: 'all' as const, label: 'Все рецепты', icon: null },
    { value: 'favorites' as const, label: 'Избранное', icon: <HeartIcon size={14} /> },
  ];

  return (
    <div className="aside-block" style={{ minWidth: 200 }}>
      <span className="aside-label">Показать</span>
      {modes.map((m) => (
        <div
          key={m.value}
          data-mode={m.value}
          className={['aside-item', mode === m.value ? 'is-active' : ''].filter(Boolean).join(' ')}
          role="button"
          tabIndex={0}
          onClick={() => setMode(m.value)}
          onKeyDown={(e) => e.key === 'Enter' && setMode(m.value)}
          style={{ display: 'flex', alignItems: 'center', gap: 6 }}
        >
          {m.icon && <span style={{ display: 'inline-flex' }}>{m.icon}</span>}
          <span>{m.label}</span>
        </div>
      ))}
    </div>
  );
}

export const ModeSwitcher: S = {
  name: 'Переключатель режима (Все / Избранное)',
  render: () => <ModeAside />,
};

export const ModeSwitcherFavoritesActive: S = {
  name: 'Переключатель: активен режим «Избранное»',
  render: () => {
    const [mode, setMode] = React.useState<'all' | 'favorites'>('favorites');
    const modes = [
      { value: 'all' as const, label: 'Все рецепты', icon: null },
      { value: 'favorites' as const, label: 'Избранное', icon: <HeartFillIcon size={14} /> },
    ];
    return (
      <div className="aside-block" style={{ minWidth: 200 }}>
        <span className="aside-label">Показать</span>
        {modes.map((m) => (
          <div
            key={m.value}
            data-mode={m.value}
            className={['aside-item', mode === m.value ? 'is-active' : ''].filter(Boolean).join(' ')}
            role="button"
            tabIndex={0}
            onClick={() => setMode(m.value)}
            onKeyDown={(e) => e.key === 'Enter' && setMode(m.value)}
            style={{ display: 'flex', alignItems: 'center', gap: 6 }}
          >
            {m.icon && <span style={{ display: 'inline-flex' }}>{m.icon}</span>}
            <span>{m.label}</span>
          </div>
        ))}
      </div>
    );
  },
};

export const Playground: S = {
  name: 'Playground: сайдбар с режимом и сортировкой',
  render: () => {
    const [mode, setMode] = React.useState<'all' | 'favorites'>('all');
    const [sort, setSort] = React.useState('');

    return (
      <div style={{ display: 'flex', flexDirection: 'column', gap: 8, minWidth: 220 }}>
        <div className="aside-block">
          <span className="aside-label">Показать</span>
          {[
            { value: 'all' as const, label: 'Все рецепты' },
            { value: 'favorites' as const, label: 'Избранное' },
          ].map((m) => (
            <div
              key={m.value}
              data-mode={m.value}
              className={['aside-item', mode === m.value ? 'is-active' : ''].filter(Boolean).join(' ')}
              role="button"
              tabIndex={0}
              onClick={() => setMode(m.value)}
              onKeyDown={(e) => e.key === 'Enter' && setMode(m.value)}
            >
              <span>{m.label}</span>
            </div>
          ))}
        </div>
        <div className="aside-block">
          <span className="aside-label">Сортировка</span>
          {[
            { value: 'title_asc', label: 'А → Я' },
            { value: 'title_desc', label: 'Я → А' },
          ].map((opt) => (
            <div
              key={opt.value}
              className={['aside-item', sort === opt.value ? 'is-active' : ''].filter(Boolean).join(' ')}
              role="button"
              tabIndex={0}
              onClick={() => setSort(opt.value)}
              onKeyDown={(e) => e.key === 'Enter' && setSort(opt.value)}
            >
              <span>{opt.label}</span>
            </div>
          ))}
        </div>
        <div style={{ marginTop: 8, fontSize: 12, color: 'var(--fg-secondary)' }}>
          Режим: <b>{mode}</b> · Сортировка: <b>{sort || 'нет'}</b>
        </div>
      </div>
    );
  },
};
