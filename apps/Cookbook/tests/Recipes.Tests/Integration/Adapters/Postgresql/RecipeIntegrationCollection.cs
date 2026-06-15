using Xunit;

namespace Recipes.Tests.Adapters.Postgresql;

[CollectionDefinition("RecipeIntegration")]
public sealed class RecipeIntegrationCollection : ICollectionFixture<RecipeIntegrationFixture>;
