using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter), typeof(Newtonsoft.Json.Serialization.CamelCaseNamingStrategy))]
/// <summary>
/// The processing priority of a job.
/// </summary>
public enum JobPriority
{
    /// <summary>Processed after normal-priority work.</summary>
    Low,

    /// <summary>The platform default.</summary>
    Normal,

    /// <summary>Processed ahead of normal-priority work.</summary>
    High,

    /// <summary>Processed ahead of all other work.</summary>
    Urgent
}