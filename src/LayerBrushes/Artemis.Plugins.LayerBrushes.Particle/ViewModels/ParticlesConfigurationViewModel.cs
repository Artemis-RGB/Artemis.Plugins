using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Artemis.Plugins.LayerBrushes.Particle.Models;
using Artemis.Plugins.LayerBrushes.Particle.PropertyGroups;
using Artemis.Plugins.LayerBrushes.Particle.ViewModels.Dialogs;
using Artemis.UI.Shared.LayerBrushes;
using Artemis.UI.Shared.Services;
using MaterialDesignThemes.Wpf;
using Stylet;

namespace Artemis.Plugins.LayerBrushes.Particle.ViewModels
{
    public class ParticlesConfigurationViewModel : BrushConfigurationViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly IProfileEditorService _profileEditorService;

        public ParticlesConfigurationViewModel(PluginLayerBrush particlesBrush,
            IDialogService dialogService,
            IProfileEditorService profileEditorService) : base(particlesBrush)
        {
            _dialogService = dialogService;
            _profileEditorService = profileEditorService;
            ParticlesBrush = particlesBrush;
            Properties = ParticlesBrush.Properties;

            ParticleViewModels = new BindableCollection<ParticleViewModel>();
            LoadConfiguration();
        }

        public PluginLayerBrush ParticlesBrush { get; }
        public MainPropertyGroup Properties { get; }
        public BindableCollection<ParticleViewModel> ParticleViewModels { get; }

        public async Task AddParticle()
        {
            ParticleConfiguration particleConfiguration = new();
            ParticleViewModel viewModel = new(particleConfiguration);

            if (await ShowParticleDialog(viewModel))
            {
                Properties.ParticleConfigurations.CurrentValue.Add(particleConfiguration);
                ParticleViewModels.Add(viewModel);
                Save();
            }
        }

        public void RemoveParticle(ParticleViewModel particleViewModel)
        {
            Properties.ParticleConfigurations.CurrentValue.Remove(particleViewModel.ParticleConfiguration);
            ParticleViewModels.Remove(particleViewModel);
            Save();
        }

        public async Task EditParticle(ParticleViewModel particleViewModel)
        {
            if (await ShowParticleDialog(particleViewModel))
                Save();
        }

        private async Task<bool> ShowParticleDialog(ParticleViewModel particleViewModel)
        {
            object result = await _dialogService.ShowDialogAt<ParticleDialogViewModel>("BrushSettingsDialog", new Dictionary<string, object> {{"particleViewModel", particleViewModel}});
            if (result is bool booleanResult)
                return booleanResult;
            return false;
        }

        private void Save()
        {
            _profileEditorService.UpdateSelectedProfileElement();
            ParticlesBrush.LoadParticles();
        }

        private void LoadConfiguration()
        {
            if (ParticleViewModels.Any())
                ParticleViewModels.Clear();

            ParticleViewModels.AddRange(Properties.ParticleConfigurations.CurrentValue.Select(p => new ParticleViewModel(p)));
        }
    }
}