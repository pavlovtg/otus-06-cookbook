import { useEffect, useState } from 'react';
import { fakeApi } from '../mock/fakeApi';
import type { Category, RecipeWithMeta, Tag } from '../mock/types';
import { RecipeCard } from '../components/RecipeCard';

export function Favorites({ onOpen }: { onOpen?: (id: string) => void }) {
  const [items, setItems] = useState<RecipeWithMeta[] | null>(null);
  const [cats, setCats] = useState<Category[]>([]);
  const [tags, setTags] = useState<Tag[]>([]);
  useEffect(() => {
    fakeApi.getRecipes({ onlyFavorites: true, perPage: 50 }).then((r) => setItems(r.items));
    fakeApi.getCategories().then(setCats);
    fakeApi.getTags().then(setTags);
  }, []);
  const catMap = Object.fromEntries(cats.map((c) => [c.id, c.name]));
  const tagMap = Object.fromEntries(tags.map((t) => [t.id, t.name]));
  return (
    <>
      <h1>Избранное</h1>
      {!items ? <div className="muted">загрузка…</div>
        : items.length === 0 ? <div className="card">Пока пусто.</div>
        : (
          <div className="recipe-grid">
            {items.map((r) => <RecipeCard key={r.id} recipe={r} categoryNames={catMap} tagNames={tagMap} onOpen={onOpen} />)}
          </div>
        )}
    </>
  );
}
