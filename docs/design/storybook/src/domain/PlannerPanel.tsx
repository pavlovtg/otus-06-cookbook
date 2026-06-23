import * as React from 'react';
import { Segmented } from '../components/Segmented';
import { SearchInput } from '../components/SearchInput';

export type PlannerMode = 'all' | 'fav' | 'mine';

export interface PlannerPanelProps {
  mode?: PlannerMode;
  onModeChange?: (mode: PlannerMode) => void;
  onSearch?: (q: string) => void;
  children?: React.ReactNode;
}

const MODE_OPTIONS: { value: PlannerMode; label: string }[] = [
  { value: 'all', label: 'Все' },
  { value: 'fav', label: 'Избранное' },
  { value: 'mine', label: 'Мои' },
];

export function PlannerPanel({ mode = 'all', onModeChange, onSearch, children }: PlannerPanelProps) {
  return (
    <div className="planner-panel">
      <div style={{ display: 'flex', gap: 12, alignItems: 'center', flexWrap: 'wrap' }}>
        <Segmented
          options={MODE_OPTIONS}
          value={mode}
          onChange={(v) => onModeChange?.(v as PlannerMode)}
        />
        <div style={{ flex: 1, minWidth: 200 }}>
          <SearchInput placeholder="Поиск рецептов…" onValueChange={onSearch} />
        </div>
      </div>
      <div className="scroll">{children}</div>
    </div>
  );
}
