using System.Diagnostics.CodeAnalysis;
using AzureMcp.Commands.Subscription;
using AzureMcp.Models.Option;
using AzureMcp.Options.Aks;

namespace AzureMcp.Commands.Aks;

public abstract class BaseAksCommand<
    [DynamicallyAccessedMembers(TrimAnnotations.CommandAnnotations)] TOptions>
    : SubscriptionCommand<TOptions> where TOptions : BaseAksOptions, new()
{
    protected readonly Option<string> _resourceGroupOption = OptionDefinitions.Common.ResourceGroup;
    protected readonly Option<string> _clusterOption = OptionDefinitions.Aks.Cluster;

    protected override void RegisterOptions(Command command)
    {
        base.RegisterOptions(command);
        command.AddOption(_resourceGroupOption);
        command.AddOption(_clusterOption);
    }

    protected override TOptions BindOptions(ParseResult parseResult)
    {
        var options = base.BindOptions(parseResult);
        options.ResourceGroup = parseResult.GetValueForOption(_resourceGroupOption);
        if (options is Options.Aks.Cluster.BaseClusterOptions clusterOptions)
        {
            clusterOptions.Cluster = parseResult.GetValueForOption(_clusterOption);
        }
        return options;
    }
}
