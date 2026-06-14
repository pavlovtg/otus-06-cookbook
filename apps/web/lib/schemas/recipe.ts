import { z } from "zod";

export const DifficultySchema = z.enum([
  "easy",
  "everyday",
  "festive",
  "restaurant",
  "signature",
]);

export type Difficulty = z.infer<typeof DifficultySchema>;

export const RecipeIngredientDtoSchema = z.object({
  ingredientId: z.string().uuid(),
  title: z.string(),
  amount: z.number().positive(),
  unit: z.string(),
});

export type RecipeIngredientDto = z.infer<typeof RecipeIngredientDtoSchema>;

export const RecipeIngredientRequestSchema = z.object({
  ingredientId: z.string().uuid(),
  amount: z.number().positive(),
});

export type RecipeIngredientRequest = z.infer<
  typeof RecipeIngredientRequestSchema
>;

export const RecipeShortDtoSchema = z.object({
  id: z.string().uuid(),
  title: z.string(),
  description: z.string(),
  cookingTime: z.number().int().positive(),
  difficulty: DifficultySchema,
});

export type RecipeShortDto = z.infer<typeof RecipeShortDtoSchema>;

export const RecipeDtoSchema = z.object({
  id: z.string().uuid(),
  title: z.string(),
  description: z.string(),
  cookingTime: z.number().int().positive(),
  difficulty: DifficultySchema,
  servings: z.number().int().positive(),
  instructions: z.string(),
  ingredients: z.array(RecipeIngredientDtoSchema),
});

export type RecipeDto = z.infer<typeof RecipeDtoSchema>;

export const RecipeListSchema = z.array(RecipeShortDtoSchema);

export const RecipeRequestSchema = z.object({
  title: z.string().min(1).max(200),
  description: z.string().min(1).max(2000),
  cookingTime: z.number().int().positive(),
  difficulty: DifficultySchema,
  servings: z.number().int().positive(),
  instructions: z.string().min(1).max(10000),
  ingredients: z.array(RecipeIngredientRequestSchema),
});

export type RecipeRequest = z.infer<typeof RecipeRequestSchema>;
