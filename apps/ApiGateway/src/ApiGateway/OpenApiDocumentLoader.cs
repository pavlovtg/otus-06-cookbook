using System.Text.Json;
using YamlDotNet.Serialization;

namespace ApiGateway;

internal static class OpenApiDocumentLoader
{
    public static string LoadAndRewrite(string yamlPath, string pathPrefix)
    {
        var yaml = File.ReadAllText(yamlPath);
        var deserializer = new DeserializerBuilder().Build();
        var root = (Dictionary<object, object>)deserializer.Deserialize<object>(yaml)!;

        // Перезапись paths: /api/... -> /api/<prefix>/...
        if (root.TryGetValue("paths", out var pathsObj) && pathsObj is Dictionary<object, object> paths)
        {
            var rewritten = new Dictionary<object, object>();
            foreach (var (key, value) in paths)
            {
                var k = key.ToString() ?? string.Empty;
                if (k.StartsWith("/api/", StringComparison.Ordinal))
                {
                    k = "/api/" + pathPrefix + k.Substring("/api".Length);
                }
                rewritten[k] = value;
            }
            root["paths"] = rewritten;
        }

        // Сервер — относительный, чтобы Try it out шёл через текущий хост
        root["servers"] = new List<object>
        {
            new Dictionary<object, object> { ["url"] = "/" }
        };

        return JsonSerializer.Serialize(ConvertToJsonCompatible(root));
    }

    private static object? ConvertToJsonCompatible(object? value)
    {
        return value switch
        {
            Dictionary<object, object> dict => dict.ToDictionary(
                kv => kv.Key.ToString() ?? string.Empty,
                kv => ConvertToJsonCompatible(kv.Value)),
            List<object> list => list.Select(ConvertToJsonCompatible).ToList(),
            _ => value,
        };
    }
}
