"use client";

import * as React from "react";

export interface KpiProps {
  label: string;
  value: number | string;
  animate?: boolean;
}

export function Kpi({ label, value, animate = true }: KpiProps) {
  const shouldAnimate = animate && typeof value === "number";

  const [animated, setAnimated] = React.useState<number>(
    shouldAnimate ? 0 : 0,
  );

  React.useEffect(() => {
    if (!shouldAnimate) return;
    const numValue = value as number;
    const start = performance.now();
    const dur = 700;
    let raf = 0;
    const tick = (t: number) => {
      const p = Math.min(1, (t - start) / dur);
      const eased = 1 - Math.pow(1 - p, 3);
      setAnimated(Math.round(numValue * eased));
      if (p < 1) raf = requestAnimationFrame(tick);
    };
    raf = requestAnimationFrame(tick);
    return () => cancelAnimationFrame(raf);
  }, [value, shouldAnimate]);

  const display = shouldAnimate ? animated : value;

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
