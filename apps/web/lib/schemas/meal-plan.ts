import { z } from "zod";

// ── Enums ────────────────────────────────────────────────────────────────────

export const WeekDaySchema = z.number().int().min(1).max(7);
export const MealTypeSchema = z.number().int().min(1).max(3);

// ── DTO (ответ от API) ────────────────────────────────────────────────────────

export const MealPlanItemDtoSchema = z.object({
  recipeId: z.string().uuid(),
  servings: z.number().int().min(1).max(99),
});

export type MealPlanItemDto = z.infer<typeof MealPlanItemDtoSchema>;

export const MealPlanSlotDtoSchema = z.object({
  weekDay: WeekDaySchema,
  mealType: MealTypeSchema,
  items: z.array(MealPlanItemDtoSchema),
});

export type MealPlanSlotDto = z.infer<typeof MealPlanSlotDtoSchema>;

export const MealPlanDtoSchema = z.object({
  id: z.string().uuid().optional(),
  slots: z.array(MealPlanSlotDtoSchema),
});

export type MealPlanDto = z.infer<typeof MealPlanDtoSchema>;

// ── Request (запрос к API) ────────────────────────────────────────────────────

export const MealPlanItemRequestSchema = z.object({
  recipeId: z.string().uuid(),
  servings: z.number().int().min(1).max(99),
});

export type MealPlanItemRequest = z.infer<typeof MealPlanItemRequestSchema>;

export const MealPlanSlotRequestSchema = z.object({
  weekDay: WeekDaySchema,
  mealType: MealTypeSchema,
  items: z.array(MealPlanItemRequestSchema),
});

export type MealPlanSlotRequest = z.infer<typeof MealPlanSlotRequestSchema>;

export const MealPlanRequestSchema = z.object({
  slots: z.array(MealPlanSlotRequestSchema),
});

export type MealPlanRequest = z.infer<typeof MealPlanRequestSchema>;
