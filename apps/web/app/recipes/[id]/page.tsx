export const dynamic = "force-dynamic";

import Image from "next/image";
import Link from "next/link";
import { notFound } from "next/navigation";
import { getRecipe } from "@/lib/bff/recipes";
import { getCategories } from "@/lib/bff/categories";
import { getSession } from "@/lib/session";
import { RecipePhoto } from "@/components/photo";
import { Tag } from "@/components/ui/Tag";
import { ArrowLeftIcon, ClockIcon, FlameIcon } from "@/components/icons";
import { DeleteRecipeButton } from "./DeleteRecipeButton";
import { RecipePhotoActions } from "./RecipePhotoActions";
import { getRecipePhotoUrl } from "@/lib/bff/photos";
import { IngredientsCard } from "@/components/features/IngredientsCard";

const DIFFICULTY_LABELS: Record<string, string> = {
  easy: "Просто",
  everyday: "На каждый день",
  festive: "Праздничное",
  restaurant: "Ресторанное",
  signature: "Авторское",
};

interface Props {
  params: Promise<{ id: string }>;
  searchParams: Promise<{ page?: string }>;
}

export default async function RecipeDetailPage({ params, searchParams }: Props) {
  const { id } = await params;
  const { page } = await searchParams;
  const backHref = page ? `/?page=${page}` : "/";

  let recipe;
  let allCategories;
  try {
    [recipe, allCategories] = await Promise.all([getRecipe(id), getCategories()]);
  } catch {
    notFound();
  }

  const session = await getSession();
  const isAuthenticated = !!session.user;

  const recipeCats = recipe.categoryIds
    .map((cid) => allCategories.find((c) => c.id === cid))
    .filter((c): c is NonNullable<typeof c> => c !== undefined);

  return (
    <>
      <div className="detail-bar">
        <Link href={backHref} className="btn btn-ghost btn-sm back-btn">
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
          {recipeCats.map((c) => <Tag key={c.id}>{c.name}</Tag>)}
        </div>
        {isAuthenticated && (
          <div className="detail-actions">
            <Link
              href={`/recipes/${recipe.id}/edit`}
              className="btn btn-ghost btn-sm"
            >
              Редактировать
            </Link>
            <DeleteRecipeButton id={recipe.id} />
          </div>
        )}
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
          <IngredientsCard
            ingredients={recipe.ingredients}
            baseServings={recipe.servings}
          />

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
