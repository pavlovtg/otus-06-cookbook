# Active Context

## Текущая задача

Завершена фича `recipe-ingredients` (все 56 задач, включая секцию 8 — тесты).

## Что было сделано в последней сессии

- Unit-тесты домена: `RecipeIngredientTests` (граничные значения amount), `RecipeWithIngredientsTests` (Create/Update с ингредиентами, лимит 50)
- Интеграционные тесты репозитория: создание рецепта с ингредиентами + загрузка, `GetRecipesUsingIngredientAsync` (top-10, total count)
- Microservice-тесты: POST/GET с ингредиентами, PUT обновление/очистка ингредиентов, DELETE блокировка при использовании → 400 с перечнем рецептов
- Фикс существующих microservice-тестов: добавлен `Ingredients: []` во все `RecipeRequest`
- Frontend unit-тесты: `RecipeIngredientDtoSchema` (8.9), `getRecipes` BFF (8.10 — уже был покрыт)
- E2E тесты: создание рецепта с ингредиентами, убирание ингредиента при редактировании
- Фикс e2e: `VALID_RECIPE` дополнен `"ingredients": []`, убраны `servings`/`instructions` из проверки списка (теперь `RecipeShortDto`)
