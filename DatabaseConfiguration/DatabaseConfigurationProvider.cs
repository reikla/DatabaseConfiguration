﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DatabaseConfiguration
{

  public static class NotifyConfigurationChanged
  {
    public static Action<string> OnConfigurationChanged { get; set; }
    public static void ConfigurationChanged(string collection)
    {
      OnConfigurationChanged(collection);
    }
  }
  
  public class DatabaseConfigurationProvider : ConfigurationProvider
  {
    private readonly string _databaseName;
    private readonly string _collectionName;

    public DatabaseConfigurationProvider(IConfiguration configuration, string databaseName, string collectionName, bool reloadOnChange)
    {
      _databaseName = databaseName;
      _collectionName = collectionName;
      Configuration = configuration;

      if (reloadOnChange)
      {
        NotifyConfigurationChanged.OnConfigurationChanged = (collection) =>
        {
          if (collection == _collectionName)
          {
            Load();
          }
        };
      }
    }

    public IConfiguration Configuration { get; }

    public override void Load()
    {
      var connectionString = Configuration.GetSection("Mongodb")["ConnectionString"];

      var client = new MongoClient(connectionString);

      var database = client.GetDatabase(_databaseName);
      var collection = database.GetCollection<BsonDocument>(_collectionName);

      var documents = collection.AsQueryable();

      var myData = BsonTypeMapper.MapToDotNetValue(documents.First());

      var myJson = JsonSerializer.Serialize(myData);

      using var stream = new MemoryStream();

      using (var sw = new StreamWriter(stream, Encoding.UTF8, 1024, true))
      {
        sw.Write(myJson);
        sw.Close();  
      }
      
      stream.Seek(0, SeekOrigin.Begin);
      Data = JsonConfigurationFileParser.Parse(stream);
      OnReload();
    }
  }
}