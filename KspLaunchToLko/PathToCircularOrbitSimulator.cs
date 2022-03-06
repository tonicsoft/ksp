using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KspLaunchToLko.RocketSimulator;

namespace KspLaunchToLko
{

    public enum TerminationReason
    {
        Unknown,
        ReachedApoapsisTooEarly
    }
    public class PathToCircularOrbitSimulator
    {
        private readonly RocketSimulator sim;
        private readonly OrbitalUtilities util;
        private readonly double targetOrbitalRadius;

        public PathToCircularOrbitSimulator(RocketSimulator sim, OrbitalUtilities util, double targetOrbitalRadius, double tolerance)
        {
            this.sim = sim;
            this.util = util;
            this.targetOrbitalRadius = targetOrbitalRadius;
        }

        private double calculateApoapsis(OrbitalState stateBeforeApoapsis, OrbitalState stateAfterApoapsis)
        {
            var oldAnomaly = util.trueAnomaly(stateBeforeApoapsis);
            var newAnomaly = util.trueAnomaly(stateAfterApoapsis);

            var interpolationFactor = (Math.PI - oldAnomaly) / (newAnomaly - oldAnomaly);

            var apoapsisPosition = stateBeforeApoapsis.position + interpolationFactor * (stateAfterApoapsis.position - stateBeforeApoapsis.position);

            return apoapsisPosition.magnitude();
        }
        public void simulateCandidateCircularizationBurn(Vector2 initialPosition, Vector2 initialVelocity, Thrust thrust)
        {
            var timestep = 0.1;
            
            var trajectory = new List<OrbitalState>();

            var targetEnergy = util.specificOrbitalEnergyOfCircularOrbit(targetOrbitalRadius);

            var state = new OrbitalState(initialPosition, initialVelocity);
            
            var reachedApoapsis = false;
            double apoapsis = 0;

            while (true)
            {
                trajectory.Add(state);

                var newState = sim.simulateTimeStep(state, thrust, timestep);
            
                if (util.trueAnomaly(newState) > Math.PI && reachedApoapsis == false)
                {
                    // we just passed over an apoapsis
                    reachedApoapsis = true;
                    apoapsis = calculateApoapsis(state, newState);
                }

                // todo: calculate apoapsis by either waiting to pass one (undershooting) or waiting until orbit energy is reached (overshooting).
                // use that as the feedback variable for the multiplier of the thrust function.
                // then figure out a way to measure whether the orbit was circularised or not and use that as the feedback variable
                // for the slope modifier of the thrust function.

                state = newState;


            }
        }
    }
}
