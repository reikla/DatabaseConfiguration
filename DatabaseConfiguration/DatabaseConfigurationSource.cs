using Microsoft.Extensions.Configuration;

namespace DatabaseConfiguration
{
  public class DatabaseConfigurationSource : IConfigurationSource
  {
    private readonly string _databaseName;
    private readonly string _collectionName;
    private readonly bool _reloadOnChange;

    public DatabaseConfigurationSource(IConfiguration configuration, string databaseName, string collectionName, bool reloadOnChange)
    {
      _databaseName = databaseName;
      _collectionName = collectionName;
      _reloadOnChange = reloadOnChange;
      Configuration = configuration;
    }
    public IConfiguration Configuration { get; }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
      return new DatabaseConfigurationProvider(Configuration, _databaseName, _collectionName, _reloadOnChange);
    }
  }
}