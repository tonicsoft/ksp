using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KspLaunchToLko
{
    public static class Globals
    {
        public const double KspGravitationalConstant = 6.67430E-11;
        public const double KerbinMass = 5.2915158E22;

        public static readonly OrbitalUtilities KerbinOrbitalUtilities = new OrbitalUtilities(KspGravitationalConstant, KerbinMass);
    }
}
