import { z } from "zod";

export const CategoryType = z.enum([
  "meal_role",
  "cooking_method",
  "main_ingredient",
  "cuisine",
  "meal_time",
  "dietary",
  "serving_form",
]);

export type CategoryType = z.infer<typeof CategoryType>;

export const CategoryTypeLabels: Record<CategoryType, string> = {
  meal_role: "Роль в приёме пищи",
  cooking_method: "Способ приготовления",
  main_ingredient: "Основной ингредиент",
  cuisine: "Национальная кухня",
  meal_time: "Время употребления",
  dietary: "Диетические особенности",
  serving_form: "Форма подачи",
};

export const CategorySchema = z.object({
  id: z.string().uuid(),
  name: z.string(),
  description: z.string().nullable().optional(),
  type: CategoryType,
});

export type Category = z.infer<typeof CategorySchema>;

export const CategoryRequestSchema = z.object({
  name: z.string().min(1).max(200),
  description: z.string().max(2000).nullable().optional(),
  type: CategoryType,
});

export type CategoryRequest = z.infer<typeof CategoryRequestSchema>;
