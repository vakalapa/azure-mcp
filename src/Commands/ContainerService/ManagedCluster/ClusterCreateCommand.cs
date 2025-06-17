using AzureMcp.Options.ContainerService.ManagedCluster;
using AzureMcp.Services.Interfaces;
using Azure.ResourceManager.ContainerService;
using Microsoft.Extensions.Logging;

namespace AzureMcp.Commands.ContainerService.ManagedCluster;

public sealed class ClusterCreateCommand(ILogger<ClusterCreateCommand> logger) : BaseManagedClusterCommand<ClusterCreateOptions>()
{
    private const string CommandTitle = "Create Managed Cluster";
    private readonly ILogger<ClusterCreateCommand> _logger = logger;

    public override string Name => "create";

    public override string Description =>
        $"""
        Create a new Azure Kubernetes Service managed cluster.
        """;

    public override string Title => CommandTitle;

    [McpServerTool(Destructive = true, ReadOnly = false, Title = CommandTitle)]
    public override async Task<CommandResponse> ExecuteAsync(CommandContext context, ParseResult parseResult)
    {
        var options = BindOptions(parseResult);

        try
        {
            if (!Validate(parseResult.CommandResult, context.Response).IsValid)
            {
                return context.Response;
            }

            var service = context.GetService<IContainerService>();
            var cluster = await service.CreateManagedClusterAsync(
                options.Cluster!,
                options.ResourceGroup!,
                options.Subscription!,
                options.Location!,
                options.DnsPrefix!,
                options.NodeCount ?? 1,
                options.NodeVmSize ?? "Standard_DS2_v2",
                options.Tenant,
                options.RetryPolicy);

            context.Response.Results = ResponseResult.Create(
                new ClusterCreateCommandResult(cluster.Data),
                ContainerServiceJsonContext.Default.ClusterCreateCommandResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating managed cluster");
            HandleException(context.Response, ex);
        }

        return context.Response;
    }

    internal record ClusterCreateCommandResult(ManagedClusterData Cluster);
}