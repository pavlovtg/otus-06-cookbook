import { z } from "zod";

export const RecipeRankDtoSchema = z.object({
  id: z.string().uuid(),
  title: z.string(),
  averageRating: z.number(),
});

export type RecipeRankDto = z.infer<typeof RecipeRankDtoSchema>;

export const CategoryCountDtoSchema = z.object({
  categoryName: z.string(),
  recipeCount: z.number().int().nonnegative(),
});

export type CategoryCountDto = z.infer<typeof CategoryCountDtoSchema>;

export const UserRankDtoSchema = z.object({
  id: z.string().uuid(),
  displayName: z.string(),
  averageRating: z.number().nullable(),
  commentCount: z.number().int().nonnegative(),
});

export type UserRankDto = z.infer<typeof UserRankDtoSchema>;

export const DashboardStatsDtoSchema = z.object({
  totalRecipes: z.number().int().nonnegative(),
  myRecipes: z.number().int().nonnegative().nullable().optional(),
  myComments: z.number().int().nonnegative().nullable().optional(),
  top10ByRating: z.array(RecipeRankDtoSchema),
  topFavoritesByRating: z.array(RecipeRankDtoSchema),
  byMainIngredient: z.array(CategoryCountDtoSchema),
  byCuisine: z.array(CategoryCountDtoSchema),
  totalUsers: z.number().int().nonnegative().nullable().optional(),
  totalComments: z.number().int().nonnegative().nullable().optional(),
  topUsersByRating: z.array(UserRankDtoSchema).nullable().optional(),
  topUsersByComments: z.array(UserRankDtoSchema).nullable().optional(),
  planFill: z.record(z.string(), z.boolean()).nullable().optional(),
});

export type DashboardStatsDto = z.infer<typeof DashboardStatsDtoSchema>;
