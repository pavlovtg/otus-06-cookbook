export const dynamic = "force-dynamic";

import logger from "@/lib/logger";
import { getCategories } from "@/lib/bff/categories";
import {
  CategoryType,
  CategoryTypeLabels,
  type Category,
} from "@/lib/schemas/category";
import { CategoryModal } from "./CategoryModal";
import { DeleteCategoryButton } from "./DeleteCategoryButton";

export default async function CategoriesPage() {
  let categories: Category[] = [];
  let fetchError: string | null = null;

  try {
    categories = await getCategories();
  } catch (err) {
    logger.error({ err }, "Failed to load categories");
    fetchError = "Не удалось загрузить список категорий.";
  }

  const grouped = Object.fromEntries(
    CategoryType.options.map((type) => [
      type,
      categories.filter((c) => c.type === type),
    ]),
  );

  return (
    <>
      <div className="page-heading">
        <h1>Категории рецептов</h1>
        <CategoryModal
          trigger={
            <button
              className="btn btn-primary btn-sm"
              data-testid="create-category-trigger"
            >
              + Новая категория
            </button>
          }
        />
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
      ) : (
        CategoryType.options.map((type) => (
          <div
            key={type}
            className="card card-pad-lg"
            style={{ marginBottom: 16 }}
            data-testid={`category-group-${type}`}
          >
            <h3 className="t-subheading" style={{ marginBottom: 14 }}>
              {CategoryTypeLabels[type]}
            </h3>
            <div style={{ display: "flex", flexWrap: "wrap", gap: 8 }}>
              {grouped[type]?.length ? (
                grouped[type].map((cat) => (
                  <span
                    key={cat.id}
                    className="tag"
                    style={{ fontSize: 13, padding: "6px 12px" }}
                    data-testid="category-tag"
                  >
                    <span>{cat.name}</span>
                    <CategoryModal
                      category={cat}
                      trigger={
                        <button
                          className="btn-icon"
                          style={{
                            width: 20,
                            height: 20,
                            background: "transparent",
                            boxShadow: "none",
                          }}
                          data-testid="edit-category-trigger"
                          title="Редактировать"
                        >
                          <svg
                            viewBox="0 0 24 24"
                            fill="none"
                            stroke="currentColor"
                            strokeWidth="1.7"
                            strokeLinecap="round"
                          >
                            <path d="M4 20h4l10-10-4-4L4 16zM14 6l4 4" />
                          </svg>
                        </button>
                      }
                    />
                    <DeleteCategoryButton id={cat.id} name={cat.name} />
                  </span>
                ))
              ) : (
                <p className="t-small">Категорий нет.</p>
              )}
            </div>
          </div>
        ))
      )}
    </>
  );
}
