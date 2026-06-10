import type { Meta, StoryObj } from '@storybook/react';
import { AppDemo } from './AppDemo';
import { LoginScreen } from './LoginScreen';
import { RecipeList } from './RecipeList';
import { RecipeDetail } from './RecipeDetail';
import { RecipeForm } from './RecipeForm';
import { MealPlanner } from './MealPlanner';
import { ShoppingList } from './ShoppingList';
import { Favorites } from './Favorites';
import { Dashboard } from './Dashboard';
import { AppShell } from '../components/AppShell';
import { buildSeedRecipes } from '../mock/data';
import { avgRating } from '../mock/fakeApi';

const meta: Meta = { title: 'Screens', tags: ['autodocs'], parameters: { layout: 'fullscreen' } };
export default meta;

function shell(active: string, children: any) {
  return <AppShell active={active} user={{ name: 'Анна' }}>{children}</AppShell>;
}

export const App: StoryObj = {
  name: 'AppDemo (full)',
  render: () => <AppDemo />,
};

export const Login: StoryObj = {
  name: 'Login',
  render: () => shell('login', <LoginScreen />),
};

export const Recipes: StoryObj = {
  name: 'Recipe list + filters',
  render: () => shell('recipes', <RecipeList />),
};

export const Detail: StoryObj = {
  name: 'Recipe detail',
  render: () => {
    const id = buildSeedRecipes()[0]!.id;
    return shell('recipes', <RecipeDetail recipeId={id} />);
  },
};

export const Form: StoryObj = {
  name: 'Recipe form (create/edit)',
  render: () => shell('recipes', <RecipeForm />),
};

export const Favs: StoryObj = {
  name: 'Favorites',
  render: () => shell('favorites', <Favorites />),
};

export const MealPlan: StoryObj = {
  name: 'Meal planner (DnD)',
  render: () => shell('mealplan', <MealPlanner />),
};

export const Shopping: StoryObj = {
  name: 'Shopping list',
  render: () => shell('shopping', <ShoppingList />),
};

export const DashboardStory: StoryObj = {
  name: 'Dashboard',
  render: () => shell('dashboard', <Dashboard />),
};

// touch avgRating to keep import for tree-shaking sanity
void avgRating;
