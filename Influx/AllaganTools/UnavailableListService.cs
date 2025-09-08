using Dalamud.Plugin.Services;

namespace InfluxReborn.AllaganTools;

internal sealed class UnavailableListService(IPluginLog pluginLog) : IListService
{
    public FilterResult? GetFilterByKeyOrName(string keyOrName)
    {
        pluginLog.Warning("Filter Service is unavailable");
        return null;
    }
}
