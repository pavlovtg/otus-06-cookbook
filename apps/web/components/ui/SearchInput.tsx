import * as React from 'react';
import { SearchIcon } from '../icons';

export interface SearchInputProps extends Omit<React.InputHTMLAttributes<HTMLInputElement>, 'onChange' | 'value'> {
  value?: string;
  defaultValue?: string;
  onValueChange?: (v: string) => void;
  suggestions?: Array<{ label: string; kind?: string }>;
  onPickSuggestion?: (label: string) => void;
}
export function SearchInput({
  value,
  defaultValue = '',
  onValueChange,
  suggestions = [],
  onPickSuggestion,
  placeholder = 'Поиск…',
  ...rest
}: SearchInputProps) {
  const [inner, setInner] = React.useState(defaultValue);
  const [open, setOpen] = React.useState(false);
  const v = value !== undefined ? value : inner;
  const set = (s: string) => {
    if (value === undefined) setInner(s);
    onValueChange?.(s);
  };
  return (
    <div className="search-wrap">
      <span className="search-icon">
        <SearchIcon size={16} />
      </span>
      <input
        className="search-input"
        placeholder={placeholder}
        value={v}
        onChange={(e) => set(e.target.value)}
        onFocus={() => setOpen(true)}
        onBlur={() => setTimeout(() => setOpen(false), 150)}
        {...rest}
      />
      {suggestions.length > 0 && (
        <div className={['autocomplete', open ? 'is-open' : ''].filter(Boolean).join(' ')}>
          {suggestions.map((s, i) => (
            <div
              key={i}
              className="autocomplete-item"
              onMouseDown={() => {
                onPickSuggestion?.(s.label);
                set(s.label);
              }}
            >
              <span>{s.label}</span>
              {s.kind && <span className="kind">{s.kind}</span>}
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
