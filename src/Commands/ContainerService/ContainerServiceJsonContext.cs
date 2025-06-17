namespace AzureMcp.Commands.ContainerService;

[JsonSerializable(typeof(ManagedClusterData))]
[JsonSerializable(typeof(ManagedClusterListCommand.ClusterListCommandResult))]
[JsonSerializable(typeof(ClusterGetCommand.ClusterGetCommandResult))]
[JsonSerializable(typeof(ClusterCreateCommand.ClusterCreateCommandResult))]
[JsonSerializable(typeof(ClusterUpdateCommand.ClusterUpdateCommandResult))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
internal sealed partial class ContainerServiceJsonContext : JsonSerializerContext;