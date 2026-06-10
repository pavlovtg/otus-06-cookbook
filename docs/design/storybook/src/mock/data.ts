import { placeholderSVG } from './placeholder';
import type {
  Category, Comment, Difficulty, Ingredient, MealPlan, Recipe, Tag, Unit, User,
} from './types';

export const SEED_USERS: User[] = [
  { id: 'u1', email: 'anna@test.ru', password: 'pass1234', name: 'Анна' },
  { id: 'u2', email: 'boris@test.ru', password: 'pass1234', name: 'Борис' },
];

export const SEED_CATEGORIES: Category[] = [
  { id: 'c1', name: 'Завтрак', slug: 'breakfast', description: 'Утренние блюда' },
  { id: 'c2', name: 'Обед', slug: 'lunch' },
  { id: 'c3', name: 'Ужин', slug: 'dinner' },
  { id: 'c4', name: 'Десерт', slug: 'dessert' },
  { id: 'c5', name: 'Салат', slug: 'salad' },
  { id: 'c6', name: 'Суп', slug: 'soup' },
  { id: 'c7', name: 'Выпечка', slug: 'bakery' },
  { id: 'c8', name: 'Напитки', slug: 'drinks' },
];

export const SEED_TAGS: Tag[] = [
  { id: 't1', name: 'веган' }, { id: 't2', name: 'быстрое' }, { id: 't3', name: 'без глютена' },
  { id: 't4', name: 'для детей' }, { id: 't5', name: 'праздничное' }, { id: 't6', name: 'дешёвое' },
  { id: 't7', name: 'острое' }, { id: 't8', name: 'диетическое' }, { id: 't9', name: 'мясное' },
  { id: 't10', name: 'рыба' }, { id: 't11', name: 'итальянское' }, { id: 't12', name: 'русское' },
];

const INGRED_NAMES: [string, Unit][] = [
  ['Курица филе', 'г'], ['Говядина', 'г'], ['Свинина', 'г'], ['Лосось', 'г'], ['Тунец консервированный', 'г'],
  ['Яйцо', 'шт'], ['Молоко', 'мл'], ['Сливки 20%', 'мл'], ['Сметана', 'г'], ['Кефир', 'мл'],
  ['Творог', 'г'], ['Сыр твёрдый', 'г'], ['Сыр моцарелла', 'г'], ['Сливочное масло', 'г'], ['Растительное масло', 'мл'],
  ['Мука пшеничная', 'г'], ['Сахар', 'г'], ['Соль', 'г'], ['Перец чёрный', 'г'], ['Дрожжи сухие', 'г'],
  ['Разрыхлитель', 'г'], ['Ванилин', 'г'], ['Какао', 'г'], ['Шоколад тёмный', 'г'], ['Мёд', 'г'],
  ['Рис', 'г'], ['Гречка', 'г'], ['Овсянка', 'г'], ['Макароны', 'г'], ['Картофель', 'г'],
  ['Лук', 'г'], ['Чеснок', 'г'], ['Морковь', 'г'], ['Помидор', 'г'], ['Огурец', 'г'],
  ['Перец болгарский', 'г'], ['Капуста', 'г'], ['Брокколи', 'г'], ['Цветная капуста', 'г'], ['Шпинат', 'г'],
  ['Яблоко', 'г'], ['Банан', 'шт'], ['Лимон', 'шт'], ['Апельсин', 'шт'], ['Клубника', 'г'],
  ['Базилик', 'г'], ['Укроп', 'г'], ['Петрушка', 'г'], ['Соевый соус', 'мл'], ['Томатная паста', 'г'],
  ['Оливки', 'г'], ['Грибы шампиньоны', 'г'], ['Креветки', 'г'], ['Кабачок', 'г'], ['Баклажан', 'г'],
];
export const SEED_INGREDIENTS: Ingredient[] = INGRED_NAMES.map(([name, unit], i) => ({ id: 'i' + (i + 1), name, unit }));

