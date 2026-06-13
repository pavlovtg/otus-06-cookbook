import Link from "next/link";
import { notFound } from "next/navigation";
import { getRecipe } from "@/lib/bff/recipes";
import { RecipePhoto } from "@/components/photo";
import { Tag } from "@/components/ui/Tag";
import { ArrowLeftIcon, ClockIcon, FlameIcon } from "@/components/icons";
import { DeleteRecipeButton } from "./DeleteRecipeButton";

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
          <span><ClockIcon size={13} /> {recipe.cookingTime} мин</span>
          <span className="sep" />
          <span><FlameIcon size={13} /> {DIFFICULTY_LABELS[recipe.difficulty] ?? recipe.difficulty}</span>
        </div>
      </div>

      <div className="detail-toolbar">
        <div className="detail-tags">
          <Tag>{DIFFICULTY_LABELS[recipe.difficulty] ?? recipe.difficulty}</Tag>
          <Tag>{recipe.servings} порц.</Tag>
          <Tag>{recipe.cookingTime} мин</Tag>
        </div>
        <div className="detail-actions">
          <Link href={`/recipes/${recipe.id}/edit`} className="btn btn-ghost btn-sm">
            Редактировать
          </Link>
          <DeleteRecipeButton id={recipe.id} />
        </div>
      </div>

      <div className="detail-grid">
        <div>
          <div className="gallery">
            <div className="main-photo">
              <RecipePhoto seed={recipe.id} title={recipe.title} />
            </div>
          </div>

          <div style={{ marginTop: 24 }}>
            <h2 className="t-subheading" style={{ marginBottom: 12 }}>Описание</h2>
            <p className="t-body" style={{ color: "var(--fg-secondary)" }}>{recipe.description}</p>
          </div>

          <div style={{ marginTop: 24 }}>
            <h2 className="t-subheading" style={{ marginBottom: 12 }}>Инструкции</h2>
            <div className="instructions-text">{recipe.instructions}</div>
          </div>
        </div>

        <div className="card card-pad-lg" style={{ display: "flex", flexDirection: "column", gap: 16 }}>
          <h2 className="t-subheading">Информация</h2>
          <div style={{ display: "flex", flexDirection: "column", gap: 8 }}>
            <div className="ingredient-row">
              <span className="name">Время приготовления</span>
              <span className="amount">{recipe.cookingTime} мин</span>
            </div>
            <div className="ingredient-row">
              <span className="name">Сложность</span>
              <span className="amount">{DIFFICULTY_LABELS[recipe.difficulty] ?? recipe.difficulty}</span>
            </div>
            <div className="ingredient-row">
              <span className="name">Порций</span>
              <span className="amount">{recipe.servings}</span>
            </div>
          </div>
        </div>
      </div>
    </>
  );
}
