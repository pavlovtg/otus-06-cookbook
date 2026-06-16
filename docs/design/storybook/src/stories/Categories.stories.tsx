import type { Meta, StoryObj } from '@storybook/react';
import * as React from 'react';

const meta: Meta = { title: 'Domain/Categories', parameters: { layout: 'padded' } };
export default meta;
type S = StoryObj;

/* ---- seed data ------------------------------------------ */
const CATEGORY_TYPES = {
  meal_role: 'Роль в приёме пищи',
  cooking_method: 'Способ приготовления',
  main_ingredient: 'Основной ингредиент',
  cuisine: 'Национальная кухня',
  meal_time: 'Время употребления',
  dietary: 'Диетические особенности',
  serving_form: 'Форма подачи',
} as const;

type CategoryType = keyof typeof CATEGORY_TYPES;

interface Category {
  id: string;
  name: string;
  description?: string | null;
  type: CategoryType;
}

const SEED: Category[] = [
  { id: 'c1', name: 'Закуски холодные', type: 'meal_role' },
  { id: 'c2', name: 'Первые блюда', type: 'meal_role' },
  { id: 'c3', name: 'Вторые блюда', type: 'meal_role' },
  { id: 'c4', name: 'Десерты', type: 'meal_role' },
  { id: 'c5', name: 'Варёные', type: 'cooking_method' },
  { id: 'c6', name: 'Жареные', type: 'cooking_method' },
  { id: 'c7', name: 'Запечённые', type: 'cooking_method' },
  { id: 'c8', name: 'Мясные', type: 'main_ingredient' },
  { id: 'c9', name: 'Рыбные', type: 'main_ingredient' },
  { id: 'c10', name: 'Овощные', type: 'main_ingredient' },
  { id: 'c11', name: 'Итальянская', type: 'cuisine' },
  { id: 'c12', name: 'Русская', type: 'cuisine' },
  { id: 'c13', name: 'Японская', type: 'cuisine' },
  { id: 'c14', name: 'Завтраки', type: 'meal_time' },
  { id: 'c15', name: 'Обеды', type: 'meal_time' },
  { id: 'c16', name: 'Ужины', type: 'meal_time' },
  { id: 'c17', name: 'Вегетарианские', type: 'dietary' },
  { id: 'c18', name: 'Веганские', type: 'dietary' },
  { id: 'c19', name: 'Супы', type: 'serving_form' },
  { id: 'c20', name: 'Пиццы', type: 'serving_form' },
];

/* ---- CategoryTag ---------------------------------------- */
function CategoryTag({
  cat,
  onEdit,
  onDelete,
}: {
  cat: Category;
  onEdit?: (c: Category) => void;
  onDelete?: (c: Category) => void;
}) {
  return (
    <span className="tag" style={{ fontSize: 13, padding: '6px 12px' }}>
      <span>{cat.name}</span>
      {onEdit && (
        <button
          className="btn-icon"
          style={{ width: 20, height: 20, background: 'transparent', boxShadow: 'none' }}
          onClick={() => onEdit(cat)}
          title="Редактировать"
        >
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.7" strokeLinecap="round">
            <path d="M4 20h4l10-10-4-4L4 16zM14 6l4 4" />
          </svg>
        </button>
      )}
      {onDelete && (
        <button
          className="btn-icon"
          style={{ width: 20, height: 20, background: 'transparent', boxShadow: 'none' }}
          onClick={() => onDelete(cat)}
          title="Удалить"
        >
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.8" strokeLinecap="round">
            <path d="m6 6 12 12M6 18 18 6" />
          </svg>
        </button>
      )}
    </span>
  );
}

/* ---- CategoryGroup -------------------------------------- */
function CategoryGroup({
  type,
  label,
  categories,
  onEdit,
  onDelete,
}: {
  type: CategoryType;
  label: string;
  categories: Category[];
  onEdit?: (c: Category) => void;
  onDelete?: (c: Category) => void;
}) {
  return (
    <div className="card card-pad-lg" style={{ marginBottom: 16 }}>
      <h3 className="t-subheading" style={{ marginBottom: 14 }}>{label}</h3>
      <div style={{ display: 'flex', flexWrap: 'wrap', gap: 8 }}>
        {categories.length ? (
          categories.map((c) => (
            <CategoryTag key={c.id} cat={c} onEdit={onEdit} onDelete={onDelete} />
          ))
        ) : (
          <p className="t-small">Категорий нет.</p>
        )}
      </div>
    </div>
  );
}

/* ---- CategoryForm --------------------------------------- */
function CategoryForm({
  category,
  onSave,
  onCancel,
}: {
  category?: Category;
  onSave: (data: { name: string; description: string; type: CategoryType }) => void;
  onCancel: () => void;
}) {
  const [name, setName] = React.useState(category?.name ?? '');
  const [description, setDescription] = React.useState(category?.description ?? '');
  const [type, setType] = React.useState<CategoryType>(category?.type ?? 'meal_role');

  return (
    <form
      className="modal-body"
      onSubmit={(e) => { e.preventDefault(); onSave({ name, description, type }); }}
    >
      <div className="field">
        <label>Название</label>
        <input className="input" value={name} onChange={(e) => setName(e.target.value)} maxLength={200} required />
      </div>
      <div className="field">
        <label>Тип</label>
        <select
          className="select"
          value={type}
          onChange={(e) => setType(e.target.value as CategoryType)}
          disabled={!!category}
        >
          {(Object.keys(CATEGORY_TYPES) as CategoryType[]).map((t) => (
            <option key={t} value={t}>{CATEGORY_TYPES[t]}</option>
          ))}
        </select>
      </div>
      <div className="field">
        <label>Описание</label>
        <textarea className="textarea" value={description} onChange={(e) => setDescription(e.target.value)} maxLength={2000} />
      </div>
      <div className="form-actions">
        <button type="button" className="btn btn-ghost" onClick={onCancel}>Отмена</button>
        <button type="submit" className="btn btn-primary">{category ? 'Сохранить' : 'Создать'}</button>
      </div>
    </form>
  );
}

