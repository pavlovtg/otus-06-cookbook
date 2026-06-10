import { useEffect, useState } from 'react';
import { AppShell } from '../components/AppShell';
import { fakeApi, setCurrentUser, currentUser } from '../mock/fakeApi';
import { RecipeList } from './RecipeList';
import { RecipeDetail } from './RecipeDetail';
import { RecipeForm } from './RecipeForm';
import { MealPlanner } from './MealPlanner';
import { ShoppingList } from './ShoppingList';
import { Favorites } from './Favorites';
import { Dashboard } from './Dashboard';
import { LoginScreen } from './LoginScreen';

type Route =
  | { name: 'recipes' }
  | { name: 'recipe'; id: string }
  | { name: 'new' }
  | { name: 'favorites' }
  | { name: 'mealplan' }
  | { name: 'shopping' }
  | { name: 'dashboard' }
  | { name: 'login' };

function parseHash(): Route {
  const h = (typeof window !== 'undefined' ? window.location.hash : '').replace(/^#/, '');
  if (h.startsWith('recipe:')) return { name: 'recipe', id: h.slice('recipe:'.length) };
  switch (h) {
    case 'new': return { name: 'new' };
    case 'favorites': return { name: 'favorites' };
    case 'mealplan': return { name: 'mealplan' };
    case 'shopping': return { name: 'shopping' };
    case 'dashboard': return { name: 'dashboard' };
    case 'login': return { name: 'login' };
    default: return { name: 'recipes' };
  }
}

export function AppDemo() {
  const [route, setRoute] = useState<Route>(parseHash());
  const [user, setUser] = useState<{ name: string } | null>(currentUser());

  useEffect(() => {
    const fn = () => setRoute(parseHash());
    window.addEventListener('hashchange', fn);
    return () => window.removeEventListener('hashchange', fn);
  }, []);

  function go(hash: string) { window.location.hash = hash; }

  const activeNav =
    route.name === 'recipe' || route.name === 'new' ? 'recipes' : route.name;

  return (
    <AppShell
      active={activeNav}
      user={user}
      onNav={(id) => go(id)}
      onLogin={() => go('login')}
      onLogout={() => { fakeApi.logout(); setCurrentUser(null); setUser(null); go('recipes'); }}
    >
      <div className="row" style={{ justifyContent: 'space-between', marginBottom: 16 }}>
        <div className="muted">Hash: #{route.name}{route.name === 'recipe' ? ':' + route.id : ''}</div>
        {route.name === 'recipes' && <button className="btn primary" onClick={() => go('new')}>+ Новый рецепт</button>}
        {route.name === 'recipe' && <button className="btn" onClick={() => go('recipes')}>← К списку</button>}
      </div>

      {route.name === 'recipes' && <RecipeList onOpen={(id) => go('recipe:' + id)} />}
      {route.name === 'recipe' && <RecipeDetail recipeId={route.id} />}
      {route.name === 'new' && <RecipeForm />}
      {route.name === 'favorites' && <Favorites onOpen={(id) => go('recipe:' + id)} />}
      {route.name === 'mealplan' && <MealPlanner />}
      {route.name === 'shopping' && <ShoppingList />}
      {route.name === 'dashboard' && <Dashboard />}
      {route.name === 'login' && (
        <LoginScreen onLogged={(name) => { setUser({ name }); go('recipes'); }} />
      )}
    </AppShell>
  );
}
