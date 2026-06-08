using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Recipes.Infrastructure;

internal sealed class InternalControllersFeatureProvider : ControllerFeatureProvider
{
    protected override bool IsController(TypeInfo typeInfo)
        => base.IsController(typeInfo) || (typeInfo.IsNotPublic && !typeInfo.IsAbstract && !typeInfo.IsGenericTypeDefinition
            && typeof(Microsoft.AspNetCore.Mvc.ControllerBase).IsAssignableFrom(typeInfo));
}
