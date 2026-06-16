import type { Meta, StoryObj } from '@storybook/react';
import * as React from 'react';
import { CategoryTagInput } from '../domain/CategoryTagInput';
import { categories } from '../mocks';

const meta: Meta = {
  title: 'Domain/CategoryTagInput',
  parameters: { layout: 'padded' },
};
export default meta;
type S = StoryObj;

/** Пустое состояние — ничего не выбрано */
export const Empty: S = {
  name: 'Пустое состояние',
  render: () => {
    const [value, setValue] = React.useState<string[]>([]);
    return (
      <div style={{ maxWidth: 480 }}>
        <div className="field">
          <label>Категории</label>
          <CategoryTagInput categories={categories} value={value} onChange={setValue} />
        </div>
        <p className="t-micro" style={{ marginTop: 8 }}>
          Выбрано: {value.length === 0 ? 'ничего' : value.join(', ')}
        </p>
      </div>
    );
  },
};

/** Добавление чипа — вводим текст, выбираем из списка */
export const AddChip: S = {
  name: 'Добавление чипа',
  render: () => {
    const [value, setValue] = React.useState<string[]>(['c1']);
    return (
      <div style={{ maxWidth: 480 }}>
        <div className="field">
          <label>Категории</label>
          <CategoryTagInput categories={categories} value={value} onChange={setValue} />
        </div>
        <p className="t-micro" style={{ marginTop: 8 }}>
          Введите «вар» или «жар» для поиска
        </p>
      </div>
    );
  },
};

/** Удаление чипа — нажмите × на теге */
export const RemoveChip: S = {
  name: 'Удаление чипа',
  render: () => {
    const [value, setValue] = React.useState<string[]>(['c1', 'c5', 'c8']);
    return (
      <div style={{ maxWidth: 480 }}>
        <div className="field">
          <label>Категории</label>
          <CategoryTagInput categories={categories} value={value} onChange={setValue} />
        </div>
        <p className="t-micro" style={{ marginTop: 8 }}>
          Нажмите × на теге, чтобы удалить
        </p>
      </div>
    );
  },
};

/** Замена при совпадении типа — один тип = одна категория */
export const ReplaceByType: S = {
  name: 'Замена при совпадении типа',
  render: () => {
    const [value, setValue] = React.useState<string[]>(['c5']); // «Варёные» (cooking_method)
    return (
      <div style={{ maxWidth: 480 }}>
        <div className="field">
          <label>Категории</label>
          <CategoryTagInput categories={categories} value={value} onChange={setValue} />
        </div>
        <p className="t-micro" style={{ marginTop: 8 }}>
          Сейчас выбрано «Варёные» (способ приготовления). Введите «жар» и выберите «Жареные» — тег заменится.
        </p>
      </div>
    );
  },
};

/** ★ Playground — полный интерактив */
export const Playground: S = {
  name: '★ Playground',
  render: () => {
    const [value, setValue] = React.useState<string[]>([]);
    const selected = value
      .map((id) => categories.find((c) => c.id === id))
      .filter(Boolean);

    return (
      <div style={{ maxWidth: 600, display: 'flex', flexDirection: 'column', gap: 20 }}>
        <div className="field">
          <label>Категории рецепта</label>
          <CategoryTagInput categories={categories} value={value} onChange={setValue} />
        </div>

        <div className="card card-pad-lg">
          <h3 className="t-subheading" style={{ marginBottom: 12 }}>Выбранные категории</h3>
          {selected.length === 0 ? (
            <p className="t-small">Ничего не выбрано</p>
          ) : (
            <div style={{ display: 'flex', flexWrap: 'wrap', gap: 6 }}>
              {selected.map((c) => c && (
                <span key={c.id} className="tag">{c.name}</span>
              ))}
            </div>
          )}
        </div>

        <button
          className="btn btn-ghost btn-sm"
          style={{ alignSelf: 'flex-start' }}
          onClick={() => setValue([])}
        >
          Сбросить
        </button>
      </div>
    );
  },
};
