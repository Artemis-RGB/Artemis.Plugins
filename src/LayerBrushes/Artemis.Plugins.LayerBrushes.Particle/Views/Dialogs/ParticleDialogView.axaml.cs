using System.Threading.Tasks;
using Artemis.Plugins.LayerBrushes.Particle.ViewModels.Dialogs;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Avalonia.Threading;

namespace Artemis.Plugins.LayerBrushes.Particle.Views.Dialogs;

public partial class ParticleDialogView : ReactiveUserControl<ParticleDialogViewModel>
{
    public ParticleDialogView()
    {
        InitializeComponent();
        
    }
    
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
    
        // Workaround for the scroll viewer scrolling down on its own
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            await Task.Delay(100);
            this.Get<ScrollViewer>("ParticleFormScrollViewer").ScrollToHome();
        });
    }

}