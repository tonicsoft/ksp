using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KspLaunchToLko
{
    public class OrbitalState
    {
        public readonly Vector2 position;
        public readonly Vector2 velocity;

        public OrbitalState(Vector2 position, Vector2 velocity)
        {
            this.position = position;
            this.velocity = velocity;
        }
    }

    // Creates a numerical simulation of a possible rocket trajectory that ends in a circular orbit. 
    // It does so by starting with the equations of motion of an object in circular orbit and simulating
    // time in reverse
    public class PathToOrbitSimulator
    {
        private readonly OrbitalUtilities util;

        public PathToOrbitSimulator(OrbitalUtilities util)
        {
            this.util = util;
        }

        public delegate Vector2 Thrust(Vector2 position, Vector2 velocity);
        public delegate bool ContinueSimulation(double time, OrbitalState orbitalState);

        public List<OrbitalState> runSimulation(Vector2 initialPosition, Vector2 initialVelocity, Thrust thrust, double timestep, ContinueSimulation cont)
        {
            var currentTime = 0.0;
            OrbitalState currentState = new OrbitalState(initialPosition, initialVelocity);
            List<OrbitalState> result = new List<OrbitalState>();
            
            while (cont(currentTime, currentState))
            {
                result.Add(currentState);

                Vector2 acceleration(Vector2 r, Vector2 v) => util.accelerationDueToGravity(r) + thrust(r, v);

                currentState = computeRk4Iteration(currentState, timestep, acceleration);
                currentTime += timestep;
            }

            return result;
        }

        private delegate Vector2 Acceleration(Vector2 position, Vector2 velocity);
        private OrbitalState computeRk4Iteration(OrbitalState currentState, double h, Acceleration a)
        {
            var r0 = currentState.position;
            var v0 = currentState.velocity;
            var a0 = a(r0, v0);

            var k1r = v0;
            var k1v = a0;

            var k2r = v0 + 0.5 * h * k1v;
            var k2v = a(r0 + 0.5 * h * k1r, k2r);

            var k3r = v0 + 0.5 * h * k2v;
            var k3v = a(r0 + 0.5 * h * k2r, k3r);

            var k4r = v0 + h * k3v;
            var k4v = a(r0 + h * k3r, k4r);

            var r1 = r0 + (h / 6) * (k1r + 2 * k2r + 2 * k3r + k4r);
            var v1 = v0 + (h / 6) * (k1v + 2 * k2v + 2 * k3v + k4v);

            return new OrbitalState(r1, v1);
        }
    }
}
