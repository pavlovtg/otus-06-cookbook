import { z } from "zod";

export const RecipeDtoSchema = z.object({
  id: z.string().uuid(),
  title: z.string(),
  description: z.string(),
});

export type RecipeDto = z.infer<typeof RecipeDtoSchema>;

export const RecipeListSchema = z.array(RecipeDtoSchema);
