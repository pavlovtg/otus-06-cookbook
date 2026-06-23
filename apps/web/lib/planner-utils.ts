import type { MealPlanDto, MealPlanRequest } from "@/lib/schemas/meal-plan";

// Внутренний формат страницы: ключ = `${dayIdx}_${meal}`, dayIdx 0–6
export type PlanItem = { recipeId: string; servings: number };
export type Plan = Record<string, PlanItem[]>;

export const DAY_LABELS = ["Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс"];
export const MEAL_KEYS = ["breakfast", "lunch", "dinner"] as const;
export type MealKey = (typeof MEAL_KEYS)[number];
export const MEAL_LABELS: Record<MealKey, string> = {
  breakfast: "Завтрак",
  lunch: "Обед",
  dinner: "Ужин",
};

// weekDay 1–7 (Mon=1) → dayIdx 0–6
const WEEK_DAY_TO_IDX: Record<number, number> = {
  1: 0,
  2: 1,
  3: 2,
  4: 3,
  5: 4,
  6: 5,
  7: 6,
};
const IDX_TO_WEEK_DAY: Record<number, number> = {
  0: 1,
  1: 2,
  2: 3,
  3: 4,
  4: 5,
  5: 6,
  6: 7,
};

// mealType 1–3 → meal key
const MEAL_TYPE_TO_KEY: Record<number, MealKey> = {
  1: "breakfast",
  2: "lunch",
  3: "dinner",
};
const MEAL_KEY_TO_TYPE: Record<MealKey, number> = {
  breakfast: 1,
  lunch: 2,
  dinner: 3,
};

export function emptyPlan(): Plan {
  const p: Plan = {};
  for (let d = 0; d < 7; d++) {
    for (const m of MEAL_KEYS) {
      p[`${d}_${m}`] = [];
    }
  }
  return p;
}

export function dtoToPlan(dto: MealPlanDto): Plan {
  const plan = emptyPlan();
  for (const slot of dto.slots) {
    const dayIdx = WEEK_DAY_TO_IDX[slot.weekDay];
    const mealKey = MEAL_TYPE_TO_KEY[slot.mealType];
    if (dayIdx === undefined || !mealKey) continue;
    const key = `${dayIdx}_${mealKey}`;
    plan[key] = slot.items.map((it) => ({
      recipeId: it.recipeId,
      servings: it.servings,
    }));
  }
  return plan;
}

export function planToRequest(plan: Plan): MealPlanRequest {
  const slots = [];
  for (let d = 0; d < 7; d++) {
    for (const m of MEAL_KEYS) {
      const key = `${d}_${m}`;
      const items = plan[key] ?? [];
      if (items.length === 0) continue;
      slots.push({
        weekDay: IDX_TO_WEEK_DAY[d]!,
        mealType: MEAL_KEY_TO_TYPE[m],
        items: items.map((it) => ({
          recipeId: it.recipeId,
          servings: it.servings,
        })),
      });
    }
  }
  return { slots };
}
