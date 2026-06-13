using Microsoft.EntityFrameworkCore;

namespace Shared.Database.ConnectionStrings;

// ReSharper disable once UnusedTypeParameter
public class DatabaseConnectionOptions<T> : DatabaseConnectionOptions where T : DbContext
{
}
