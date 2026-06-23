import { getDashboardStats } from "@/lib/bff/dashboard.server";
import { KpiGrid } from "@/components/features/dashboard/Kpi";
import { TopList } from "@/components/features/dashboard/TopList";
import { PlanFill } from "@/components/features/dashboard/PlanFill";
import { DashboardCharts } from "@/components/features/dashboard/DashboardCharts";

export const dynamic = "force-dynamic";

export default async function DashboardPage() {
  const stats = await getDashboardStats();

  const kpiItems = [
    { label: "Всего рецептов", value: stats.totalRecipes },
    ...(stats.myRecipes != null
      ? [{ label: "Мои рецепты", value: stats.myRecipes }]
      : []),
    ...(stats.myComments != null
      ? [{ label: "Мои комментарии", value: stats.myComments }]
      : []),
    ...(stats.totalUsers != null
      ? [{ label: "Пользователей", value: stats.totalUsers }]
      : []),
    ...(stats.totalComments != null
      ? [{ label: "Всего комментариев", value: stats.totalComments }]
      : []),
  ];

  return (
    <div className="dash-grid">
      <section className="dash-block" style={{ gridColumn: "1 / -1" }}>
        <KpiGrid items={kpiItems} />
      </section>

      <section className="dash-block">
        <h2 className="dash-block-title">Топ-10 по рейтингу</h2>
        <TopList
          items={stats.top10ByRating.map((r) => ({
            name: r.title,
            value: r.averageRating.toFixed(1),
            withStar: true,
          }))}
        />
      </section>

      {stats.topFavoritesByRating.length > 0 && (
        <section className="dash-block">
          <h2 className="dash-block-title">Топ из избранного</h2>
          <TopList
            items={stats.topFavoritesByRating.map((r) => ({
              name: r.title,
              value: r.averageRating.toFixed(1),
              withStar: true,
            }))}
          />
        </section>
      )}

      {stats.topUsersByRating && stats.topUsersByRating.length > 0 && (
        <section className="dash-block">
          <h2 className="dash-block-title">Топ пользователей по рейтингу</h2>
          <TopList
            items={stats.topUsersByRating.map((u) => ({
              name: u.displayName,
              value: u.averageRating != null ? u.averageRating.toFixed(1) : "—",
              withStar: true,
            }))}
          />
        </section>
      )}

      {stats.topUsersByComments && stats.topUsersByComments.length > 0 && (
        <section className="dash-block">
          <h2 className="dash-block-title">Топ пользователей по комментариям</h2>
          <TopList
            items={stats.topUsersByComments.map((u) => ({
              name: u.displayName,
              value: u.commentCount,
              withStar: false,
            }))}
          />
        </section>
      )}

      <DashboardCharts
        byMainIngredient={stats.byMainIngredient}
        byCuisine={stats.byCuisine}
      />

      {stats.planFill && (
        <section className="dash-block">
          <h2 className="dash-block-title">План меню на неделю</h2>
          <PlanFill filled={stats.planFill} />
        </section>
      )}
    </div>
  );
}
