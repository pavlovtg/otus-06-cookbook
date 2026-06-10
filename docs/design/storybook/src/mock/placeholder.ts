export function placeholderSVG(label: string, color?: string): string {
  const c =
    color ||
    '#' +
      (
        Math.floor(Math.abs(Math.sin(label.length * 9.7 + label.charCodeAt(0)) * 16777215)) % 16777215
      )
        .toString(16)
        .padStart(6, '0');
  const esc = (s: string) =>
    s.replace(/[&<>"']/g, (m) => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[m]!));
  const svg = `<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 400 300'><rect width='400' height='300' fill='${c}'/><text x='50%' y='50%' fill='#fff' font-family='sans-serif' font-size='28' text-anchor='middle' dominant-baseline='middle'>${esc(label).slice(0, 18)}</text></svg>`;
  return 'data:image/svg+xml;utf8,' + encodeURIComponent(svg);
}
