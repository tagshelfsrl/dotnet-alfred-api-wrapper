using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter), typeof(Newtonsoft.Json.Serialization.CamelCaseNamingStrategy))]
public enum JobPriority
{
    Low,
    Normal,
    High
}