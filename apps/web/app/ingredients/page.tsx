import { getIngredients } from "@/lib/bff/ingredients";
import {
  IngredientCategory,
  IngredientCategoryLabels,
  type Ingredient,
} from "@/lib/schemas/ingredient";
import { IngredientModal } from "./IngredientModal";
import { DeleteIngredientButton } from "./DeleteIngredientButton";

interface Props {
  searchParams: Promise<{ title?: string; category?: string }>;
}

export default async function IngredientsPage({ searchParams }: Props) {
  const { title, category } = await searchParams;

  const categoryValue = IngredientCategory.safeParse(category).success
    ? (category as (typeof IngredientCategory.options)[number])
    : undefined;

  let ingredients: Ingredient[] = [];
  let fetchError: string | null = null;

  try {
    ingredients = await getIngredients({
      title: title || undefined,
      category: categoryValue,
    });
  } catch (err) {
    fetchError =
      err instanceof Error ? err.message : "Не удалось загрузить ингредиенты.";
  }

  return (
    <>
      <div className="page-heading">
        <h1>Ингредиенты</h1>
        <IngredientModal
          trigger={
            <button
              className="btn btn-primary btn-sm"
              data-testid="create-ingredient-trigger"
            >
              + Новый ингредиент
            </button>
          }
        />
      </div>

      <div className="toolbar">
        <form style={{ display: "contents" }}>
          <div className="search-wrap">
            <input
              className="search-input"
              name="title"
              defaultValue={title ?? ""}
              placeholder="Найти ингредиент…"
              data-testid="filter-title"
            />
          </div>
          <select
            className="select"
            name="category"
            defaultValue={category ?? ""}
            style={{ maxWidth: 240 }}
            data-testid="filter-category"
          >
            <option value="">Все категории</option>
            {IngredientCategory.options.map((cat) => (
              <option key={cat} value={cat}>
                {IngredientCategoryLabels[cat]}
              </option>
            ))}
          </select>
          <button type="submit" className="btn btn-ghost btn-sm">
            Найти
          </button>
        </form>
      </div>

      {fetchError ? (
        <div className="state">
          <div
            className="state-eyebrow"
            style={{
              color: "var(--danger)",
              boxShadow: "inset 0 0 0 1px rgba(244,114,114,0.3)",
            }}
          >
            Ошибка загрузки
          </div>
          <p className="t-small" style={{ maxWidth: 400 }}>
            {fetchError}
          </p>
        </div>
      ) : ingredients.length === 0 ? (
        <div className="state">
          <div className="state-eyebrow">Не найдено</div>
          <p className="t-small">Попробуйте изменить фильтры.</p>
        </div>
      ) : (
        <div className="ingredients-list">
          {ingredients.map((ingredient) => (
            <div
              key={ingredient.id}
              className="ingredient-item ingredient-row"
              data-category={ingredient.category}
            >
              <div>
                <span
                  className="ingredient-title name"
                  style={{ color: "var(--fg-primary)" }}
                >
                  {ingredient.title}
                </span>
                <span
                  className="t-micro"
                  style={{ display: "block", marginTop: 2 }}
                >
                  {IngredientCategoryLabels[ingredient.category]} ·{" "}
                  {ingredient.defaultAmount} {ingredient.unit}
                  {ingredient.isSystem && " · системный"}
                </span>
              </div>
              <div style={{ display: "flex", gap: 8, alignItems: "center" }}>
                <IngredientModal
                  ingredient={ingredient}
                  trigger={
                    <button
                      className="btn btn-ghost btn-sm"
                      data-testid="edit-ingredient-trigger"
                    >
                      Редактировать
                    </button>
                  }
                />
                <DeleteIngredientButton
                  id={ingredient.id}
                  title={ingredient.title}
                />
              </div>
            </div>
          ))}
        </div>
      )}
    </>
  );
}
