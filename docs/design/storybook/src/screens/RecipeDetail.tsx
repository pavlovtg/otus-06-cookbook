import { useEffect, useMemo, useState } from 'react';
import { fakeApi } from '../mock/fakeApi';
import type { Category, Comment, Ingredient, RecipeWithMeta, Tag } from '../mock/types';
import { Badge } from '../components/Badge';
import { RatingStars } from '../components/RatingStars';
import { Spinner } from '../components/Spinner';

export function RecipeDetail({ recipeId }: { recipeId: string }) {
  const [r, setR] = useState<RecipeWithMeta | null>(null);
  const [comments, setComments] = useState<Comment[]>([]);
  const [cats, setCats] = useState<Category[]>([]);
  const [tags, setTags] = useState<Tag[]>([]);
  const [ings, setIngs] = useState<Ingredient[]>([]);
  const [favs, setFavs] = useState<string[]>([]);
  const [servings, setServings] = useState(2);
  const [newComment, setNewComment] = useState('');

  useEffect(() => {
    fakeApi.getRecipe(recipeId).then((rec) => { setR(rec); setServings(rec.servings); });
    fakeApi.getComments(recipeId).then(setComments);
    fakeApi.getCategories().then(setCats);
    fakeApi.getTags().then(setTags);
    fakeApi.getIngredients().then(setIngs);
    fakeApi.getFavorites().then(setFavs);
  }, [recipeId]);

  const ingMap = useMemo(() => Object.fromEntries(ings.map((i) => [i.id, i])), [ings]);

  if (!r) return <Spinner />;

  const ratio = servings / (r.servings || 1);
  const fav = favs.includes(r.id);

  async function rate(v: number) {
    const { avg } = await fakeApi.rateRecipe(r!.id, v);
    setR({ ...r!, avgRating: avg });
  }
  async function toggleFav() {
    const { favorite } = await fakeApi.toggleFavorite(r!.id);
    setFavs((arr) => favorite ? [...arr, r!.id] : arr.filter((x) => x !== r!.id));
  }
  async function addComment() {
    if (newComment.trim().length < 2) return;
    const c: Comment = { id: 'cm' + Date.now(), recipeId: r!.id, userId: 'u1', text: newComment, createdAt: Date.now(), author: 'Анна' };
    setComments([c, ...comments]);
    setNewComment('');
  }

  return (
    /* style: detail-layout */
    <div className="detail">
      <div>
        <h1>{r.title}</h1>
        <img src={r.photo} alt="" style={{ borderRadius: 14, marginBottom: 12 }} />
        <p>{r.description}</p>
        <div className="row">
          <span className="muted">⏱ {r.timeMin} мин · {r.difficulty}</span>
          <RatingStars value={Math.round(r.avgRating)} onChange={rate} />
          <button className="btn" onClick={toggleFav}>{fav ? '★ В избранном' : '☆ В избранное'}</button>
        </div>
        <div className="row" style={{ gap: 4, marginTop: 8 }}>
          {r.categories.map((c) => <Badge key={c} variant="cat">{cats.find((x) => x.id === c)?.name || c}</Badge>)}
          {r.tags.map((t) => <Badge key={t}>#{tags.find((x) => x.id === t)?.name || t}</Badge>)}
        </div>

        <h2 style={{ marginTop: 24 }}>Шаги</h2>
        <ol>{r.steps.map((s, i) => <li key={i} style={{ marginBottom: 8 }}>{s}</li>)}</ol>
      </div>

      <aside>
        <div className="card">
          <h3>Ингредиенты</h3>
          <label>Порций
            <input type="number" min={1} max={20} value={servings} onChange={(e) => setServings(Math.max(1, Number(e.target.value) || 1))} />
          </label>
          <ul>
            {r.ingredients.map((i) => {
              const meta = ingMap[i.ingredientId];
              if (!meta) return null;
              return <li key={i.ingredientId}>{meta.name} — {Math.round(i.qty * ratio * 10) / 10} {meta.unit}</li>;
            })}
          </ul>
        </div>

        <div className="card" style={{ marginTop: 16 }}>
          <h3>Комментарии ({comments.length})</h3>
          {/* style: form */}
          <div className="form">
            <textarea value={newComment} onChange={(e) => setNewComment(e.target.value)} placeholder="Оставьте комментарий…" />
            <button className="btn primary" onClick={addComment}>Отправить</button>
          </div>
          <div>
            {comments.map((c) => (
              /* style: comment */
              <div className="comment" key={c.id}>
                <div className="meta"><span>{c.author}</span><span>{new Date(c.createdAt).toLocaleString('ru')}</span></div>
                <div>{c.text}</div>
              </div>
            ))}
          </div>
        </div>
      </aside>
    </div>
  );
}
