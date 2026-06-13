import type { Meta, StoryObj } from '@storybook/react';
import * as React from 'react';
import { StarsRating } from '../domain/StarsRating';
import { ServingsControl } from '../domain/ServingsControl';
import { IngredientList } from '../domain/IngredientList';
import { CommentItem } from '../domain/CommentItem';
import { recipes, comments, getRecipe } from '../mocks';

const meta: Meta = { title: 'Domain/Recipe', parameters: { layout: 'centered' } };
export default meta;
type S = StoryObj;

export const Stars: S = { render: () => <StarsRating defaultValue={4} /> };
export const StarsReadOnly: S = { render: () => <StarsRating value={3} readOnly /> };
export const Servings: S = { render: () => <ServingsControl defaultValue={4} /> };

export const Ingredients: S = {
  render: () => (
    <div style={{ maxWidth: 420 }}>
      <IngredientList recipe={recipes[0]} servings={4} />
    </div>
  ),
};

export const IngredientsScaled: S = {
  render: () => {
    const r = recipes[0];
    const [s, setS] = React.useState(r.servings);
    return (
      <div className="card card-pad-lg" style={{ maxWidth: 480 }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 14 }}>
          <h3 className="t-subheading">Ингредиенты</h3>
          <ServingsControl defaultValue={r.servings} onChange={setS} />
        </div>
        <IngredientList recipe={r} servings={s} />
        <p className="t-micro" style={{ marginTop: 10 }}>на {s} порц.</p>
      </div>
    );
  },
};

export const Comment: S = {
  render: () => (
    <div style={{ maxWidth: 520 }}>
      <CommentItem comment={comments[0]} />
    </div>
  ),
};

export const CommentsList: S = {
  render: () => {
    const [items, setItems] = React.useState(comments);
    return (
      <div style={{ maxWidth: 560, display: 'flex', flexDirection: 'column', gap: 14 }}>
        {items.map((c) => (
          <CommentItem key={c.id} comment={c} canDelete onDelete={() => setItems((x) => x.filter((i) => i.id !== c.id))} />
        ))}
      </div>
    );
  },
};

export const Playground: S = {
  render: () => {
    const r = getRecipe('r1')!;
    const [s, setS] = React.useState(r.servings);
    const [rating, setRating] = React.useState(0);
    return (
      <div className="card card-pad-lg" style={{ width: 520 }}>
        <h3 className="t-subheading" style={{ marginBottom: 14 }}>{r.title}</h3>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 14 }}>
          <span className="t-small">Порции</span>
          <ServingsControl defaultValue={r.servings} onChange={setS} />
        </div>
        <IngredientList recipe={r} servings={s} />
        <div style={{ marginTop: 18 }}>
          <h3 className="t-subheading" style={{ marginBottom: 10 }}>Ваш рейтинг</h3>
          <div className="rating-widget">
            <StarsRating value={rating} onChange={setRating} />
            <span className="t-small">{rating ? `Вы поставили ${rating}/5` : 'Поставьте оценку'}</span>
          </div>
        </div>
      </div>
    );
  },
};