const RECIPE_TITLES = [
  'Паста Карбонара', 'Борщ классический', 'Цезарь с курицей', 'Овсянка с бананом', 'Шакшука',
  'Том-ям с креветками', 'Греческий салат', 'Картофельное пюре', 'Сырники', 'Плов с курицей',
  'Лазанья болоньезе', 'Тыквенный суп-пюре', 'Глазированные сырки', 'Оладьи на кефире', 'Рататуй',
  'Стейк рибай', 'Запечённый лосось', 'Брускетта с томатами', 'Шарлотка с яблоками', 'Чизкейк ванильный',
  'Гречка с грибами', 'Куриный суп', 'Цыплёнок табака', 'Рагу овощное', 'Хумус с лавашом',
  'Пицца Маргарита', 'Брауни шоколадный',
];
const STEPS_POOL = [
  'Нарежьте овощи кубиками.', 'Разогрейте сковороду на среднем огне.', 'Добавьте масло и обжарьте лук.',
  'Посолите и поперчите по вкусу.', 'Тушите под крышкой 15 минут.', 'Взбейте яйца с молоком.',
  'Разогрейте духовку до 180°C.', 'Выложите смесь в форму.', 'Запекайте 25–30 минут.',
  'Украсьте зеленью перед подачей.', 'Доведите воду до кипения.', 'Варите на медленном огне 20 минут.',
];

// deterministic pseudo-random so stories rebuild stable
let _s = 1;
function rand() { _s = (_s * 9301 + 49297) % 233280; return _s / 233280; }
function resetRand() { _s = 1; }
function pick<T>(arr: T[], n: number): T[] {
  const c = [...arr]; const r: T[] = [];
  while (n-- > 0 && c.length) r.push(c.splice(Math.floor(rand() * c.length), 1)[0]!);
  return r;
}

export function buildSeedRecipes(): Recipe[] {
  resetRand();
  const out: Recipe[] = [];
  RECIPE_TITLES.forEach((title, idx) => {
    const id = 'r' + (idx + 1);
    const ings = pick(SEED_INGREDIENTS, 4 + Math.floor(rand() * 5)).map((ing) => ({
      ingredientId: ing.id,
      qty: Math.round((ing.unit === 'шт' ? 1 + rand() * 3 : 50 + rand() * 250) * 10) / 10,
    }));
    const cats = pick(SEED_CATEGORIES, 1 + Math.floor(rand() * 2)).map((c) => c.id);
    const tags = pick(SEED_TAGS, 1 + Math.floor(rand() * 3)).map((t) => t.id);
    const steps = pick(STEPS_POOL, 4 + Math.floor(rand() * 3));
    const r: Recipe = {
      id, title,
      description: 'Простой и понятный рецепт «' + title + '». Подходит для домашнего ужина.',
      timeMin: 10 + Math.floor(rand() * 70),
      difficulty: (['лёгкая', 'средняя', 'сложная'] as Difficulty[])[idx % 3]!,
      servings: 2 + (idx % 4),
      photo: placeholderSVG(title),
      steps, ingredients: ings, categories: cats, tags,
      authorId: idx % 2 === 0 ? 'u1' : 'u2',
      isPublic: idx % 7 !== 0,
      ratings: { u1: 3 + ((idx * 7) % 3), u2: 3 + ((idx * 5) % 3) },
      createdAt: Date.now() - idx * 86400000,
    };
    out.push(r);
  });
  return out;
}

export function buildSeedComments(recipes: Recipe[]): Comment[] {
  const texts = [
    'Очень вкусно, спасибо!', 'Готовила вчера — семья в восторге.', 'Слишком солёно на мой вкус.',
    'Добавила больше чеснока — отлично!', 'Попробую завтра.', 'Идеально на ужин.',
    'Не получилось тесто. Что я сделал не так?', 'Великолепно!', 'Можно ли заменить молоко на сливки?',
    'Шикарный рецепт.', 'Дети попросили добавки.', 'Слишком долго готовить.',
    'Сделала половину порций — всё ок.', 'Очень оригинально.', 'Любимый рецепт уже год.',
    'Простая и быстрая еда.', 'Получилось пресно.', 'Чуть передержал в духовке.',
    'Отличный вариант для воскресного обеда.', 'Спасибо автору!', 'Лучший борщ в моей жизни.',
    'Картинка лучше реальности :)', 'Попробую с другими специями.',
  ];
  return texts.map((text, i) => ({
    id: 'cm' + (i + 1),
    recipeId: recipes[i % recipes.length]!.id,
    userId: i % 2 === 0 ? 'u1' : 'u2',
    text, createdAt: Date.now() - i * 3600000,
  }));
}

export function buildSeedMealPlan(recipes: Recipe[]): MealPlan {
  const slots = ['breakfast', 'lunch', 'dinner'] as const;
  const plan = {} as MealPlan;
  for (let d = 0; d < 7; d++) {
    plan[d] = { breakfast: [], lunch: [], dinner: [] };
    slots.forEach((s, si) => {
      plan[d]![s] = (d + s.length) % 4 === 0 ? [] : [recipes[(d * 3 + si) % recipes.length]!.id];
    });
  }
  return plan;
}
