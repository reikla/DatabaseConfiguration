using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace DatabaseConfiguration
{
  //Mainly copied from NET Core FW
  internal class JsonConfigurationFileParser
  {
    private JsonConfigurationFileParser()
    {
    }

    private readonly IDictionary<string, string> _data =
      new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    private readonly Stack<string> _context = new Stack<string>();
    private string _currentPath;

    public static IDictionary<string, string> Parse(Stream input)
      => new JsonConfigurationFileParser().ParseStream(input);

    private IDictionary<string, string> ParseStream(Stream input)
    {
      _data.Clear();

      var jsonDocumentOptions = new JsonDocumentOptions
      {
        CommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
      };

      using (var reader = new StreamReader(input))
      using (JsonDocument doc = JsonDocument.Parse(reader.ReadToEnd(), jsonDocumentOptions))
      {
        if (doc.RootElement.ValueKind != JsonValueKind.Object)
        {
          throw new FormatException(string.Format(SR.Error_UnsupportedJSONToken, doc.RootElement.ValueKind));
        }

        VisitElement(doc.RootElement);
      }

      return _data;
    }

    private void VisitElement(JsonElement element)
    {
      foreach (var property in element.EnumerateObject().Where(x=> !x.NameEquals("_id")))
      {
          EnterContext(property.Name);
          VisitValue(property.Value);
          ExitContext();
      }
    }

    private void VisitValue(JsonElement value)
    {
      switch (value.ValueKind)
      {
        case JsonValueKind.Object:
          VisitElement(value);
          break;

        case JsonValueKind.Array:
          int index = 0;
          foreach (JsonElement arrayElement in value.EnumerateArray())
          {
            EnterContext(index.ToString());
            VisitValue(arrayElement);
            ExitContext();
            index++;
          }

          break;

        case JsonValueKind.Number:
        case JsonValueKind.String:
        case JsonValueKind.True:
        case JsonValueKind.False:
        case JsonValueKind.Null:
          string key = _currentPath;
          if (_data.ContainsKey(key))
          {
            throw new FormatException(string.Format(SR.Error_KeyIsDuplicated, key));
          }

          _data[key] = value.ToString();
          break;

        default:
          throw new FormatException(string.Format(SR.Error_UnsupportedJSONToken, value.ValueKind));
      }
    }

    private void EnterContext(string context)
    {
      _context.Push(context);
      _currentPath = ConfigurationPath.Combine(_context.Reverse());
    }

    private void ExitContext()
    {
      _context.Pop();
      _currentPath = ConfigurationPath.Combine(_context.Reverse());
    }

    private static class SR
    {
      /// <summary>File path must be a non-empty string.</summary>
      internal static string @Error_InvalidFilePath => "File path must be a non-empty string.";
      /// <summary>Could not parse the JSON file.</summary>
      internal static string @Error_JSONParseError => "Could not parse the JSON file.";
      /// <summary>A duplicate key '{0}' was found.</summary>
      internal static string @Error_KeyIsDuplicated => "A duplicate key '{0}' was found.";
      /// <summary>Unsupported JSON token '{0}' was found.</summary>
      internal static string @Error_UnsupportedJSONToken => "Unsupported JSON token '{0}' was found.";
    }
  }
}