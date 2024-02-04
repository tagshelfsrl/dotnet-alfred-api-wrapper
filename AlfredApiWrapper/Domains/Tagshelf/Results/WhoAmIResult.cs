using Newtonsoft.Json;
using System.Collections.Generic;

public class WhoAmIResult
{
    [JsonProperty("request_origin_ip_address")]
    public string RequestOriginIpAddress { get; set; }

    [JsonProperty("authentication_type")]
    public string AuthenticationType { get; set; }

    [JsonProperty("roles")]
    public List<string> Roles { get; set; }

    [JsonProperty("user_name")]
    public string UserName { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("last_name")]
    public string LastName { get; set; }

    [JsonProperty("user_id")]
    public string UserId { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("company_id")]
    public string CompanyId { get; set; }

    [JsonProperty("company_name")]
    public string CompanyName { get; set; }

    [JsonProperty("app_id")]
    public string AppId { get; set; }

    [JsonProperty("app_name")]
    public string AppName { get; set; }

    /// <summary>
    /// Returns a string that represents the current object, formatted to include relevant user information.
    /// </summary>
    /// <returns>A string that contains the user's UserName, Name, LastName, Roles, Status, and CompanyName.</returns>
    public override string ToString()
    {
        return $"UserName: {UserName}, Name: {Name} {LastName}, Roles: {string.Join(", ", Roles)}, Status: {Status}, Company: {CompanyName}, App: {AppName}, AppId: {AppId}, RequestOriginIpAddress: {RequestOriginIpAddress}";
    }
}
