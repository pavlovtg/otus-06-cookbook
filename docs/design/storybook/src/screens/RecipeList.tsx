import { useEffect, useMemo, useState } from 'react';
import { fakeApi } from '../mock/fakeApi';
import type { Category, RecipeWithMeta, Tag } from '../mock/types';
import { Badge } from '../components/Badge';
import { Pager } from '../components/Pager';
import { Spinner } from '../components/Spinner';
import { RecipeCard } from '../components/RecipeCard';

const PER = 6;

export function RecipeList({ onOpen }: { onOpen?: (id: string) => void }) {
  const [q, setQ] = useState('');
  const [ingQ, setIngQ] = useState('');
  const [catId, setCatId] = useState<string>('');
  const [tagId, setTagId] = useState<string>('');
  const [sort, setSort] = useState<'new' | 'rating' | 'time'>('new');
  const [page, setPage] = useState(1);
  const [data, setData] = useState<{ items: RecipeWithMeta[]; pages: number; total: number } | null>(null);
  const [loading, setLoading] = useState(false);
  const [cats, setCats] = useState<Category[]>([]);
  const [tags, setTags] = useState<Tag[]>([]);
  const [ings, setIngs] = useState<{ id: string; name: string }[]>([]);

  useEffect(() => {
    fakeApi.getCategories().then(setCats);
    fakeApi.getTags().then(setTags);
    fakeApi.getIngredients().then(setIngs);
  }, []);

  const ingIds = useMemo(() => {
    if (!ingQ.trim()) return [];
    const tokens = ingQ.toLowerCase().split(',').map((s) => s.trim()).filter(Boolean);
    return ings.filter((i) => tokens.some((t) => i.name.toLowerCase().includes(t))).map((i) => i.id);
  }, [ingQ, ings]);

  useEffect(() => {
    setLoading(true);
    fakeApi
      .getRecipes({ page, perPage: PER, q, ingredientIds: ingIds, categoryIds: catId ? [catId] : [], tagIds: tagId ? [tagId] : [], sort })
      .then((r) => setData({ items: r.items, pages: r.pages, total: r.total }))
      .finally(() => setLoading(false));
  }, [page, q, ingIds, catId, tagId, sort]);

  const catMap = Object.fromEntries(cats.map((c) => [c.id, c.name]));
  const tagMap = Object.fromEntries(tags.map((t) => [t.id, t.name]));

  return (
    <>
      <h1>Рецепты</h1>
      {/* style: form */}
      <div className="card row" style={{ alignItems: 'flex-end' }}>
        <label style={{ flex: 1, minWidth: 180 }}>Поиск
          <input value={q} onChange={(e) => { setQ(e.target.value); setPage(1); }} placeholder="название…" />
        </label>
        <label style={{ flex: 1, minWidth: 180 }}>По ингредиентам
          <input value={ingQ} onChange={(e) => { setIngQ(e.target.value); setPage(1); }} placeholder="курица, лук" />
        </label>
        <label>Категория
          <select value={catId} onChange={(e) => { setCatId(e.target.value); setPage(1); }}>
            <option value="">— все —</option>
            {cats.map((c) => <option key={c.id} value={c.id}>{c.name}</option>)}
          </select>
        </label>
        <label>Тег
          <select value={tagId} onChange={(e) => { setTagId(e.target.value); setPage(1); }}>
            <option value="">— все —</option>
            {tags.map((t) => <option key={t.id} value={t.id}>{t.name}</option>)}
          </select>
        </label>
        <label>Сортировка
          <select value={sort} onChange={(e) => setSort(e.target.value as 'new' | 'rating' | 'time')}>
            <option value="new">по дате</option>
            <option value="rating">по рейтингу</option>
            <option value="time">по времени</option>
          </select>
        </label>
      </div>

      <div className="muted" style={{ margin: '16px 0' }}>
        {loading ? <Spinner /> : `Найдено: ${data?.total ?? 0}`}
      </div>

      {/* style: recipe-grid */}
      <div className="recipe-grid">
        {(data?.items ?? []).map((r) => (
          <RecipeCard key={r.id} recipe={r} categoryNames={catMap} tagNames={tagMap} onOpen={onOpen} />
        ))}
      </div>

      {data && data.pages > 1 && <Pager page={page} pages={data.pages} onGo={setPage} />}

      {!loading && data && data.items.length === 0 && (
        <div className="card" style={{ marginTop: 16 }}>
          Ничего не найдено. <Badge variant="cat">подсказка</Badge> попробуйте сбросить фильтры.
        </div>
      )}
    </>
  );
}
