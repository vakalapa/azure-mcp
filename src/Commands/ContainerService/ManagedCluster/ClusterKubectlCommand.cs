public sealed class ClusterKubectlCommand(ILogger<ClusterKubectlCommand> logger) : BaseManagedClusterCommand<ClusterKubectlOptions>()
{
    private const string CommandTitle = "Run Kubectl Command";
    private readonly ILogger<ClusterKubectlCommand> _logger = logger;

    public override string Name => "kubectl";

    public override string Description =>
        $"""
        Execute a kubectl command against the specified AKS cluster. If no kubeconfig is provided,
        cluster user credentials will be retrieved automatically.
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
            string kubeconfig = options.KubeConfig ?? await service.GetUserKubeConfigAsync(
                options.Cluster!, options.ResourceGroup!, options.Subscription!, options.Tenant, options.RetryPolicy);

            var result = await service.RunKubectlCommandAsync(options.Command!, kubeconfig);
            if (result.ExitCode != 0)
            {
                context.Response.Status = 500;
                context.Response.Message = result.Error;
            }

            var jElem = service.ParseKubectlResult(result);
            context.Response.Results = ResponseResult.Create(jElem, JsonSourceGenerationContext.Default.JsonElement);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error running kubectl command");
            HandleException(context.Response, ex);
        }

        return context.Response;
    }
}