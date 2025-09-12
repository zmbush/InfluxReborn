using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace LLib.Gear;


// From caraxi/SimpleTweaks
[Sheet("BaseParam")]
[SuppressMessage("Performance", "CA1815", Justification = "Lumina doesn't implement any equality ops")]
public readonly unsafe struct ExtendedBaseParam(ExcelPage page, uint offset, uint row)
    : IExcelRow<ExtendedBaseParam>
{
    private const int ParamCount = 23;

    public uint RowId => row;
    public BaseParam BaseParam => new(page, offset, row);

    public Collection<ushort> EquipSlotCategoryPct =>
        new(page, offset, offset, &EquipSlotCategoryPctCtor, ParamCount);

    private static ushort EquipSlotCategoryPctCtor(ExcelPage page, uint parentOffset, uint offset, uint i) =>
        i == 0 ? (ushort)0 : page.ReadUInt16(offset + 8 + (i - 1) * 2);

    public static ExtendedBaseParam Create(ExcelPage page, uint offset, uint row) => new(page, offset, row);
}
