/* Minimal seed data ported from docs/design/mockup/index.html */

export type User = { id: string; display_name: string; email: string; role: 'user' | 'admin' };
export type Category = { id: string; name: string; type: CategoryType };
export type CategoryType =
  | 'meal_role'
  | 'cooking_method'
  | 'main_ingredient'
  | 'cuisine'
  | 'meal_time'
  | 'dietary'
  | 'serving_form';
export type Ingredient = {
  id: string;
  title: string;
  unit: string;
  default_amount: number;
  category: string;
  is_system: boolean;
  author_id: string | null;
};
export type RecipeIngredient = { ingredient_id: string; amount: number };
export type Difficulty = 'easy' | 'everyday' | 'festive' | 'restaurant' | 'signature';
export type Recipe = {
  id: string;
  title: string;
  description: string;
  cooking_time: number;
  difficulty: Difficulty;
  servings: number;
  ingredients: RecipeIngredient[];
  instructions: string;
  categories: Category[];
  author_id: string;
  is_public: boolean;
  photo_url?: string;
};
export type Comment = {
  id: string;
  recipe_id: string;
  author_id: string;
  text: string;
  created_at: string;
};

export const CATEGORY_TYPE_LABELS: Record<CategoryType, string> = {
  meal_role: 'Роль в приёме пищи',
  cooking_method: 'Способ приготовления',
  main_ingredient: 'Основной ингредиент',
  cuisine: 'Национальная кухня',
  meal_time: 'Время употребления',
  dietary: 'Диетические особенности',
  serving_form: 'Форма подачи',
};

export const DIFFICULTY_LABELS: Record<Difficulty, string> = {
  easy: 'Простой',
  everyday: 'Повседневный',
  festive: 'Праздничный',
  restaurant: 'Ресторанный',
  signature: 'Авторский',
};

export const DAY_LABELS = ['Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб', 'Вс'];
export const MEAL_KEYS = ['breakfast', 'lunch', 'dinner'] as const;
export type MealKey = (typeof MEAL_KEYS)[number];
export const MEAL_LABELS: Record<MealKey, string> = {
  breakfast: 'Завтрак',
  lunch: 'Обед',
  dinner: 'Ужин',
};

export const users: User[] = [
  { id: 'u1', display_name: 'Анна Воронова', email: 'user1@cookbook.test', role: 'user' },
  { id: 'u2', display_name: 'Кирилл Соколов', email: 'user2@cookbook.test', role: 'user' },
  { id: 'u3', display_name: 'Админ', email: 'admin@cookbook.test', role: 'admin' },
];

export const categories: Category[] = [
  { id: 'c1', name: 'Вторые блюда', type: 'meal_role' },
  { id: 'c2', name: 'Первые блюда', type: 'meal_role' },
  { id: 'c3', name: 'Салаты', type: 'meal_role' },
  { id: 'c4', name: 'Десерты', type: 'meal_role' },
  { id: 'c5', name: 'Завтраки', type: 'meal_time' },
  { id: 'c6', name: 'Ужины', type: 'meal_time' },
  { id: 'c7', name: 'Итальянская', type: 'cuisine' },
  { id: 'c8', name: 'Русская', type: 'cuisine' },
  { id: 'c9', name: 'Мясные', type: 'main_ingredient' },
  { id: 'c10', name: 'Овощные', type: 'main_ingredient' },
  { id: 'c11', name: 'Вегетарианские', type: 'dietary' },
  { id: 'c12', name: 'Запечённые', type: 'cooking_method' },
  { id: 'c13', name: 'Паста', type: 'serving_form' },
  { id: 'c14', name: 'Супы', type: 'serving_form' },
];

const c = (id: string) => categories.find((x) => x.id === id)!;

