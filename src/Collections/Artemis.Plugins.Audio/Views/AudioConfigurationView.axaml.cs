using Artemis.Plugins.Audio.ViewModels;
using ReactiveUI.Avalonia;

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
