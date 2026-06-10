import { useEffect, useState } from 'react';
import { fakeApi } from '../mock/fakeApi';
import type { MealPlan, RecipeWithMeta } from '../mock/types';

const DAYS = ['Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб', 'Вс'];
const SLOTS = [
  { key: 'breakfast', label: 'Завтрак' },
  { key: 'lunch', label: 'Обед' },
  { key: 'dinner', label: 'Ужин' },
] as const;

export function MealPlanner() {
  const [plan, setPlan] = useState<MealPlan | null>(null);
  const [pool, setPool] = useState<RecipeWithMeta[]>([]);
  const [over, setOver] = useState<string | null>(null);

  useEffect(() => {
    fakeApi.getMealPlan().then(setPlan);
    fakeApi.getRecipes({ page: 1, perPage: 12 }).then((r) => setPool(r.items));
  }, []);

  function onDragStart(e: any, payload: string) {
    e.dataTransfer.setData('text/plain', payload);
  }
  function onDrop(e: any, day: number, slot: 'breakfast' | 'lunch' | 'dinner') {
    e.preventDefault();
    setOver(null);
    if (!plan) return;
    const payload = e.dataTransfer.getData('text/plain');
    const [src, recipeId] = payload.split('|');
    const next: MealPlan = JSON.parse(JSON.stringify(plan));
    if (src.startsWith('cell:')) {
      const [, sd, ss] = src.split(':');
      next[Number(sd)]![ss as 'breakfast' | 'lunch' | 'dinner'] = next[Number(sd)]![ss as 'breakfast' | 'lunch' | 'dinner'].filter((x: string) => x !== recipeId);
    }
    if (!next[day]![slot].includes(recipeId!)) next[day]![slot].push(recipeId!);
    setPlan(next);
    fakeApi.updateMealPlan(next);
  }

  if (!plan) return <div className="muted">загрузка…</div>;

  return (
    <>
      <h1>План меню на неделю</h1>
      <p className="muted">Перетащите рецепты из палитры в нужный слот.</p>

      {/* style: meal-plan-grid */}
      <div className="mp-scroll">
        <div className="mp-grid">
          <div></div>
          {DAYS.map((d) => <div key={d} className="mp-head">{d}</div>)}
          {SLOTS.map((s) => (
            <>
              <div key={s.key + '-side'} className="mp-side">{s.label}</div>
              {DAYS.map((_, d) => {
                const id = d + ':' + s.key;
                return (
                  <div
                    key={id}
                    className={'mp-cell' + (over === id ? ' over' : '')}
                    onDragOver={(e: any) => { e.preventDefault(); setOver(id); }}
                    onDragLeave={() => setOver((x) => (x === id ? null : x))}
                    onDrop={(e) => onDrop(e, d, s.key)}
                  >
                    {plan[d]![s.key].map((rid) => {
                      const rec = pool.find((x) => x.id === rid);
                      return (
                        <div
                          key={rid}
                          className="mp-slot-item"
                          draggable
                          onDragStart={(e) => onDragStart(e, 'cell:' + d + ':' + s.key + '|' + rid)}
                        >
                          <span>{rec ? rec.title : rid}</span>
                          <button
                            className="btn sm ghost"
                            onClick={() => {
                              const next: MealPlan = JSON.parse(JSON.stringify(plan));
                              next[d]![s.key] = next[d]![s.key].filter((x) => x !== rid);
                              setPlan(next);
                              fakeApi.updateMealPlan(next);
                            }}
                          >✕</button>
                        </div>
                      );
                    })}
                  </div>
                );
              })}
            </>
          ))}
        </div>
      </div>

      <h2 style={{ marginTop: 24 }}>Палитра рецептов</h2>
      <div className="recipe-grid">
        {pool.map((r) => (
          <div
            key={r.id}
            className="recipe-card"
            draggable
            onDragStart={(e) => onDragStart(e, 'pool|' + r.id)}
            style={{ cursor: 'grab' }}
          >
            <img className="thumb" src={r.photo} alt="" />
            <div className="body"><h3>{r.title}</h3><div className="muted">⭐ {r.avgRating || '—'} · {r.timeMin} мин</div></div>
          </div>
        ))}
      </div>
    </>
  );
}
