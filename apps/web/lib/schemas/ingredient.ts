import { z } from "zod";

export const IngredientCategory = z.enum([
  "vegetables",
  "fruits_and_berries",
  "meat_and_poultry",
  "fish_and_seafood",
  "dairy_and_eggs",
  "grains_and_cereals",
  "legumes",
  "nuts_and_seeds",
  "oils_and_fats",
  "spices_and_seasonings",
  "sauces_and_pastes",
  "bakery_and_sweets",
  "other",
]);

export type IngredientCategory = z.infer<typeof IngredientCategory>;

export const IngredientCategoryLabels: Record<IngredientCategory, string> = {
  vegetables: "Овощи",
  fruits_and_berries: "Фрукты и ягоды",
  meat_and_poultry: "Мясо и птица",
  fish_and_seafood: "Рыба и морепродукты",
  dairy_and_eggs: "Молочные продукты и яйца",
  grains_and_cereals: "Крупы и злаки",
  legumes: "Бобовые",
  nuts_and_seeds: "Орехи и семена",
  oils_and_fats: "Масла и жиры",
  spices_and_seasonings: "Специи и приправы",
  sauces_and_pastes: "Соусы и пасты",
  bakery_and_sweets: "Выпечка и сладости",
  other: "Прочее",
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
});

export type IngredientRequest = z.infer<typeof IngredientRequestSchema>;
