using System.Collections.Generic;
using InfluxReborn.AllaganTools;
using InfluxReborn.LocalStatistics;
using InfluxReborn.SubmarineTracker;

namespace InfluxReborn;

internal sealed class StatisticsUpdate
{
    public required IReadOnlyDictionary<Character, Currencies> Currencies { get; init; }
    public required IReadOnlyDictionary<string, IReadOnlyList<SortingResult>> InventoryItems { get; init; }
    public required IReadOnlyDictionary<Character, SubmarineStats> Submarines { get; init; }
    public required IReadOnlyDictionary<Character, LocalStats> LocalStats { get; init; }
    public required IReadOnlyDictionary<ulong, FcStats> FcStats { get; init; }
}
