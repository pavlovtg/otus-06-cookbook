import * as React from "react";

const DAY_LABELS = ["Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс"];
const MEAL_KEYS = ["breakfast", "lunch", "dinner"] as const;
type MealKey = (typeof MEAL_KEYS)[number];
const MEAL_LABELS: Record<MealKey, string> = {
  breakfast: "Завтрак",
  lunch: "Обед",
  dinner: "Ужин",
};

export function PlanFill({ filled }: { filled: Record<string, boolean> }) {
  return (
    <>
      <div className="mini-week-grid">
        {DAY_LABELS.map((d, di) => (
          <div key={d} className="mini-week-day">
            <span className="day-label">{d}</span>
            {MEAL_KEYS.map((m) => (
              <div
                key={m}
                className={[
                  "mini-slot",
                  filled[`${di}_${m}`] ? "is-filled" : "",
                ]
                  .filter(Boolean)
                  .join(" ")}
                title={MEAL_LABELS[m]}
              />
            ))}
          </div>
        ))}
      </div>
      <p className="t-micro">Фиолетовая плитка — слот заполнен.</p>
    </>
  );
}
