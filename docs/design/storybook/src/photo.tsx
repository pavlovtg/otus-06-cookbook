import * as React from 'react';

const HUES: [string, string][] = [
  ['#7d79e7', '#a279e6'],
  ['#ff6cb2', '#9968ff'],
  ['#ffaf56', '#ff6cb2'],
  ['#76d9a3', '#7d79e7'],
  ['#9968ff', '#7d79e7'],
  ['#ffaf56', '#9968ff'],
  ['#a279e6', '#ff6cb2'],
  ['#76d9a3', '#9968ff'],
];

function hash(s: string): number {
  s = String(s);
  let h = 0;
  for (let i = 0; i < s.length; i++) h = (h << 5) - h + s.charCodeAt(i);
  return h;
}

export function RecipePhoto({ seed, title }: { seed: string; title: string }) {
  const h = HUES[Math.abs(hash(seed)) % HUES.length];
  const initials = (title || '?')
    .split(/\s+/)
    .slice(0, 2)
    .map((s) => s[0] || '')
    .join('')
    .toUpperCase();
  const id = Math.abs(hash(seed));
  return (
    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 320 200" preserveAspectRatio="xMidYMid slice">
      <defs>
        <linearGradient id={`g${id}`} x1="0" y1="0" x2="1" y2="1">
          <stop offset="0%" stopColor={h[0]} />
          <stop offset="100%" stopColor={h[1]} />
        </linearGradient>
        <radialGradient id={`r${id}`} cx="80%" cy="20%" r="60%">
          <stop offset="0%" stopColor="rgba(255,255,255,0.35)" />
          <stop offset="100%" stopColor="rgba(255,255,255,0)" />
        </radialGradient>
      </defs>
      <rect width="320" height="200" fill={`url(#g${id})`} />
      <rect width="320" height="200" fill={`url(#r${id})`} />
      <text
        x="50%"
        y="55%"
        textAnchor="middle"
        fill="rgba(255,255,255,0.85)"
        fontFamily="Inter, sans-serif"
        fontWeight={500}
        fontSize={56}
        letterSpacing={-2}
      >
        {initials}
      </text>
    </svg>
  );
}
