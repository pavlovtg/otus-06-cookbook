import * as React from 'react';

export interface SegmentedOption<V extends string = string> {
  value: V;
  label: React.ReactNode;
}
export interface SegmentedProps<V extends string = string> {
  options: SegmentedOption<V>[];
  value?: V;
  defaultValue?: V;
  onChange?: (v: V) => void;
}
export function Segmented<V extends string = string>({ options, value, defaultValue, onChange }: SegmentedProps<V>) {
  const [inner, setInner] = React.useState<V>(defaultValue ?? options[0].value);
  const v = value !== undefined ? value : inner;
  const set = (next: V) => {
    if (value === undefined) setInner(next);
    onChange?.(next);
  };
  return (
    <div className="segmented" role="tablist">
      {options.map((o) => (
        <button
          key={o.value}
          role="tab"
          aria-selected={v === o.value}
          className={v === o.value ? 'is-active' : ''}
          onClick={() => set(o.value)}
        >
          {o.label}
        </button>
      ))}
    </div>
  );
}

export interface TabsProps<V extends string = string> extends SegmentedProps<V> {}
export function Tabs<V extends string = string>(props: TabsProps<V>) {
  const { options, value, defaultValue, onChange } = props;
  const [inner, setInner] = React.useState<V>(defaultValue ?? options[0].value);
  const v = value !== undefined ? value : inner;
  const set = (n: V) => {
    if (value === undefined) setInner(n);
    onChange?.(n);
  };
  return (
    <div className="tabs">
      {options.map((o) => (
        <button key={o.value} className={v === o.value ? 'is-active' : ''} onClick={() => set(o.value)}>
          {o.label}
        </button>
      ))}
    </div>
  );
}
