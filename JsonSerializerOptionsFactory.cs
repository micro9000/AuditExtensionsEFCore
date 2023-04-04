using System.Text.Json;
using DateOnlyTimeOnly.AspNet.Converters;

public static class JsonSerializerOptionsFactory
{
  public static JsonSerializerOptions Create()
  {
    var jsonSerializerOptions = new JsonSerializerOptions();
    jsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
    return jsonSerializerOptions;
  }
}
