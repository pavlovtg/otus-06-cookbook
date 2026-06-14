export const dynamic = "force-dynamic";

import Image from "next/image";
import Link from "next/link";
import { notFound } from "next/navigation";
import { getRecipe } from "@/lib/bff/recipes";
import { RecipePhoto } from "@/components/photo";
import { Tag } from "@/components/ui/Tag";
import { ArrowLeftIcon, ClockIcon, FlameIcon } from "@/components/icons";
import { DeleteRecipeButton } from "./DeleteRecipeButton";
import { RecipePhotoActions } from "./RecipePhotoActions";
import { getRecipePhotoUrl } from "@/lib/bff/photos";

const DIFFICULTY_LABELS: Record<string, string> = {
  easy: "Просто",
  everyday: "На каждый день",
  festive: "Праздничное",
  restaurant: "Ресторанное",
  signature: "Авторское",
};

interface Props {
  params: Promise<{ id: string }>;
}

export default async function RecipeDetailPage({ params }: Props) {
  const { id } = await params;

  let recipe;
  try {
    recipe = await getRecipe(id);
  } catch {
    notFound();
  }

  return (
    <>
      <div className="detail-bar">
        <Link href="/" className="btn btn-ghost btn-sm back-btn">
          <ArrowLeftIcon size={14} /> Назад
        </Link>
        <span className="title">{recipe.title}</span>
        <span className="spacer" />
        <div className="meta">
          <span>
            <ClockIcon size={13} /> {recipe.cookingTime} мин
          </span>
          <span className="sep" />
          <span>
            <FlameIcon size={13} />{" "}
            {DIFFICULTY_LABELS[recipe.difficulty] ?? recipe.difficulty}
          </span>
        </div>
      </div>

      <div className="detail-toolbar">
        <div className="detail-tags">
          <Tag>{DIFFICULTY_LABELS[recipe.difficulty] ?? recipe.difficulty}</Tag>
          <Tag>{recipe.servings} порц.</Tag>
          <Tag>{recipe.cookingTime} мин</Tag>
        </div>
        <div className="detail-actions">
          <Link
            href={`/recipes/${recipe.id}/edit`}
            className="btn btn-ghost btn-sm"
          >
            Редактировать
          </Link>
          <DeleteRecipeButton id={recipe.id} />
        </div>
      </div>

      <div className="detail-grid">
        {/* Left column */}
        <div className="gallery">
          <div className="main-photo" style={{ position: "relative" }}>
            {recipe.photoId != null ? (
              <Image
                src={getRecipePhotoUrl(recipe.photoId)}
                alt={recipe.title}
                fill
                unoptimized
                style={{ objectFit: "cover" }}
              />
            ) : (
              <RecipePhoto seed={recipe.id} title={recipe.title} />
            )}
          </div>

          <RecipePhotoActions recipeId={recipe.id} photoId={recipe.photoId} />

          <p
            className="t-small"
            style={{ color: "var(--fg-secondary)", fontSize: 15, marginTop: 6 }}
          >
            {recipe.description}
          </p>

          <h3 className="t-subheading" style={{ marginTop: 14 }}>
            Шаги приготовления
          </h3>
          <div className="instructions-text">{recipe.instructions}</div>
        </div>

        {/* Right column */}
        <div style={{ display: "flex", flexDirection: "column", gap: 24 }}>
          {/* Ingredients card */}
          <div className="card card-pad-lg">
            <div
              style={{
                display: "flex",
                justifyContent: "space-between",
                alignItems: "center",
                marginBottom: 14,
              }}
            >
              <h3 className="t-subheading">Ингредиенты</h3>
              <div className="servings-control">
                <span
                  className="value"
                  style={{ padding: "0 8px", fontVariantNumeric: "tabular-nums" }}
                >
                  {recipe.servings} порц.
                </span>
              </div>
            </div>

            {recipe.ingredients.length === 0 ? (
              <p className="t-small">Ингредиенты не указаны</p>
            ) : (
              <div className="ingredients-list">
                {recipe.ingredients.map((ing) => (
                  <div key={ing.ingredientId} className="ingredient-row">
                    <span className="name">{ing.title}</span>
                    <span className="amount">
                      {ing.amount} {ing.unit}
                    </span>
                  </div>
                ))}
              </div>
            )}
          </div>

          {/* Info card */}
          <div className="card card-pad-lg">
            <h3 className="t-subheading" style={{ marginBottom: 12 }}>
              Информация
            </h3>
            <div style={{ display: "flex", flexDirection: "column", gap: 4 }}>
              <div className="ingredient-row">
                <span className="name">Время приготовления</span>
                <span className="amount">{recipe.cookingTime} мин</span>
              </div>
              <div className="ingredient-row">
                <span className="name">Сложность</span>
                <span className="amount">
                  {DIFFICULTY_LABELS[recipe.difficulty] ?? recipe.difficulty}
                </span>
              </div>
              <div className="ingredient-row">
                <span className="name">Порций</span>
                <span className="amount">{recipe.servings}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </>
  );
}
