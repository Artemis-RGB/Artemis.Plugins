using Artemis.Plugins.LayerBrushes.Particle.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

namespace Artemis.Plugins.LayerBrushes.Particle.Views;

public partial class ParticlesConfigurationView : ReactiveUserControl<ParticlesConfigurationViewModel>
{
    public ParticlesConfigurationView()
    {
        InitializeComponent();
    }

}