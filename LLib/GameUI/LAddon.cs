using System;
using System.Linq;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace LLib.GameUI;

public static class LAddon
{
    public static unsafe bool TryGetAddonByName<T>(this IGameGui gameGui, string addonName, out T* addonPtr)
        where T : unmanaged
    {
        ArgumentNullException.ThrowIfNull(gameGui);
        ArgumentException.ThrowIfNullOrEmpty(addonName);

        var a = gameGui.GetAddonByName(addonName);
        if (!a.IsNull)
        {
            addonPtr = (T*)a.Address;
            return true;
        }
        else
        {
            addonPtr = null;
            return false;
        }
    }
}
