using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KspLaunchToLko
{
    public class OrbitalUtilities
    {
        public readonly double G;
        public readonly double M;
        public readonly double μ;

        public OrbitalUtilities(double gravitationalConstant, double planetMass)
        {
            G = gravitationalConstant;
            M = planetMass;
            μ = G * M;
        }

        public double specificOrbitalEnergy(Vector2 position, Vector2 velocity)
        {
            double kineticEnergy = 0.5 * velocity.squareMagnitude();

            double potentialEnergy = - μ / position.magnitude();

            return kineticEnergy + potentialEnergy;
        }

        public double specificOrbitalEnergyOfCircularOrbit(double radius)
        {
            double kineticEnergy = 0.5 * circularOrbitSpeed(radius) * circularOrbitSpeed(radius);

            double potentialEnergy = - μ / radius;

            return kineticEnergy + potentialEnergy;
        }

        public Vector2 accelerationDueToGravity(Vector2 position) => -(μ / Math.Pow(position.squareMagnitude(), 1.5)) * position;

        public double circularOrbitSpeed(double radius)
        {
            return Math.Sqrt(μ / radius);
        }

        public Vector2 eccentricityVector(Vector2 position, Vector2 velocity)
        {
            return (velocity.squareMagnitude() / μ - 1/position.magnitude()) * position - (position.dot(velocity)/μ) * velocity;
        }

        public double eccentricity(Vector2 position, Vector2 velocity) => eccentricityVector(position, velocity).magnitude();

        public double semiMajorAxis(Vector2 position, Vector2 velocity)
        {
            return -μ / (2 * specificOrbitalEnergy(position, velocity));
        }
    }
}
