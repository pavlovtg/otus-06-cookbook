import { z } from "zod";

export const DifficultySchema = z.enum([
  "easy",
  "everyday",
  "festive",
  "restaurant",
  "signature",
]);

export type Difficulty = z.infer<typeof DifficultySchema>;

export const RecipeDtoSchema = z.object({
  id: z.string().uuid(),
  title: z.string(),
  description: z.string(),
  cookingTime: z.number().int().positive(),
  difficulty: DifficultySchema,
  servings: z.number().int().positive(),
  instructions: z.string(),
});

export type RecipeDto = z.infer<typeof RecipeDtoSchema>;

export const RecipeListSchema = z.array(RecipeDtoSchema);

export const RecipeRequestSchema = z.object({
  title: z.string().min(1).max(200),
  description: z.string().min(1).max(2000),
  cookingTime: z.number().int().positive(),
  difficulty: DifficultySchema,
  servings: z.number().int().positive(),
  instructions: z.string().min(1).max(10000),
});

export type RecipeRequest = z.infer<typeof RecipeRequestSchema>;
