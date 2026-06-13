import * as React from 'react';

export interface KpiProps {
  label: string;
  value: number | string;
  animate?: boolean;
}
export function Kpi({ label, value, animate = true }: KpiProps) {
  const [display, setDisplay] = React.useState<number | string>(animate && typeof value === 'number' ? 0 : value);
  React.useEffect(() => {
    if (!animate || typeof value !== 'number') {
      setDisplay(value);
      return;
    }
    const start = performance.now();
    const dur = 700;
    let raf = 0;
    const tick = (t: number) => {
      const p = Math.min(1, (t - start) / dur);
      const eased = 1 - Math.pow(1 - p, 3);
      setDisplay(Math.round(value * eased));
      if (p < 1) raf = requestAnimationFrame(tick);
    };
    raf = requestAnimationFrame(tick);
    return () => cancelAnimationFrame(raf);
  }, [value, animate]);
  return (
    <div className="kpi">
      <span className="label">{label}</span>
      <span className="value">{display}</span>
    </div>
  );
}

export function KpiGrid({ items }: { items: KpiProps[] }) {
  return (
    <div className="kpi-grid">
      {items.map((k, i) => (
        <Kpi key={i} {...k} />
      ))}
    </div>
  );
}
