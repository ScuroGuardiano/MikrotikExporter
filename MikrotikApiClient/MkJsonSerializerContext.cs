using System.Text.Json.Serialization;
using MikrotikApiClient.Dto;

namespace MikrotikApiClient;

[JsonSerializable(typeof(InterfaceSummary[]))]
public partial class MkJsonSerializerContext : JsonSerializerContext
{
    
}