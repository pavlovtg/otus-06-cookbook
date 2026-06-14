import type { Meta, StoryObj } from '@storybook/react';
import * as React from 'react';
import { RecipeCard } from '../domain/RecipeCard';
import { recipes, getUser, avgRating, favorites } from '../mocks';

const meta: Meta<typeof RecipeCard> = {
  title: 'Domain/RecipeCard',
  component: RecipeCard,
  parameters: { layout: 'centered' },
};
export default meta;
type S = StoryObj<typeof RecipeCard>;

const r = recipes[0];

export const Default: S = {
  args: {
    recipe: r,
    author: getUser(r.author_id),
    rating: avgRating(r.id),
    myRating: 5,
    favorite: true,
  },
};

export const WithPhoto: S = {
  name: 'С фото',
  args: {
    recipe: { ...r, photo_url: 'https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=400&h=300&fit=crop' },
    author: getUser(r.author_id),
    rating: avgRating(r.id),
  },
};

export const WithoutPhoto: S = {
  name: 'Без фото (заглушка)',
  args: {
    recipe: { ...r, photo_url: undefined },
    author: getUser(r.author_id),
    rating: avgRating(r.id),
  },
};

export const Private: S = {
  args: {
    recipe: recipes.find((x) => !x.is_public)!,
    author: getUser('u1'),
    rating: 5,
  },
};

export const Guest: S = {
  args: { recipe: r, author: getUser(r.author_id), rating: avgRating(r.id), showFavorite: false },
};

export const Playground: S = {
  render: () => {
    const [favs, setFavs] = React.useState(new Set(favorites.filter((f) => f.user_id === 'u1').map((f) => f.recipe_id)));
    return (
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: 20, maxWidth: 1100 }}>
        {recipes.map((rec) => (
          <RecipeCard
            key={rec.id}
            recipe={rec}
            author={getUser(rec.author_id)}
            rating={avgRating(rec.id)}
            favorite={favs.has(rec.id)}
            onToggleFavorite={() =>
              setFavs((prev) => {
                const n = new Set(prev);
                n.has(rec.id) ? n.delete(rec.id) : n.add(rec.id);
                return n;
              })
            }
            onClick={() => alert(`Open ${rec.title}`)}
          />
        ))}
      </div>
    );
  },
};
