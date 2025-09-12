﻿using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Text.ReadOnly;

namespace LLib.GameUI;

public static class SeStringExtensions
{
    public static string WithCertainMacroCodeReplacements(this SeString? str)
    {
        if (str == null)
            return string.Empty;

        // dalamud doesn't have all payload types that Lumina's SeString has, so we don't even know if certain payloads are e.g. soft hyphens
        var seString = new ReadOnlySeString(str.Encode());
        return seString.WithCertainMacroCodeReplacements();
    }
}
