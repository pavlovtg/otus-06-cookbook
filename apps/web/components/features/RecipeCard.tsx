import Image from "next/image";
import type { RecipeShortDto } from "@/lib/schemas/recipe";
import type { Category } from "@/lib/schemas/category";
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
  categories?: Category[];
  onClick?: () => void;
}

export function RecipeCard({ recipe, categories = [], onClick }: RecipeCardProps) {
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
      </div>
      <div className="body">
        <h3>{recipe.title}</h3>
        <div className="tags">
          {recipeCats.length > 0 ? (
            recipeCats.map((c) => <Tag key={c.id}>{c.name}</Tag>)
          ) : (
            <Tag>{DIFFICULTY_LABELS[recipe.difficulty] ?? recipe.difficulty}</Tag>
          )}
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
