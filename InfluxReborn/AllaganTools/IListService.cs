namespace InfluxReborn.AllaganTools;

internal interface IListService
{
    FilterResult? GetFilterByKeyOrName(string keyOrName);
}
