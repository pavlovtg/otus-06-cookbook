import * as React from 'react';

export interface BarChartProps {
  data: Array<[string, number]>;
  height?: number;
}
/** Native SVG bar chart with grow-in animation. No external lib. */
export function BarChart({ data, height = 240 }: BarChartProps) {
  const max = Math.max(1, ...data.map((d) => d[1]));
  const w = 600;
  const padL = 32;
  const padB = 28;
  const innerW = w - padL - 8;
  const innerH = height - padB - 8;
  const barW = innerW / data.length - 6;
  return (
    <div className="dash-chart">
      <svg viewBox={`0 0 ${w} ${height}`} width="100%" height={height} preserveAspectRatio="xMidYMid meet">
        <defs>
          <linearGradient id="barGrad" x1="0" y1="0" x2="0" y2="1">
            <stop offset="0%" stopColor="#a279e6" />
            <stop offset="100%" stopColor="#7d79e7" />
          </linearGradient>
        </defs>
        {/* y ticks */}
        {[0, 0.25, 0.5, 0.75, 1].map((t, i) => {
          const y = 8 + innerH * (1 - t);
          return (
            <g key={i}>
              <line x1={padL} x2={w - 8} y1={y} y2={y} stroke="rgba(255,255,255,0.06)" />
              <text x={padL - 6} y={y + 3} fill="#a9a9ac" fontSize={10} textAnchor="end" fontFamily="Inter, sans-serif">
                {Math.round(max * t)}
              </text>
            </g>
          );
        })}
        {data.map((d, i) => {
          const h = (d[1] / max) * innerH;
          const x = padL + i * (barW + 6) + 3;
          const y = 8 + innerH - h;
          return (
            <g key={i}>
              <rect
                className="chart-bar"
                x={x}
                y={y}
                width={barW}
                height={h}
                rx={6}
                fill="url(#barGrad)"
                style={{ animationDelay: `${i * 50}ms` }}
              />
              <text
                x={x + barW / 2}
                y={height - 10}
                fill="#a9a9ac"
                fontSize={11}
                textAnchor="middle"
                fontFamily="Inter, sans-serif"
              >
                {d[0].length > 12 ? d[0].slice(0, 11) + '…' : d[0]}
              </text>
            </g>
          );
        })}
      </svg>
    </div>
  );
}
