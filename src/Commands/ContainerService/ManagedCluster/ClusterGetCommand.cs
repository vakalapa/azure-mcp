using AzureMcp.Options.ContainerService.ManagedCluster;
using AzureMcp.Services.Interfaces;
using Azure.ResourceManager.ContainerService;
using Microsoft.Extensions.Logging;

namespace AzureMcp.Commands.ContainerService.ManagedCluster;

public sealed class ClusterGetCommand(ILogger<ClusterGetCommand> logger) : BaseManagedClusterCommand<ClusterGetOptions>()
{
    private const string CommandTitle = "Get Managed Cluster";
    private readonly ILogger<ClusterGetCommand> _logger = logger;

    public override string Name => "get";

    public override string Description =>
        $"""
        Get details for an Azure Kubernetes Service managed cluster.
        Returns the cluster properties as JSON.
        """;

    public override string Title => CommandTitle;

    [McpServerTool(Destructive = false, ReadOnly = true, Title = CommandTitle)]
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
            var cluster = await service.GetManagedClusterAsync(
                options.Cluster!,
                options.ResourceGroup!,
                options.Subscription!,
                options.Tenant,
                options.RetryPolicy);

            context.Response.Results = ResponseResult.Create(
                new ClusterGetCommandResult(cluster.Data),
                ContainerServiceJsonContext.Default.ClusterGetCommandResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting managed cluster");
            HandleException(context.Response, ex);
        }

        return context.Response;
    }

    internal record ClusterGetCommandResult(ManagedClusterData Cluster);
}