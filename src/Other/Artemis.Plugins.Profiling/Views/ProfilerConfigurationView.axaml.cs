using Artemis.Plugins.Profiling.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI.Avalonia;

namespace Artemis.Plugins.Profiling.Views;

public partial class ProfilerConfigurationView : ReactiveUserControl<ProfilerConfigurationViewModel>
{
    public ProfilerConfigurationView()
    {
        InitializeComponent();
    }

}