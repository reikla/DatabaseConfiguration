using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DatabaseConfiguration
{
  public class DatabaseConfigurationProvider : ConfigurationProvider
  {
    private readonly string _databaseName;
    private readonly string _collectionName;

    public DatabaseConfigurationProvider(IConfiguration configuration, string databaseName, string collectionName)
    {
      _databaseName = databaseName;
      _collectionName = collectionName;
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public override void Load()
    {
      var connectionString = Configuration.GetSection("Mongodb")["ConnectionString"];

      var client = new MongoClient(connectionString);

      var database = client.GetDatabase(_databaseName);
      var collection = database.GetCollection<BsonDocument>(_collectionName);

      var documents = collection.AsQueryable();
      var myData = documents.ToList().ConvertAll(BsonTypeMapper.MapToDotNetValue);

      foreach (var element in myData)
      {
        var myJson = JsonSerializer.Serialize(element);

        using var stream = new MemoryStream();

        var sw = new StreamWriter(stream, Encoding.UTF8, 1024, true);
        sw.Write(myJson);
        sw.Close();
        stream.Seek(0, SeekOrigin.Begin);
        var provider = new JsonConfigurationProvider(new JsonConfigurationSource());

        JsonDocument doc = JsonDocument.Parse(myJson);

        var type = typeof(JsonConfigurationProvider).Assembly.GetType(
          "Microsoft.Extensions.Configuration.Json.JsonConfigurationFileParser");


        var method = type.GetMethod("Parse");

        var result = (IDictionary<string, string>) method.Invoke(null, new[] {stream});

        var toRemove = result.Keys.Where(x => x.StartsWith("_"));
        toRemove.ToList().ForEach(r => result.Remove(r));

        Data = result;
      }
    }
  }
}