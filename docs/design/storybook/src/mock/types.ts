export type User = { id: string; email: string; password: string; name: string };
export type Category = { id: string; name: string; slug: string; description?: string };
export type Tag = { id: string; name: string };
export type Unit = 'г' | 'шт' | 'мл';
export type Ingredient = { id: string; name: string; unit: Unit };
export type RecipeIngredient = { ingredientId: string; qty: number };
export type Difficulty = 'лёгкая' | 'средняя' | 'сложная';

export type Recipe = {
  id: string;
  title: string;
  description: string;
  timeMin: number;
  difficulty: Difficulty;
  servings: number;
  photo: string;
  steps: string[];
  ingredients: RecipeIngredient[];
  categories: string[];
  tags: string[];
  authorId: string;
  isPublic: boolean;
  ratings: Record<string, number>;
  createdAt: number;
};

export type RecipeWithMeta = Recipe & { avgRating: number };

export type Comment = {
  id: string;
  recipeId: string;
  userId: string;
  text: string;
  createdAt: number;
  author?: string;
};

export type MealPlan = Record<number, Record<'breakfast' | 'lunch' | 'dinner', string[]>>;

export type ShoppingItem = { name: string; unit: Unit; qty: number };

export type DashboardStats = {
  byCategory: { label: string; value: number }[];
  topRated: { id: string; title: string; avg: number }[];
  planFilled: number;
  planTotal: number;
  totals: { recipes: number; users: number; comments: number };
};
