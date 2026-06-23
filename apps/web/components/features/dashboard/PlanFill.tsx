import * as React from "react";

const DAYS: { name: string; label: string }[] = [
  { name: "Monday",    label: "Пн" },
  { name: "Tuesday",   label: "Вт" },
  { name: "Wednesday", label: "Ср" },
  { name: "Thursday",  label: "Чт" },
  { name: "Friday",    label: "Пт" },
  { name: "Saturday",  label: "Сб" },
  { name: "Sunday",    label: "Вс" },
];
const MEAL_KEYS = ["Breakfast", "Lunch", "Dinner"] as const;
type MealKey = (typeof MEAL_KEYS)[number];
const MEAL_LABELS: Record<MealKey, string> = {
  Breakfast: "Завтрак",
  Lunch: "Обед",
  Dinner: "Ужин",
};

export function PlanFill({ filled }: { filled: Record<string, boolean> }) {
  return (
    <>
      <div className="mini-week-grid">
        {DAYS.map((d) => (
          <div key={d.name} className="mini-week-day">
            <span className="day-label">{d.label}</span>
            {MEAL_KEYS.map((m) => (
              <div
                key={m}
                className={[
                  "mini-slot",
                  filled[`${d.name}_${m}`] ? "is-filled" : "",
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
