using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artemis.Plugins.LayerBrushes.Particle.SKParticle
{
    public readonly struct SKConfettiPhysics
    {
        public SKConfettiPhysics(float size, float mass)
        {
            Size = size;
            Mass = mass;
        }

        public float Size { get; }
        public float Mass { get; }
    }
}
