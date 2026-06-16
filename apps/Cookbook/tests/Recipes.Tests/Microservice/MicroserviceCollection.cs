using Xunit;

namespace Recipes.Tests.Microservice;

[CollectionDefinition("RecipeMicroservice")]
public sealed class RecipeMicroserviceCollection : ICollectionFixture<RecipeMicroserviceFixture>;
