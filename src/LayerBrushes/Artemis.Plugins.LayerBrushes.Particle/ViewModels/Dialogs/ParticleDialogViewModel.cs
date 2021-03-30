using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artemis.Plugins.LayerBrushes.Particle.ViewModels.Dialogs
{
    public class ParticleDialogViewModel
    {
        public ParticleViewModel ParticleViewModel { get; }

        public ParticleDialogViewModel(ParticleViewModel particleViewModel)
        {
            ParticleViewModel = particleViewModel;
        }
    }
}