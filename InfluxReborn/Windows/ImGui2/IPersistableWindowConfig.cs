namespace InfluxReborn.Windows.ImGui2;

public interface IPersistableWindowConfig
{
    WindowConfig? WindowConfig { get; }

    void SaveWindowConfig();
}

