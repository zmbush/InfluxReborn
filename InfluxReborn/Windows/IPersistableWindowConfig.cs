namespace InfluxReborn.Windows;

public interface IPersistableWindowConfig
{
    WindowConfig? WindowConfig { get; }

    void SaveWindowConfig();
}

