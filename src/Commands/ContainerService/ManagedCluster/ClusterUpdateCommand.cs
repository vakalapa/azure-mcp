using AzureMcp.Options.ContainerService.ManagedCluster;
using AzureMcp.Services.Interfaces;
using Azure.ResourceManager.ContainerService;
using Microsoft.Extensions.Logging;

namespace AzureMcp.Commands.ContainerService.ManagedCluster;

public sealed class ClusterUpdateCommand(ILogger<ClusterUpdateCommand> logger) : BaseManagedClusterCommand<ClusterUpdateOptions>()
{
    private const string CommandTitle = "Update Managed Cluster";
    private readonly ILogger<ClusterUpdateCommand> _logger = logger;

    public override string Name => "update";

    public override string Description =>
        $"""
        Update an Azure Kubernetes Service managed cluster.
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
            var cluster = await service.UpdateManagedClusterAsync(
                options.Cluster!,
                options.ResourceGroup!,
                options.Subscription!,
                options.NodeCount ?? 1,
                options.Tenant,
                options.RetryPolicy);

            context.Response.Results = ResponseResult.Create(
                new ClusterUpdateCommandResult(cluster.Data),
                ContainerServiceJsonContext.Default.ClusterUpdateCommandResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating managed cluster");
            HandleException(context.Response, ex);
        }

        return context.Response;
    }

    internal record ClusterUpdateCommandResult(ManagedClusterData Cluster);
}