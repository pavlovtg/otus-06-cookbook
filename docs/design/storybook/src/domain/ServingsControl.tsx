import * as React from 'react';
import { PlusIcon, MinusIcon } from '../icons';

export interface ServingsControlProps {
  value?: number;
  defaultValue?: number;
  min?: number;
  max?: number;
  onChange?: (v: number) => void;
}
export function ServingsControl({ value, defaultValue = 2, min = 1, max = 99, onChange }: ServingsControlProps) {
  const [inner, setInner] = React.useState(defaultValue);
  const v = value !== undefined ? value : inner;
  const set = (n: number) => {
    const next = Math.max(min, Math.min(max, n));
    if (value === undefined) setInner(next);
    onChange?.(next);
  };
  return (
    <div className="servings-control">
      <button onClick={() => set(v - 1)} aria-label="Меньше">
        <MinusIcon size={14} />
      </button>
      <span className="value">{v}</span>
      <button onClick={() => set(v + 1)} aria-label="Больше">
        <PlusIcon size={14} />
      </button>
    </div>
  );
}
