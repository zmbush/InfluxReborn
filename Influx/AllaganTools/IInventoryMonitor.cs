using System.Collections.Generic;

namespace InfluxReborn.AllaganTools;

internal interface IInventoryMonitor
{
    public IReadOnlyDictionary<ulong, Inventory> All { get; }
}
