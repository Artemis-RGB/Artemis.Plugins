using Artemis.Core;
using Artemis.Storage.Entities.Profile;

namespace Artemis.Plugins.Nodes.General.Nodes.Input;

public class HotkeyEnableDisableNodeEntity
{
    public HotkeyEnableDisableNodeEntity(Hotkey? enableHotkey, Hotkey? disableHotkey)
    {
        enableHotkey?.Save();
        EnableHotkey = enableHotkey?.Entity;
        disableHotkey?.Save();
        DisableHotkey = disableHotkey?.Entity;
    }

    public ProfileConfigurationHotkeyEntity? EnableHotkey { get; set; }
    public ProfileConfigurationHotkeyEntity? DisableHotkey { get; set; }
}