/* ---- Stories -------------------------------------------- */

/** Страница категорий — все 7 групп, только просмотр */
export const Page: S = {
  render: () => {
    const grouped = (Object.keys(CATEGORY_TYPES) as CategoryType[]).map((type) => ({
      type,
      label: CATEGORY_TYPES[type],
      categories: SEED.filter((c) => c.type === type),
    }));
    return (
      <div style={{ maxWidth: 900 }}>
        <div className="page-heading">
          <h1>Категории рецептов</h1>
          <button className="btn btn-primary btn-sm">+ Новая категория</button>
        </div>
        {grouped.map(({ type, label, categories }) => (
          <CategoryGroup key={type} type={type} label={label} categories={categories} />
        ))}
      </div>
    );
  },
};

/** Playground — интерактивное добавление/удаление/редактирование ★ */
export const Playground: S = {
  render: () => {
    const [categories, setCategories] = React.useState<Category[]>(SEED);
    const [modal, setModal] = React.useState<{ mode: 'create' | 'edit' | 'delete'; cat?: Category } | null>(null);

    const grouped = (Object.keys(CATEGORY_TYPES) as CategoryType[]).map((type) => ({
      type,
      label: CATEGORY_TYPES[type],
      categories: categories.filter((c) => c.type === type),
    }));

    function handleSave(data: { name: string; description: string; type: CategoryType }) {
      if (modal?.mode === 'edit' && modal.cat) {
        setCategories((prev) =>
          prev.map((c) => c.id === modal.cat!.id ? { ...c, name: data.name, description: data.description } : c)
        );
      } else {
        setCategories((prev) => [
          ...prev,
          { id: 'c' + Date.now(), name: data.name, description: data.description, type: data.type },
        ]);
      }
      setModal(null);
    }

    function handleDelete() {
      if (modal?.cat) {
        setCategories((prev) => prev.filter((c) => c.id !== modal.cat!.id));
      }
      setModal(null);
    }

    return (
      <div style={{ maxWidth: 900 }}>
        <div className="page-heading">
          <h1>Категории рецептов</h1>
          <button className="btn btn-primary btn-sm" onClick={() => setModal({ mode: 'create' })}>
            + Новая категория
          </button>
        </div>

        {grouped.map(({ type, label, categories: cats }) => (
          <CategoryGroup
            key={type}
            type={type}
            label={label}
            categories={cats}
            onEdit={(c) => setModal({ mode: 'edit', cat: c })}
            onDelete={(c) => setModal({ mode: 'delete', cat: c })}
          />
        ))}

        {modal && modal.mode !== 'delete' && (
          <div className="modal-backdrop is-open" onClick={() => setModal(null)}>
            <div className="modal" onClick={(e) => e.stopPropagation()}>
              <div className="modal-head">
                <h2>{modal.mode === 'edit' ? 'Редактировать категорию' : 'Новая категория'}</h2>
              </div>
              <CategoryForm
                category={modal.cat}
                onSave={handleSave}
                onCancel={() => setModal(null)}
              />
            </div>
          </div>
        )}

        {modal?.mode === 'delete' && modal.cat && (
          <div className="modal-backdrop is-open" onClick={() => setModal(null)}>
            <div className="modal" onClick={(e) => e.stopPropagation()}>
              <div className="modal-head">
                <h2>Удалить «{modal.cat.name}»?</h2>
              </div>
              <div className="modal-body">
                <p className="t-small">Удалить можно только если категория не используется в рецептах.</p>
                <div className="form-actions">
                  <button className="btn btn-ghost" onClick={() => setModal(null)}>Отмена</button>
                  <button className="btn btn-danger" onClick={handleDelete}>Удалить</button>
                </div>
              </div>
            </div>
          </div>
        )}
      </div>
    );
  },
};

/** Одна группа — пустое состояние */
export const EmptyGroup: S = {
  render: () => (
    <div style={{ maxWidth: 600 }}>
      <CategoryGroup type="dietary" label={CATEGORY_TYPES.dietary} categories={[]} />
    </div>
  ),
};

/** Одиночный тег с кнопками */
export const SingleTag: S = {
  render: () => (
    <div style={{ display: 'flex', gap: 8, padding: 24 }}>
      <CategoryTag
        cat={{ id: 'c1', name: 'Итальянская', type: 'cuisine' }}
        onEdit={() => alert('edit')}
        onDelete={() => alert('delete')}
      />
      <CategoryTag cat={{ id: 'c2', name: 'Только просмотр', type: 'cuisine' }} />
    </div>
  ),
};
