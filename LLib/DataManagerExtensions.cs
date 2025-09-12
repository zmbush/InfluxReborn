using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Text;
using Lumina.Text.Payloads;
using Lumina.Text.ReadOnly;

namespace LLib;

public static class DataManagerExtensions
{
    public static string WithCertainMacroCodeReplacements(this ReadOnlySeString text)
    {
        return string.Join("", text.Select(payload =>
        {
            return payload.Type switch
            {
                ReadOnlySePayloadType.Text => payload.ToString(),
                ReadOnlySePayloadType.Macro => payload.MacroCode switch
                {
                    MacroCode.NewLine => "",
                    MacroCode.NonBreakingSpace => " ",
                    MacroCode.Hyphen => "-",
                    MacroCode.SoftHyphen => "",
                    _ => payload.ToString()
                },
                _ => payload.ToString()
            };
        }));
    }
}

public interface IQuestDialogueText
{
    public ReadOnlySeString Key { get; }
    public ReadOnlySeString Value { get; }
}

[SuppressMessage("Performance", "CA1815")]
[Sheet("PleaseSpecifyTheSheetExplicitly")]
public readonly struct QuestDialogueText(ExcelPage page, uint offset, uint row) : IQuestDialogueText, IExcelRow<QuestDialogueText>
{
    public uint RowId => row;

    public ReadOnlySeString Key => page.ReadString(offset, offset);
    public ReadOnlySeString Value => page.ReadString(offset + 4, offset);

    static QuestDialogueText IExcelRow<QuestDialogueText>.Create(ExcelPage page, uint offset, uint row) =>
        new(page, offset, row);
}
