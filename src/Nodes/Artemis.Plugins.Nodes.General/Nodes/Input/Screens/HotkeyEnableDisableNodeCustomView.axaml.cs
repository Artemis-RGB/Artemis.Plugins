using Artemis.UI.Shared;
using Avalonia.ReactiveUI;

namespace Artemis.Plugins.Nodes.General.Nodes.Input.Screens;

public partial class HotkeyEnableDisableNodeCustomView : ReactiveUserControl<HotkeyEnableDisableNodeCustomViewModel>
{
    public HotkeyEnableDisableNodeCustomView()
    {
        InitializeComponent();
    }


    private void HotkeyBox_OnHotkeyChanged(HotkeyBox sender, EventArgs args)
    {
        ViewModel?.Save();
    }
}