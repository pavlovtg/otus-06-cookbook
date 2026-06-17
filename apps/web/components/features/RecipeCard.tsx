import Image from "next/image";
import type { RecipeShortDto } from "@/lib/schemas/recipe";
import type { Category } from "@/lib/schemas/category";
import { ClockIcon, FlameIcon, UserIcon, LockIcon, StarIcon } from "@/components/icons";
import { RecipePhoto } from "@/components/photo";
import { Tag } from "@/components/ui/Tag";
import { getRecipeThumbnailUrl } from "@/lib/bff/photos";
import { FavoriteButton } from "@/components/features/FavoriteButton";
import { StarsRating } from "@/components/StarsRating";

const DIFFICULTY_LABELS: Record<string, string> = {
  easy: "Просто",
  everyday: "На каждый день",
  festive: "Праздничное",
  restaurant: "Ресторанное",
  signature: "Авторское",
};

interface RecipeCardProps {
  recipe: RecipeShortDto;
  categories?: Category[];
  showFavorite?: boolean;
  onClick?: () => void;
}

export function RecipeCard({ recipe, categories = [], showFavorite = false, onClick }: RecipeCardProps) {
  const recipeCats = recipe.categoryIds
    .slice(0, 3)
    .map((id) => categories.find((c) => c.id === id))
    .filter((c): c is Category => c !== undefined);

  return (
    <div className="card recipe-card card-link" onClick={onClick}>
      <div className="photo" style={{ position: "relative" }}>
        {recipe.photoId != null ? (
          <Image
            src={getRecipeThumbnailUrl(recipe.photoId)}
            alt={recipe.title}
            fill
            unoptimized
            style={{ objectFit: "cover" }}
          />
        ) : (
          <RecipePhoto seed={recipe.id} title={recipe.title} />
        )}
        {!recipe.isPublic && (
          <div className="photo-private" title="Приватный рецепт">
            <LockIcon size={16} className="tag-private" />
          </div>
        )}
        {showFavorite && (
          <FavoriteButton
            recipeId={recipe.id}
            isFavorite={recipe.isFavorite ?? false}
          />
        )}
      </div>
      <div className="body">
        <h3>{recipe.title}</h3>
        <div className="tags">
          {recipeCats.map((c) => <Tag key={c.id}>{c.name}</Tag>)}
        </div>
        <div className="meta">
          <span>
            <ClockIcon size={12} /> {recipe.cookingTime} мин
          </span>
          <span>
            <FlameIcon size={12} /> {DIFFICULTY_LABELS[recipe.difficulty] ?? recipe.difficulty}
          </span>
        </div>
        <p className="t-small" style={{ margin: 0, overflow: "hidden", display: "-webkit-box", WebkitLineClamp: 2, WebkitBoxOrient: "vertical" }}>
          {recipe.description}
        </p>
        <div className="footer">
          <span className="author-inline">
            <UserIcon size={12} />
            <span>{recipe.authorName ?? "—"}</span>
          </span>
          <span className="rating-inline">
            {recipe.averageRating != null ? (
              <>
                <StarIcon size={12} />
                <span>{recipe.averageRating.toFixed(1)}</span>
              </>
            ) : null}
            {recipe.myRating != null ? (
              <span className="my-rating-badge">Моя: {recipe.myRating}★</span>
            ) : null}
          </span>
        </div>
      </div>
    </div>
  );
}
