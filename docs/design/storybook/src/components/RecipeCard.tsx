import type { RecipeWithMeta } from '../mock/types';
import { Badge } from './Badge';

export type RecipeCardProps = {
  recipe: RecipeWithMeta;
  categoryNames?: Record<string, string>;
  tagNames?: Record<string, string>;
  authorName?: string;
  onOpen?: (id: string) => void;
};

export function RecipeCard({ recipe, categoryNames = {}, tagNames = {}, authorName, onOpen }: RecipeCardProps) {
  return (
    /* style: card */
    <article className="recipe-card">
      <img className="thumb" src={recipe.photo} alt={recipe.title} />
      <div className="body">
        <h3>{recipe.title}</h3>
        <div className="muted">
          {authorName ? 'Автор: ' + authorName + ' · ' : ''}
          ⭐ {recipe.avgRating || '—'} · {recipe.timeMin} мин · {recipe.difficulty}
        </div>
        <div className="row" style={{ gap: 4 }}>
          {recipe.categories.slice(0, 2).map((c) => <Badge key={c} variant="cat">{categoryNames[c] || c}</Badge>)}
          {recipe.tags.slice(0, 3).map((t) => <Badge key={t}>#{tagNames[t] || t}</Badge>)}
          {!recipe.isPublic && <Badge variant="priv">приватный</Badge>}
        </div>
        <div style={{ marginTop: 'auto' }}>
          <button className="btn primary" onClick={() => onOpen?.(recipe.id)}>Открыть</button>
        </div>
      </div>
    </article>
  );
}
