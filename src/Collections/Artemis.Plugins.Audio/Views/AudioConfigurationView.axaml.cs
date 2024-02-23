using Artemis.Plugins.Audio.ViewModels;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

namespace Artemis.Plugins.Audio.Views
{
    public partial class AudioConfigurationView : ReactiveUserControl<AudioConfigurationViewModel>
    {
        public AudioConfigurationView()
        {
            InitializeComponent();
        }
    }
}
