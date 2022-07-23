using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Artemis.Plugins.LayerBrushes.Particle.LayerProperties;
using Artemis.Plugins.LayerBrushes.Particle.Models;
using Artemis.Plugins.LayerBrushes.Particle.ViewModels.Dialogs;
using Artemis.UI.Shared.LayerBrushes;
using Artemis.UI.Shared.Services;
using Artemis.UI.Shared.Services.ProfileEditor;
using FluentAvalonia.UI.Controls;
using ReactiveUI;
using ContentDialogButton = Artemis.UI.Shared.Services.Builders.ContentDialogButton;

namespace Artemis.Plugins.LayerBrushes.Particle.ViewModels
{
    public class ParticlesConfigurationViewModel : BrushConfigurationViewModel
    {
        private readonly IWindowService _windowService;
        private bool _hasChanges;

        public ParticlesConfigurationViewModel(ParticleLayerBrush particlesBrush, IWindowService windowService) : base(particlesBrush)
        {
            _windowService = windowService;
            ParticlesBrush = particlesBrush;
            Properties = ParticlesBrush.Properties;

            ParticleViewModels = new ObservableCollection<ParticleViewModel>(Properties.ParticleConfigurations.CurrentValue.Select(p => new ParticleViewModel(p)));

            Save = ReactiveCommand.Create(ExecuteSave);
            Cancel = ReactiveCommand.CreateFromTask(ExecuteCancel);
            AddParticle = ReactiveCommand.CreateFromTask(ExecuteAddParticle);
            EditParticle = ReactiveCommand.CreateFromTask<ParticleViewModel>(ExecuteEditParticle);
            RemoveParticle = ReactiveCommand.Create<ParticleViewModel>(ExecuteRemoveParticle);
        }

        public ReactiveCommand<Unit, Unit> Save { get; }
        public ReactiveCommand<Unit, Unit> Cancel { get; }
        public ReactiveCommand<Unit, Unit> AddParticle { get; }
        public ReactiveCommand<ParticleViewModel, Unit> EditParticle { get; }
        public ReactiveCommand<ParticleViewModel, Unit> RemoveParticle { get; }

        public ParticleLayerBrush ParticlesBrush { get; }
        public MainPropertyGroup Properties { get; }
        public ObservableCollection<ParticleViewModel> ParticleViewModels { get; }

        private async Task ExecuteAddParticle()
        {
            ParticleConfiguration particleConfiguration = new();
            ParticleViewModel viewModel = new(particleConfiguration);

            if (await ShowParticleDialog(viewModel, "Add particle"))
            {
                ParticleViewModels.Add(viewModel);
                _hasChanges = true;
            }
        }

        private async Task ExecuteEditParticle(ParticleViewModel particleViewModel)
        {
            if (await ShowParticleDialog(particleViewModel, "Edit particle"))
                _hasChanges = true;
        }

        private void ExecuteRemoveParticle(ParticleViewModel particleViewModel)
        {
            ParticleViewModels.Remove(particleViewModel);
            _hasChanges = true;
        }

        private async Task<bool> ShowParticleDialog(ParticleViewModel particleViewModel, string title)
        {
            ContentDialogResult result = await _windowService.CreateContentDialog()
                .WithTitle(title)
                .WithViewModel(out ParticleDialogViewModel vm, ("particleViewModel", particleViewModel))
                .WithCloseButtonText("Cancel")
                .WithDefaultButton(ContentDialogButton.Primary)
                .HavingPrimaryButton(c => c.WithText("Save changes").WithCommand(vm.Save))
                .ShowAsync();

            return result == ContentDialogResult.Primary;
        }

        private void ExecuteSave()
        {
            Properties.ParticleConfigurations.CurrentValue.Clear();
            Properties.ParticleConfigurations.CurrentValue.AddRange(ParticleViewModels.Select(vm => vm.ParticleConfiguration));

            RequestClose();
        }

        private async Task ExecuteCancel()
        {
            if (_hasChanges && !await _windowService.ShowConfirmContentDialog("Discard changes", "Do you want to discard any changes you made?"))
                return;

            RequestClose();
        }
    }
}