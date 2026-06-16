import * as React from 'react';
import { XIcon } from '../icons';
import type { Category, CategoryType } from '../mocks';
import { CATEGORY_TYPE_LABELS } from '../mocks';

interface CategoryTagInputProps {
  categories: Category[];
  value: string[];
  onChange: (ids: string[]) => void;
}

export function CategoryTagInput({ categories, value, onChange }: CategoryTagInputProps) {
  const [query, setQuery] = React.useState('');
  const [open, setOpen] = React.useState(false);
  const inputRef = React.useRef<HTMLInputElement>(null);

  const selectedCats = value
    .map((id) => categories.find((c) => c.id === id))
    .filter((c): c is Category => c !== undefined);

  const suggestions = query.trim().length > 0
    ? categories
        .filter((c) => c.name.toLowerCase().includes(query.toLowerCase().trim()))
        .slice(0, 8)
    : [];

  function addCategory(cat: Category) {
    // Замена при совпадении типа: один тип — одна категория
    const filtered = value.filter((id) => {
      const existing = categories.find((c) => c.id === id);
      return existing?.type !== cat.type;
    });
    onChange([...filtered, cat.id]);
    setQuery('');
    setOpen(false);
    inputRef.current?.focus();
  }

  function removeCategory(id: string) {
    onChange(value.filter((v) => v !== id));
  }

  return (
    <div className="tag-input">
      {selectedCats.map((c) => (
        <span key={c.id} className="chip">
          <span>{c.name}</span>
          <button
            type="button"
            onClick={() => removeCategory(c.id)}
            aria-label={`Удалить ${c.name}`}
          >
            <XIcon size={12} />
          </button>
        </span>
      ))}

      <div style={{ position: 'relative', flex: 1, minWidth: 120 }}>
        <input
          ref={inputRef}
          type="text"
          value={query}
          onChange={(e) => {
            setQuery(e.target.value);
            setOpen(true);
          }}
          onFocus={() => setOpen(true)}
          onBlur={() => setTimeout(() => setOpen(false), 150)}
          placeholder={selectedCats.length > 0 ? 'Добавьте ещё…' : 'Начните вводить категорию…'}
          style={{
            background: 'transparent',
            border: 0,
            outline: 'none',
            color: 'var(--fg-primary)',
            fontSize: 14,
            padding: '4px 6px',
            width: '100%',
          }}
        />

        {open && suggestions.length > 0 && (
          <div className="autocomplete is-open">
            {suggestions.map((c) => (
              <div
                key={c.id}
                className="autocomplete-item"
                onMouseDown={(e) => {
                  e.preventDefault();
                  addCategory(c);
                }}
              >
                <span>{c.name}</span>
                <span className="kind">{CATEGORY_TYPE_LABELS[c.type as CategoryType]}</span>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
