import {
  buildSeedComments, buildSeedMealPlan, buildSeedRecipes,
  SEED_CATEGORIES, SEED_INGREDIENTS, SEED_TAGS, SEED_USERS,
} from './data';
import type {
  Category, Comment, DashboardStats, Ingredient, MealPlan, Recipe, RecipeWithMeta,
  ShoppingItem, Tag, User,
} from './types';

type DB = {
  users: User[]; categories: Category[]; tags: Tag[]; ingredients: Ingredient[];
  recipes: Recipe[]; comments: Comment[];
  favorites: Record<string, string[]>;
  mealPlan: Record<string, MealPlan>;
};

function freshDB(): DB {
  const recipes = buildSeedRecipes();
  return {
    users: structuredClone(SEED_USERS),
    categories: structuredClone(SEED_CATEGORIES),
    tags: structuredClone(SEED_TAGS),
    ingredients: structuredClone(SEED_INGREDIENTS),
    recipes,
    comments: buildSeedComments(recipes),
    favorites: { u1: [recipes[0]!.id, recipes[3]!.id], u2: [recipes[1]!.id] },
    mealPlan: { u1: buildSeedMealPlan(recipes), u2: buildSeedMealPlan(recipes) },
  };
}

export const db: DB = freshDB();
export function resetDB() { Object.assign(db, freshDB()); }

let _currentUserId: string | null = 'u1';
export function setCurrentUser(id: string | null) { _currentUserId = id; }
export function currentUser(): User | null {
  return _currentUserId ? db.users.find((u) => u.id === _currentUserId) ?? null : null;
}

const delay = (ms = 0) => new Promise<void>((r) => setTimeout(r, ms));

export function avgRating(r: Recipe): number {
  const v = Object.values(r.ratings || {});
  return v.length ? Math.round((v.reduce((s, x) => s + x, 0) / v.length) * 10) / 10 : 0;
}

