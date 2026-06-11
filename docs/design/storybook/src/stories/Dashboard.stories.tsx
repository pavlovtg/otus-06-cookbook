import type { Meta, StoryObj } from '@storybook/react';
import * as React from 'react';
import { KpiGrid } from '../dash/Kpi';
import { BarChart } from '../dash/BarChart';
import { TopList } from '../dash/TopList';
import { PlanFill } from '../dash/PlanFill';

const meta: Meta = { title: 'Domain/Dashboard', parameters: { layout: 'fullscreen' } };
export default meta;
type S = StoryObj;

export const KPIs: S = {
  render: () => (
    <div style={{ padding: 24 }}>
      <KpiGrid
        items={[
          { label: 'Всего рецептов', value: 26 },
          { label: 'Мои рецепты', value: 9 },
          { label: 'Мои комментарии', value: 14 },
          { label: 'Пользователей', value: 3 },
        ]}
      />
    </div>
  ),
};

export const Bars: S = {
  render: () => (
    <div className="dash-block" style={{ margin: 24, maxWidth: 720 }}>
      <h3>По национальной кухне</h3>
      <BarChart
        data={[
          ['Итальянская', 5],
          ['Русская', 3],
          ['Французская', 2],
          ['Японская', 1],
          ['Тайская', 1],
          ['Мексиканская', 2],
        ]}
      />
    </div>
  ),
};

export const Top: S = {
  render: () => (
    <div className="dash-block" style={{ margin: 24, maxWidth: 480 }}>
      <h3>Топ-10 по рейтингу</h3>
      <TopList
        items={[
          { name: 'Карбонара', value: '4.7' },
          { name: 'Маргарита', value: '4.6' },
          { name: 'Шоколадный фондан', value: '4.5' },
          { name: 'Борщ украинский', value: '4.4' },
          { name: 'Цезарь с курицей', value: '4.2' },
        ]}
      />
    </div>
  ),
};

export const WeekFill: S = {
  render: () => (
    <div className="dash-block" style={{ margin: 24, maxWidth: 480 }}>
      <h3>Заполненность плана меню</h3>
      <PlanFill
        filled={{
          '0_breakfast': true,
          '0_dinner': true,
          '1_lunch': true,
          '2_dinner': true,
          '4_dinner': true,
          '5_breakfast': true,
          '5_dinner': true,
        }}
      />
    </div>
  ),
};

export const Playground: S = {
  render: () => (
    <div style={{ padding: 24, display: 'flex', flexDirection: 'column', gap: 16 }}>
      <KpiGrid
        items={[
          { label: 'Всего рецептов', value: 26 },
          { label: 'Мои рецепты', value: 9 },
          { label: 'Мои комментарии', value: 14 },
          { label: 'Пользователей', value: 3 },
        ]}
      />
      <div className="dash-grid">
        <div className="dash-block">
          <h3>Топ-10 по рейтингу</h3>
          <TopList
            items={[
              { name: 'Карбонара', value: '4.7' },
              { name: 'Маргарита', value: '4.6' },
              { name: 'Шоколадный фондан', value: '4.5' },
              { name: 'Борщ украинский', value: '4.4' },
            ]}
          />
        </div>
        <div className="dash-block">
          <h3>Заполненность плана меню</h3>
          <PlanFill
            filled={{ '0_breakfast': true, '0_dinner': true, '1_lunch': true, '2_dinner': true, '4_dinner': true }}
          />
        </div>
        <div className="dash-block">
          <h3>По основному ингредиенту</h3>
          <BarChart
            data={[
              ['Мясные', 6],
              ['Овощные', 4],
              ['Птичьи', 3],
              ['Зерновые', 3],
              ['Бобовые', 2],
            ]}
          />
        </div>
        <div className="dash-block">
          <h3>По национальной кухне</h3>
          <BarChart
            data={[
              ['Итальянская', 5],
              ['Русская', 3],
              ['Французская', 2],
              ['Японская', 1],
              ['Тайская', 1],
            ]}
          />
        </div>
      </div>
    </div>
  ),
};
