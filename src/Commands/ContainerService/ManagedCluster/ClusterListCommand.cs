using AzureMcp.Options.ContainerService.ManagedCluster;
using AzureMcp.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace AzureMcp.Commands.ContainerService.ManagedCluster;

public sealed class ClusterListCommand(ILogger<ClusterListCommand> logger) : SubscriptionCommand<ClusterListOptions>()
{
    private const string CommandTitle = "List Managed Clusters";
    private readonly ILogger<ClusterListCommand> _logger = logger;

    public override string Name => "list";

    public override string Description =>
        $"""
        List all Azure Kubernetes Service managed clusters in the specified subscription.
        Returns the cluster names as a JSON array.
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
            var clusters = await service.ListManagedClustersAsync(
                options.Subscription!,
                options.Tenant,
                options.RetryPolicy);

            context.Response.Results = clusters.Any()
                ? ResponseResult.Create(new ClusterListCommandResult(clusters.ToList()), ContainerServiceJsonContext.Default.ClusterListCommandResult)
                : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing managed clusters");
            HandleException(context.Response, ex);
        }

        return context.Response;
    }

    internal record ClusterListCommandResult(List<string> Clusters);
}