export const fakeApi = {
  async login({ email, password }: { email: string; password: string }) {
    await delay();
    const u = db.users.find((x) => x.email === email && x.password === password);
    if (!u) throw new Error('Неверный email или пароль');
    _currentUserId = u.id;
    return { user: { id: u.id, email: u.email, name: u.name } };
  },
  async logout() { _currentUserId = null; },

  async getRecipes(opts: {
    page?: number; perPage?: number; q?: string;
    ingredientIds?: string[]; categoryIds?: string[]; tagIds?: string[];
    sort?: 'new' | 'rating' | 'time'; onlyMine?: boolean; onlyFavorites?: boolean;
  } = {}) {
    await delay();
    const { page = 1, perPage = 9, q = '', ingredientIds = [], categoryIds = [], tagIds = [], sort = 'new', onlyMine, onlyFavorites } = opts;
    const me = currentUser();
    let list = db.recipes.filter((r) => r.isPublic || (me && r.authorId === me.id));
    if (onlyMine && me) list = list.filter((r) => r.authorId === me.id);
    if (onlyFavorites && me) {
      const fav = db.favorites[me.id] || [];
      list = list.filter((r) => fav.includes(r.id));
    }
    if (q) {
      const qq = q.toLowerCase();
      list = list.filter((r) => r.title.toLowerCase().includes(qq) || r.description.toLowerCase().includes(qq));
    }
    if (ingredientIds.length) list = list.filter((r) => ingredientIds.every((id) => r.ingredients.some((i) => i.ingredientId === id)));
    if (categoryIds.length) list = list.filter((r) => categoryIds.some((id) => r.categories.includes(id)));
    if (tagIds.length) list = list.filter((r) => tagIds.some((id) => r.tags.includes(id)));
    const withMeta: RecipeWithMeta[] = list.map((r) => ({ ...r, avgRating: avgRating(r) }));
    withMeta.sort((a, b) => {
      if (sort === 'rating') return b.avgRating - a.avgRating;
      if (sort === 'time') return a.timeMin - b.timeMin;
      return b.createdAt - a.createdAt;
    });
    const total = withMeta.length;
    const start = (page - 1) * perPage;
    return { items: withMeta.slice(start, start + perPage), total, page, perPage, pages: Math.max(1, Math.ceil(total / perPage)) };
  },
  async getRecipe(id: string): Promise<RecipeWithMeta> {
    await delay();
    const r = db.recipes.find((x) => x.id === id);
    if (!r) throw new Error('Рецепт не найден');
    return { ...r, avgRating: avgRating(r) };
  },
  async rateRecipe(id: string, stars: number) {
    await delay();
    const me = currentUser(); if (!me) throw new Error('Требуется вход');
    const r = db.recipes.find((x) => x.id === id); if (!r) throw new Error('Не найдено');
    r.ratings[me.id] = Math.max(1, Math.min(5, stars | 0));
    return { avg: avgRating(r) };
  },
  async toggleFavorite(id: string) {
    await delay();
    const me = currentUser(); if (!me) throw new Error('Требуется вход');
    db.favorites[me.id] = db.favorites[me.id] || [];
    const arr = db.favorites[me.id]!; const i = arr.indexOf(id);
    if (i >= 0) arr.splice(i, 1); else arr.push(id);
    return { favorite: i < 0 };
  },
  async getFavorites() {
    await delay();
    const me = currentUser(); return me ? db.favorites[me.id] || [] : [];
  },
  async getComments(recipeId: string): Promise<Comment[]> {
    await delay();
    return db.comments
      .filter((c) => c.recipeId === recipeId)
      .sort((a, b) => b.createdAt - a.createdAt)
      .map((c) => ({ ...c, author: db.users.find((u) => u.id === c.userId)?.name || '?' }));
  },
  async getMealPlan(): Promise<MealPlan> {
    await delay();
    const me = currentUser(); if (!me) throw new Error('Требуется вход');
    return db.mealPlan[me.id]!;
  },
  async updateMealPlan(plan: MealPlan) {
    await delay();
    const me = currentUser(); if (!me) throw new Error('Требуется вход');
    db.mealPlan[me.id] = plan; return plan;
  },
  async getShoppingList(): Promise<ShoppingItem[]> {
    await delay();
    const me = currentUser(); if (!me) throw new Error('Требуется вход');
    const plan = db.mealPlan[me.id] || ({} as MealPlan);
    const map = new Map<string, ShoppingItem>();
    for (const d of Object.keys(plan)) {
      const day = plan[Number(d)]!;
      for (const s of Object.keys(day) as ('breakfast' | 'lunch' | 'dinner')[]) {
        for (const rid of day[s]) {
          const r = db.recipes.find((x) => x.id === rid); if (!r) continue;
          for (const ing of r.ingredients) {
            const meta = db.ingredients.find((x) => x.id === ing.ingredientId); if (!meta) continue;
            const k = ing.ingredientId + '|' + meta.unit;
            const ex = map.get(k) || { name: meta.name, unit: meta.unit, qty: 0 };
            ex.qty += ing.qty; map.set(k, ex);
          }
        }
      }
    }
    return [...map.values()].sort((a, b) => a.name.localeCompare(b.name, 'ru'));
  },
  async getCategories() { await delay(); return [...db.categories]; },
  async getTags() { await delay(); return [...db.tags]; },
  async getIngredients() { await delay(); return [...db.ingredients]; },
  async getDashboardStats(): Promise<DashboardStats> {
    await delay();
    const byCategory = db.categories.map((c) => ({ label: c.name, value: db.recipes.filter((r) => r.categories.includes(c.id)).length }));
    const topRated = [...db.recipes]
      .map((r) => ({ id: r.id, title: r.title, avg: avgRating(r) }))
      .sort((a, b) => b.avg - a.avg)
      .slice(0, 5);
    const me = currentUser();
    let planFilled = 0;
    if (me) {
      const plan = db.mealPlan[me.id] || ({} as MealPlan);
      for (const d of Object.keys(plan)) {
        const day = plan[Number(d)]!;
        for (const s of Object.keys(day) as ('breakfast' | 'lunch' | 'dinner')[]) {
          if (day[s].length) planFilled++;
        }
      }
    }
    return { byCategory, topRated, planFilled, planTotal: 21, totals: { recipes: db.recipes.length, users: db.users.length, comments: db.comments.length } };
  },
};
