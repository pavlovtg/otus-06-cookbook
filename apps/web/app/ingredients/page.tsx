export const dynamic = "force-dynamic";

import logger from "@/lib/logger";
import { getIngredients } from "@/lib/bff/ingredients.server";
import {
  IngredientCategory,
  IngredientCategoryLabels,
  type Ingredient,
  type PagedIngredient,
} from "@/lib/schemas/ingredient";
import { IngredientModal } from "./IngredientModal";
import { DeleteIngredientButton } from "./DeleteIngredientButton";

interface Props {
  searchParams: Promise<{ title?: string; category?: string; page?: string }>;
}

function Paginator({
  page,
  total,
  pageSize,
  searchParams,
}: {
  page: number;
  total: number;
  pageSize: number;
  searchParams: { title?: string; category?: string };
}) {
  const totalPages = Math.ceil(total / pageSize);
  if (totalPages <= 1) return null;

  const buildHref = (p: number) => {
    const params = new URLSearchParams();
    if (searchParams.title) params.set("title", searchParams.title);
    if (searchParams.category) params.set("category", searchParams.category);
    params.set("page", String(p));
    return `/ingredients?${params.toString()}`;
  };

  return (
    <nav className="pagination" aria-label="Пагинация" data-testid="paginator">
      {page > 1 && (
        <a href={buildHref(page - 1)} className="page-btn" aria-label="Предыдущая страница">
          ‹
        </a>
      )}
      {Array.from({ length: totalPages }, (_, i) => i + 1).map((p) => (
        <a
          key={p}
          href={buildHref(p)}
          className={`page-btn${p === page ? " is-active" : ""}`}
          aria-current={p === page ? "page" : undefined}
          data-testid={`paginator-page-${p}`}
        >
          {p}
        </a>
      ))}
      {page < totalPages && (
        <a href={buildHref(page + 1)} className="page-btn" aria-label="Следующая страница">
          ›
        </a>
      )}
    </nav>
  );
}

export default async function IngredientsPage({ searchParams }: Props) {
  const { title, category, page: pageParam } = await searchParams;

  const page = Math.max(1, parseInt(pageParam ?? "1", 10) || 1);

  const categoryValue = IngredientCategory.safeParse(category).success
    ? (category as (typeof IngredientCategory.options)[number])
    : undefined;

  let result: PagedIngredient | null = null;
  let fetchError: string | null = null;

  try {
    result = await getIngredients({
      title: title || undefined,
      category: categoryValue,
      page,
    });
  } catch (err) {
    logger.error({ err }, "Failed to load ingredients");
    fetchError = "Не удалось загрузить список ингредиентов.";
  }

  const ingredients: Ingredient[] = result?.items ?? [];

  const groups = IngredientCategory.options
    .map((cat) => ({
      category: cat,
      items: ingredients
        .filter((i) => i.category === cat)
        .sort((a, b) => a.title.localeCompare(b.title, "ru")),
    }))
    .filter((g) => g.items.length > 0);

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
              maxLength={200}
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
        <>
          <div className="shopping-table ingredients-list">
            {groups.map((group) => (
              <div key={group.category}>
                <div className="shopping-group-head">
                  {IngredientCategoryLabels[group.category]}
                </div>
                {group.items.map((ingredient) => (
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
            ))}
          </div>

          {result && (
            <Paginator
              page={result.page}
              total={result.total}
              pageSize={result.pageSize}
              searchParams={{ title, category }}
            />
          )}
        </>
      )}
    </>
  );
}
