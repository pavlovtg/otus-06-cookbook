"use client";

import * as React from "react";
import { BarChart } from "./BarChart";
import type { CategoryCountDto } from "@/lib/schemas/dashboard";

interface DashboardChartsProps {
  byMainIngredient: CategoryCountDto[];
  byCuisine: CategoryCountDto[];
}

export function DashboardCharts({
  byMainIngredient,
  byCuisine,
}: DashboardChartsProps) {
  return (
    <>
      <div className="dash-block">
        <h3 className="dash-block-title">По основному ингредиенту</h3>
        <BarChart
          data={byMainIngredient.map((c) => [c.categoryName, c.recipeCount])}
        />
      </div>
      <div className="dash-block">
        <h3 className="dash-block-title">По национальной кухне</h3>
        <BarChart
          data={byCuisine.map((c) => [c.categoryName, c.recipeCount])}
        />
      </div>
    </>
  );
}
