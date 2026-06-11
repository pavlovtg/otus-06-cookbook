import * as React from 'react';
import { ClockIcon, FlameIcon, UserIcon, StarIcon, HeartIcon, HeartFillIcon, LockIcon } from '../icons';
import { RecipePhoto } from '../photo';
import { Tag } from '../components/Tag';
import { DIFFICULTY_LABELS, type Recipe, type User, type Difficulty } from '../mocks';

export interface RecipeCardProps {
  recipe: Recipe;
  author?: User;
  rating?: number;
  myRating?: number;
  favorite?: boolean;
  showFavorite?: boolean;
  onClick?: () => void;
  onToggleFavorite?: () => void;
}

export function RecipeCard({
  recipe: r,
  author,
  rating = 0,
  myRating = 0,
  favorite = false,
  showFavorite = true,
  onClick,
  onToggleFavorite,
}: RecipeCardProps) {
  return (
    <div className="card recipe-card card-link" onClick={onClick}>
      <div className="photo">
        <RecipePhoto seed={r.id} title={r.title} />
        {!r.is_public && (
          <div className="photo-private" title="Приватный рецепт">
            <LockIcon size={16} />
          </div>
        )}
        {showFavorite && (
          <button
            className={['btn-icon', 'photo-fav', favorite ? 'is-on' : ''].filter(Boolean).join(' ')}
            onClick={(e) => {
              e.stopPropagation();
              onToggleFavorite?.();
            }}
            aria-label="Избранное"
          >
            {favorite ? <HeartFillIcon size={16} /> : <HeartIcon size={16} />}
          </button>
        )}
      </div>
      <div className="body">
        <h3>{r.title}</h3>
        <div className="tags">
          {r.categories.slice(0, 3).map((c) => (
            <Tag key={c.id}>{c.name}</Tag>
          ))}
        </div>
        <div className="meta">
          <span>
            <ClockIcon size={12} /> {r.cooking_time} мин
          </span>
          <span>
            <FlameIcon size={12} /> {DIFFICULTY_LABELS[r.difficulty as Difficulty]}
          </span>
        </div>
        <div className="footer">
          <span className="rating-inline">
            <StarIcon size={14} />
            <span>{rating ? rating.toFixed(1) : '—'}</span>
            {myRating ? <span className="my">· вы: {myRating}</span> : null}
          </span>
          <span className="author-inline">
            <UserIcon size={12} />
            <span>{author?.display_name ?? '—'}</span>
          </span>
        </div>
      </div>
    </div>
  );
}
