using Google.Cloud.Functions.Framework;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Google.Cloud.Datastore.V1;

namespace HelloHttp;

public class Function : IHttpFunction
{
  private readonly ILogger _logger;
  private readonly DatastoreDb _db;

  public Function(ILogger<Function> logger)
  {
    _logger = logger;
    _db = DatastoreDb.Create("tabajaradevapi");
  }

  public async Task HandleAsync(HttpContext context)
  {
    var request = context.Request;
    var path = request.Path.Value?.ToLower();

    if (path == "/static" && request.Method == "POST")
    {
      await HandlePostStaticAsync(context);
      return;
    }

    if (path == "/static" && request.Method == "GET")
    {
      await HandleGetStaticAsync(context);
      return;
    }

    await context.Response.WriteAsync("Hello!");
  }

  private async Task HandlePostStaticAsync(HttpContext context)
  {
    try
    {
      var teammate = await JsonSerializer.DeserializeAsync<StaticTeammate>(context.Request.Body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

      if (teammate == null)
      {
        _logger.LogWarning("POST /static: Invalid payload (null teammate)");
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("Invalid payload");
        return;
      }

      var entity = new Entity
      {
        Key = _db.CreateKeyFactory("StaticTeammate").CreateIncompleteKey(),
        ["Name"] = teammate.Name,
        ["StaticGUID"] = teammate.StaticGUID,
        ["EarsValue"] = teammate.EarsValue,
        ["NeckValue"] = teammate.NeckValue,
        ["WristsValue"] = teammate.WristsValue,
        ["RingValue"] = teammate.RingValue,
        ["WeaponValue"] = teammate.WeaponValue,
        ["HeadValue"] = teammate.HeadValue,
        ["BodyValue"] = teammate.BodyValue,
        ["HandsValue"] = teammate.HandsValue,
        ["LegsValue"] = teammate.LegsValue,
        ["FeetValue"] = teammate.FeetValue,
        ["WeaponTokenValue"] = teammate.WeaponTokenValue,
        ["WeaponUpgradeValue"] = teammate.WeaponUpgradeValue,
        ["AccUpgradeValue"] = teammate.AccUpgradeValue,
        ["GearUpgradeValue"] = teammate.GearUpgradeValue
      };

      await _db.InsertAsync(entity);

      context.Response.StatusCode = 201;
      await context.Response.WriteAsync("Inserted");
    }
    catch (JsonException ex)
    {
      _logger.LogError(ex, "POST /static: Invalid JSON");
      context.Response.StatusCode = 400;
      await context.Response.WriteAsync("Invalid JSON");
    }
    catch (System.Exception ex)
    {
      _logger.LogError(ex, "POST /static: Unexpected error");
      context.Response.StatusCode = 500;
      await context.Response.WriteAsync("Internal server error");
    }
  }

  private async Task HandleGetStaticAsync(HttpContext context)
  {
    string guid = context.Request.Query["guid"];
    if (string.IsNullOrEmpty(guid))
    {
      _logger.LogWarning("GET /static: Missing guid parameter");
      context.Response.StatusCode = 400;
      await context.Response.WriteAsync("Missing guid parameter");
      return;
    }

    if (!System.Guid.TryParse(guid, out _))
    {
      _logger.LogWarning("GET /static: Invalid guid format '{Guid}'", guid);
      context.Response.StatusCode = 400;
      await context.Response.WriteAsync("Invalid guid format");
      return;
    }

    try
    {
      var query = new Query("StaticTeammate")
      {
        Filter = Filter.Equal("StaticGUID", guid)
      };

      var results = await _db.RunQueryAsync(query);
      var teammates = new List<StaticTeammate>();

      foreach (var entity in results.Entities)
      {
        teammates.Add(EntityToStaticTeammate(entity));
      }

      context.Response.ContentType = "application/json";
      await context.Response.WriteAsync(JsonSerializer.Serialize(teammates));
    }
    catch (System.Exception ex)
    {
      _logger.LogError(ex, "GET /static: Error fetching teammates for guid {Guid}", guid);
      context.Response.StatusCode = 500;
      await context.Response.WriteAsync("Internal server error");
    }
  }

  private StaticTeammate EntityToStaticTeammate(Entity entity)
  {
    var p = entity.Properties;
    return new StaticTeammate
    {
      Name = p.ContainsKey("Name") ? (string)p["Name"] : "",
      StaticGUID = p.ContainsKey("StaticGUID") ? (string)p["StaticGUID"] : "",
      EarsValue = p.ContainsKey("EarsValue") ? (int)(long)p["EarsValue"] : 0,
      NeckValue = p.ContainsKey("NeckValue") ? (int)(long)p["NeckValue"] : 0,
      WristsValue = p.ContainsKey("WristsValue") ? (int)(long)p["WristsValue"] : 0,
      RingValue = p.ContainsKey("RingValue") ? (int)(long)p["RingValue"] : 0,
      WeaponValue = p.ContainsKey("WeaponValue") ? (int)(long)p["WeaponValue"] : 0,
      HeadValue = p.ContainsKey("HeadValue") ? (int)(long)p["HeadValue"] : 0,
      BodyValue = p.ContainsKey("BodyValue") ? (int)(long)p["BodyValue"] : 0,
      HandsValue = p.ContainsKey("HandsValue") ? (int)(long)p["HandsValue"] : 0,
      LegsValue = p.ContainsKey("LegsValue") ? (int)(long)p["LegsValue"] : 0,
      FeetValue = p.ContainsKey("FeetValue") ? (int)(long)p["FeetValue"] : 0,
      WeaponTokenValue = p.ContainsKey("WeaponTokenValue") ? (int)(long)p["WeaponTokenValue"] : 0,
      WeaponUpgradeValue = p.ContainsKey("WeaponUpgradeValue") ? (int)(long)p["WeaponUpgradeValue"] : 0,
      AccUpgradeValue = p.ContainsKey("AccUpgradeValue") ? (int)(long)p["AccUpgradeValue"] : 0,
      GearUpgradeValue = p.ContainsKey("GearUpgradeValue") ? (int)(long)p["GearUpgradeValue"] : 0
    };
  }
}

public class StaticTeammate
{
  public string Name { get; set; } = string.Empty;
  public string StaticGUID { get; set; } = string.Empty;
  //acc
  public int EarsValue { get; set; } = 0;
  public int NeckValue { get; set; } = 0;
  public int WristsValue { get; set; } = 0;
  public int RingValue { get; set; } = 0;
  // gear
  public int WeaponValue { get; set; } = 0;
  public int HeadValue { get; set; } = 0;
  public int BodyValue { get; set; } = 0;
  public int HandsValue { get; set; } = 0;
  public int LegsValue { get; set; } = 0;
  public int FeetValue { get; set; } = 0;

  // upgrades
  public int WeaponTokenValue { get; set; } = 0;
  public int WeaponUpgradeValue { get; set; } = 0;
  public int AccUpgradeValue { get; set; } = 0;
  public int GearUpgradeValue { get; set; } = 0;
}