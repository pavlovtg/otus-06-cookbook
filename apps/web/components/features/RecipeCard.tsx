import type { RecipeShortDto } from "@/lib/schemas/recipe";
import { ClockIcon, FlameIcon } from "@/components/icons";
import { RecipePhoto } from "@/components/photo";
import { Tag } from "@/components/ui/Tag";
import { getRecipeThumbnailUrl } from "@/lib/bff/photos";

const DIFFICULTY_LABELS: Record<string, string> = {
  easy: "Просто",
  everyday: "На каждый день",
  festive: "Праздничное",
  restaurant: "Ресторанное",
  signature: "Авторское",
};

interface RecipeCardProps {
  recipe: RecipeShortDto;
  onClick?: () => void;
}

export function RecipeCard({ recipe, onClick }: RecipeCardProps) {
  return (
    <div className="card recipe-card card-link" onClick={onClick}>
      <div className="photo">
        {recipe.photoId != null ? (
          <img
            src={getRecipeThumbnailUrl(recipe.photoId)}
            alt={recipe.title}
            style={{ width: "100%", height: "100%", objectFit: "cover" }}
          />
        ) : (
          <RecipePhoto seed={recipe.id} title={recipe.title} />
        )}
      </div>
      <div className="body">
        <h3>{recipe.title}</h3>
        <div className="tags">
          <Tag>{DIFFICULTY_LABELS[recipe.difficulty] ?? recipe.difficulty}</Tag>
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
      </div>
    </div>
  );
}