export const ingredients: Ingredient[] = [
  { id: 'i1', title: 'Спагетти', unit: 'г', default_amount: 200, category: 'Крупы и зерновые', is_system: true, author_id: null },
  { id: 'i2', title: 'Бекон', unit: 'г', default_amount: 150, category: 'Мясо и птица', is_system: true, author_id: null },
  { id: 'i3', title: 'Яйцо куриное', unit: 'шт', default_amount: 2, category: 'Молочные продукты и яйца', is_system: true, author_id: null },
  { id: 'i4', title: 'Сыр пармезан', unit: 'г', default_amount: 60, category: 'Молочные продукты и яйца', is_system: true, author_id: null },
  { id: 'i5', title: 'Чёрный перец', unit: 'г', default_amount: 2, category: 'Специи и приправы', is_system: true, author_id: null },
  { id: 'i6', title: 'Соль', unit: 'г', default_amount: 5, category: 'Специи и приправы', is_system: true, author_id: null },
  { id: 'i7', title: 'Помидор', unit: 'шт', default_amount: 2, category: 'Овощи', is_system: true, author_id: null },
  { id: 'i8', title: 'Лук репчатый', unit: 'шт', default_amount: 1, category: 'Овощи', is_system: true, author_id: null },
  { id: 'i9', title: 'Картофель', unit: 'шт', default_amount: 1, category: 'Овощи', is_system: true, author_id: null },
  { id: 'i10', title: 'Морковь', unit: 'шт', default_amount: 1, category: 'Овощи', is_system: true, author_id: null },
  { id: 'i11', title: 'Куриная грудка', unit: 'г', default_amount: 500, category: 'Мясо и птица', is_system: true, author_id: null },
  { id: 'i12', title: 'Оливковое масло', unit: 'мл', default_amount: 30, category: 'Масла и жиры', is_system: true, author_id: null },
  { id: 'i13', title: 'Мука пшеничная', unit: 'г', default_amount: 300, category: 'Крупы и зерновые', is_system: true, author_id: null },
  { id: 'i14', title: 'Сыр моцарелла', unit: 'г', default_amount: 150, category: 'Молочные продукты и яйца', is_system: true, author_id: null },
  { id: 'i15', title: 'Базилик', unit: 'г', default_amount: 20, category: 'Овощи', is_system: true, author_id: null },
  { id: 'i16', title: 'Шоколад тёмный', unit: 'г', default_amount: 100, category: 'Выпечка и сладости', is_system: true, author_id: null },
  { id: 'i17', title: 'Сахар', unit: 'г', default_amount: 50, category: 'Выпечка и сладости', is_system: true, author_id: null },
  { id: 'i18', title: 'Сливочное масло', unit: 'г', default_amount: 50, category: 'Молочные продукты и яйца', is_system: true, author_id: null },
];

const ri = (id: string, amount: number): RecipeIngredient => ({ ingredient_id: id, amount });

