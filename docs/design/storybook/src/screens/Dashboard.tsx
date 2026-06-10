import { useEffect, useRef, useState } from 'react';
import { fakeApi } from '../mock/fakeApi';
import type { DashboardStats } from '../mock/types';

// Chart.js available globally via preview-head CDN
declare const Chart: any;

export function Dashboard() {
  const [stats, setStats] = useState<DashboardStats | null>(null);
  const catRef = useRef<HTMLCanvasElement>(null);
  const topRef = useRef<HTMLCanvasElement>(null);

  useEffect(() => { fakeApi.getDashboardStats().then(setStats); }, []);

  useEffect(() => {
    if (!stats || typeof Chart === 'undefined') return;
    const charts: any[] = [];
    if (catRef.current) {
      charts.push(new Chart(catRef.current, {
        type: 'bar',
        data: {
          labels: stats.byCategory.map((x) => x.label),
          datasets: [{ label: 'Рецептов', data: stats.byCategory.map((x) => x.value), backgroundColor: '#2f6f4e' }],
        },
        options: { plugins: { legend: { display: false } } },
      }));
    }
    if (topRef.current) {
      charts.push(new Chart(topRef.current, {
        type: 'doughnut',
        data: {
          labels: stats.topRated.map((x) => x.title),
          datasets: [{ data: stats.topRated.map((x) => x.avg), backgroundColor: ['#2f6f4e', '#e8a73c', '#b3261e', '#a86b00', '#255a3e'] }],
        },
      }));
    }
    return () => { charts.forEach((c) => c.destroy()); };
  }, [stats]);

  if (!stats) return <div className="muted">загрузка…</div>;
  if (typeof Chart === 'undefined') {
    return <div className="notice">Chart.js не загружен. Откройте сторис в обычном режиме (preview-head подключает CDN).</div>;
  }

  return (
    <>
      <h1>Дашборд</h1>
      {/* style: dashboard */}
      <div className="dash-grid">
        <div className="dash-card">
          <h3>Всего</h3>
          <ul>
            <li>Рецептов: {stats.totals.recipes}</li>
            <li>Пользователей: {stats.totals.users}</li>
            <li>Комментариев: {stats.totals.comments}</li>
            <li>Заполненность плана: {stats.planFilled} / {stats.planTotal}</li>
          </ul>
        </div>
        <div className="dash-card">
          <h3>Рецепты по категориям</h3>
          <canvas ref={catRef} height={220} />
        </div>
        <div className="dash-card">
          <h3>Топ-5 по рейтингу</h3>
          <canvas ref={topRef} height={220} />
        </div>
      </div>
    </>
  );
}
