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
        private readonly double orbitalRadius;
        private readonly double finalSpeed;
        private readonly OrbitalUtilities util;

        public PathToOrbitSimulator(double orbitalRadius, OrbitalUtilities util)
        {
            this.orbitalRadius = orbitalRadius;
            this.util = util;
            
            finalSpeed = util.circularOrbitSpeed(orbitalRadius);
        }

        public List<OrbitalState> runForwardsSimulation(Vector2 initialPosition, Vector2 initialVelocity, Func<Vector2, Vector2, Vector2> thrust)
        {
            OrbitalState currentState = new OrbitalState(initialPosition, initialVelocity);
            List<OrbitalState> result = new List<OrbitalState>();
            
            for (int i = 0; i < 3000; i++)
            {
                Func<Vector2, Vector2, Vector2> acceleration = (Vector2 position, Vector2 velocity) => util.accelerationDueToGravity(position) + thrust(position, velocity);

                var nextState = computeRk4Iteration(currentState, 1, acceleration);
                
                if (util.specificOrbitalEnergy(nextState.position, nextState.velocity) > util.specificOrbitalEnergyOfCircularOrbit(1000000))
                {
                    break;
                }

                currentState = nextState;
                result.Add(nextState);
            }

            return result;
        }

        public List<OrbitalState> runBackwardsSimulation(Func<Vector2, Vector2, Vector2> thrust)
        {
            Vector2 finalPosition = new Vector2(orbitalRadius, 0);
            Vector2 finalVelocity = new Vector2(0, -finalSpeed);

            OrbitalState currentState = new OrbitalState(finalPosition, finalVelocity);
            List<OrbitalState> result = new List<OrbitalState>();
            // due to numerical issues r may increase in the first steps of the sim
            bool rShouldBeDecreasing = false;
            bool aShouldBeDecreasing = false;

            for (int i = 0; i < 3000; i++)
            {
                Func<Vector2, Vector2, Vector2> acceleration = (Vector2 position, Vector2 velocity) => util.accelerationDueToGravity(position) + thrust(position, velocity);

                var previousState = computeRk4Iteration(currentState, -1, acceleration);
                /*
                // heuristic to prevent the rocket turning around
                if (accelerationDueToThrust.magnitude() > 2 * currentState.velocity.magnitude())
                {
                    break;
                }
                */
                // heuristic to stop when the orbit becomes highly eccentric
                if (util.eccentricity(currentState.position, currentState.velocity) > 0.999)
                {
                    break;
                }
                // heuristic to stop when we reach periapsis
                if (currentState.position.magnitude() > previousState.position.magnitude())
                {
                    rShouldBeDecreasing = true;
                } else // r is now increasing
                {
                    if (rShouldBeDecreasing)
                    {
                        break;
                    }
                }
                // heuristic to stop when semi major axis gets silly
                if (util.semiMajorAxis(currentState.position, currentState.velocity) >
                    util.semiMajorAxis(previousState.position, previousState.velocity))
                {
                    aShouldBeDecreasing = true;
                }
                else
                {
                    if (aShouldBeDecreasing)
                    {
                        break;
                    }
                }
                
                currentState = previousState;
                result.Add(previousState);
            }

            return result;
        }

        public static Vector2 computeThrust(OrbitalState currentState)
        {
            // constant thrust in the direction of motion
            return currentState.velocity.unit() * 30;
        }

        private OrbitalState computeSimpleEulerIteration(OrbitalState currentState, double timeStep, Func<Vector2, Vector2> acceleration)
        {
            Vector2 nextPosition = currentState.position + currentState.velocity * timeStep + 0.5 * acceleration(currentState.position) * timeStep * timeStep;

            Vector2 nextVelocity = currentState.velocity + acceleration(currentState.position) * timeStep;

            return new OrbitalState(nextPosition, nextVelocity);
        }

        // acceleration is a function of position and velocity, a(r, v)
        private OrbitalState computeRk4Iteration(OrbitalState currentState, double h, Func<Vector2, Vector2, Vector2> acceleration)
        {
            var r0 = currentState.position;
            var v0 = currentState.velocity;
            var a0 = acceleration(r0, v0);

            var k1r = v0;
            var k1v = a0;

            var k2r = v0 + 0.5 * h * k1v;
            var k2v = acceleration(r0 + 0.5 * h * k1r, k2r);

            var k3r = v0 + 0.5 * h * k2v;
            var k3v = acceleration(r0 + 0.5 * h * k2r, k3r);

            var k4r = v0 + h * k3v;
            var k4v = acceleration(r0 + h * k3r, k4r);

            var r1 = r0 + (h / 6) * (k1r + 2 * k2r + 2 * k3r + k4r);
            var v1 = v0 + (h / 6) * (k1v + 2 * k2v + 2 * k3v + k4v);

            return new OrbitalState(r1, v1);
        }
    }
}