export const recipes: Recipe[] = [
  {
    id: 'r1',
    title: 'Карбонара',
    description: 'Классическая римская паста с гуанчиале и желтком',
    cooking_time: 20,
    difficulty: 'everyday',
    servings: 4,
    ingredients: [ri('i1', 200), ri('i2', 150), ri('i3', 2), ri('i4', 60), ri('i5', 2), ri('i6', 5)],
    instructions:
      'Спагетти отварить в подсолёной воде до al dente.\n\nБекон обжарить до хруста.\n\nЯйца взбить с пармезаном и перцем.\n\nСмешать горячие спагетти с беконом, снять с огня и влить смесь.',
    categories: [c('c1'), c('c7'), c('c9'), c('c13'), c('c6')],
    author_id: 'u1',
    is_public: true,
  },
  {
    id: 'r2',
    title: 'Шакшука',
    description: 'Яйца, тушёные в томатном соусе с перцем',
    cooking_time: 25,
    difficulty: 'easy',
    servings: 2,
    ingredients: [ri('i7', 2), ri('i8', 1), ri('i3', 2), ri('i12', 30), ri('i6', 5)],
    instructions:
      'Обжарить лук и перец.\n\nДобавить помидоры и паприку, тушить 10 минут.\n\nСделать углубления, разбить яйца, накрыть крышкой.',
    categories: [c('c5'), c('c10'), c('c11')],
    author_id: 'u2',
    is_public: true,
  },
  {
    id: 'r3',
    title: 'Маргарита',
    description: 'Тонкая пицца с моцареллой и базиликом',
    cooking_time: 40,
    difficulty: 'festive',
    servings: 2,
    ingredients: [ri('i13', 250), ri('i7', 2), ri('i14', 150), ri('i15', 20), ri('i12', 20), ri('i6', 5)],
    instructions:
      'Замесить тесто и дать подойти 30 минут.\n\nРаскатать, выложить помидоры и моцареллу.\n\nЗапекать 8 минут на максимальной температуре.\n\nПосыпать базиликом.',
    categories: [c('c1'), c('c12'), c('c10'), c('c7'), c('c6'), c('c11')],
    author_id: 'u2',
    is_public: true,
  },
  {
    id: 'r4',
    title: 'Цезарь с курицей',
    description: 'Салат с гриль-курицей и пармезаном',
    cooking_time: 30,
    difficulty: 'everyday',
    servings: 2,
    ingredients: [ri('i11', 300), ri('i4', 50), ri('i12', 20), ri('i6', 5), ri('i5', 2)],
    instructions:
      'Куриную грудку обжарить с обеих сторон.\n\nНарезать листовой салат и смешать с заправкой.\n\nДобавить курицу, пармезан и сухарики.',
    categories: [c('c3'), c('c9')],
    author_id: 'u1',
    is_public: true,
  },
  {
    id: 'r5',
    title: 'Борщ',
    description: 'Свекольный суп с томатной пастой и сметаной',
    cooking_time: 90,
    difficulty: 'everyday',
    servings: 6,
    ingredients: [ri('i9', 2), ri('i8', 1), ri('i10', 1), ri('i6', 5)],
    instructions: 'Сварить мясной бульон.\n\nСпассеровать овощи.\n\nДобавить капусту, варить 20 минут.',
    categories: [c('c2'), c('c8'), c('c14')],
    author_id: 'u2',
    is_public: true,
  },
  {
    id: 'r6',
    title: 'Шоколадный фондан',
    description: 'Десерт с жидким центром',
    cooking_time: 25,
    difficulty: 'restaurant',
    servings: 4,
    ingredients: [ri('i16', 200), ri('i18', 100), ri('i3', 3), ri('i17', 80), ri('i13', 40)],
    instructions:
      'Растопить шоколад с маслом.\n\nВзбить яйца с сахаром.\n\nСоединить, добавить муку.\n\nВыпекать 10 минут при 200°C.',
    categories: [c('c4'), c('c12'), c('c11')],
    author_id: 'u1',
    is_public: false,
  },
];

export const comments: Comment[] = [
  {
    id: 'cm1',
    recipe_id: 'r1',
    author_id: 'u2',
    text: 'Готовил по этому рецепту вчера — семья в восторге!',
    created_at: '2026-06-09T14:20:00Z',
  },
  {
    id: 'cm2',
    recipe_id: 'r1',
    author_id: 'u3',
    text: 'Очень вкусно, но я бы добавил чуть больше специй.',
    created_at: '2026-06-08T10:15:00Z',
  },
  {
    id: 'cm3',
    recipe_id: 'r1',
    author_id: 'u1',
    text: 'Идеально для воскресного ужина.',
    created_at: '2026-06-05T19:00:00Z',
  },
];

export const ratings = [
  { recipe_id: 'r1', user_id: 'u1', value: 5 },
  { recipe_id: 'r1', user_id: 'u2', value: 4 },
  { recipe_id: 'r1', user_id: 'u3', value: 5 },
  { recipe_id: 'r2', user_id: 'u1', value: 4 },
  { recipe_id: 'r3', user_id: 'u1', value: 5 },
  { recipe_id: 'r3', user_id: 'u2', value: 5 },
  { recipe_id: 'r4', user_id: 'u2', value: 4 },
  { recipe_id: 'r5', user_id: 'u3', value: 5 },
  { recipe_id: 'r6', user_id: 'u1', value: 5 },
];

export const favorites = [
  { user_id: 'u1', recipe_id: 'r1' },
  { user_id: 'u1', recipe_id: 'r3' },
  { user_id: 'u1', recipe_id: 'r5' },
];

export function avgRating(recipeId: string): number {
  const arr = ratings.filter((r) => r.recipe_id === recipeId);
  if (!arr.length) return 0;
  return arr.reduce((s, r) => s + r.value, 0) / arr.length;
}

export function getIngredient(id: string): Ingredient | undefined {
  return ingredients.find((i) => i.id === id);
}

export function getUser(id: string): User | undefined {
  return users.find((u) => u.id === id);
}

export function getRecipe(id: string): Recipe | undefined {
  return recipes.find((r) => r.id === id);
}
