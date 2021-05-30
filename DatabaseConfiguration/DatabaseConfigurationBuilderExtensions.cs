using Microsoft.Extensions.Configuration;

namespace DatabaseConfiguration
{
  public static class DatabaseConfigurationBuilderExtensions
  {
    public static IConfigurationBuilder AddDatabaseConfiguration(
      this IConfigurationBuilder builder, string databaseName, string collectionName)
    {
      var tempConfig = builder.Build();
      return builder.Add(new DatabaseConfigurationSource(tempConfig, databaseName, collectionName));
    }
  }
}