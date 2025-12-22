using Artemis.UI.Shared;
using ReactiveUI.Avalonia;

namespace Artemis.Plugins.Nodes.General.Nodes.Input.Screens;

public partial class HotkeyToggleNodeCustomView : ReactiveUserControl<HotkeyToggleNodeCustomViewModel>
{
    public HotkeyToggleNodeCustomView()
    {
        InitializeComponent();
    }


    private void HotkeyBox_OnHotkeyChanged(HotkeyBox sender, EventArgs args)
    {
        ViewModel?.Save();
    }
}