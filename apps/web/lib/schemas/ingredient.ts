import { z } from "zod";

// Порядок совпадает с алфавитной сортировкой строк в БД (category хранится как snake_case строка)
export const IngredientCategory = z.enum([
  "bakery_and_sweets",
  "dairy_and_eggs",
  "fish_and_seafood",
  "fruits_and_berries",
  "grains_and_cereals",
  "legumes",
  "meat_and_poultry",
  "nuts_and_seeds",
  "oils_and_fats",
  "other",
  "sauces_and_pastes",
  "spices_and_seasonings",
  "vegetables",
]);

export type IngredientCategory = z.infer<typeof IngredientCategory>;

export const IngredientCategoryLabels: Record<IngredientCategory, string> = {
  bakery_and_sweets: "Выпечка и сладости",
  dairy_and_eggs: "Молочные продукты и яйца",
  fish_and_seafood: "Рыба и морепродукты",
  fruits_and_berries: "Фрукты и ягоды",
  grains_and_cereals: "Крупы и злаки",
  legumes: "Бобовые",
  meat_and_poultry: "Мясо и птица",
  nuts_and_seeds: "Орехи и семена",
  oils_and_fats: "Масла и жиры",
  other: "Прочее",
  sauces_and_pastes: "Соусы и пасты",
  spices_and_seasonings: "Специи и приправы",
  vegetables: "Овощи",
};

export const IngredientSchema = z.object({
  id: z.string().uuid(),
  title: z.string(),
  unit: z.string(),
  defaultAmount: z.number(),
  category: IngredientCategory,
  isSystem: z.boolean(),
});

export type Ingredient = z.infer<typeof IngredientSchema>;

export const IngredientRequestSchema = z.object({
  title: z.string().min(2).max(200),
  unit: z.string().min(1).max(20),
  defaultAmount: z.number().min(0.001).max(100000),
  category: IngredientCategory,
  isSystem: z.boolean().optional(),
});

export type IngredientRequest = z.infer<typeof IngredientRequestSchema>;

export function pagedIngredientSchema() {
  return z.object({
    items: z.array(IngredientSchema),
    total: z.number().int().nonnegative(),
    page: z.number().int().positive(),
    pageSize: z.number().int().positive(),
  });
}

export const PagedIngredientSchema = pagedIngredientSchema();

export type PagedIngredient = z.infer<typeof PagedIngredientSchema>;
