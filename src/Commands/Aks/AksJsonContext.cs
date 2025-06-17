using System.Text.Json.Serialization;
using Azure.ResourceManager.ContainerService.Models;

namespace AzureMcp.Commands.Aks;

[JsonSerializable(typeof(ClusterListCommand.ClusterListCommandResult))]
[JsonSerializable(typeof(ContainerServiceManagedClusterData))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault)]
internal sealed partial class AksJsonContext : JsonSerializerContext
{
}
