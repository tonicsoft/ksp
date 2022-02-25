using KspLaunchToLko;
using NUnit.Framework;
using System.IO;

namespace Tests
{
    public class Tests
    {
        private const double GravitationalConstant = 6.67E-11;

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void simulateCircularOrbit()
        {
            var util = Globals.KerbinOrbitalUtilities;

            var radius = 80000;

            var initialPosition = new Vector2(0, radius);

            var initialVelocity = new Vector2(-util.circularOrbitSpeed(radius), 0);

            var sim = new PathToOrbitSimulator(util);

            var result = sim.runSimulation(initialPosition, initialVelocity, (p, v) => Vector2.ZERO, 1, (time, state) => time < 3000);

            var maxR = initialPosition.magnitude();
            var minR = initialPosition.magnitude();
            foreach (var state in result)
            {
                var r = state.position.magnitude();
                if (r > maxR)
                {
                    maxR = r;
                }

                if (r < minR)
                {
                    minR = r;
                }
            }

            Assert.That(maxR, Is.InRange(radius, radius + 1));
            Assert.That(minR, Is.InRange(radius -3, radius));
        }

        [Test]
        public void simulateRealisticAscent()
        {
            const int targetOrbitalRadius = 750000;

            var util = Globals.KerbinOrbitalUtilities;

            var initialPosition = new Vector2(-638947, 36691);
            var initialVelocity = new Vector2(-593, -500);
            var initialEnergy = util.specificOrbitalEnergy(initialPosition, initialVelocity);

            var finalPosition = new Vector2(targetOrbitalRadius, 0);
            var finalVelocity = new Vector2(0, -util.circularOrbitSpeed(targetOrbitalRadius));

            var sim = new PathToOrbitSimulator(util);

            using (var writer = new StreamWriter("plot.txt"))
            {
                using (var positionWriter = new StreamWriter("position-plot.txt"))
                {
                    using (var otherPositionWriter = new StreamWriter("other-position-plot.txt"))
                    {
                        for (double i = 5; i < 30; i += 2)
                        {
                            var backwardsResult = sim.runSimulation(finalPosition, finalVelocity, (position, velocity) => velocity.unit() * i,
                                -1,
                                (time, state) => util.specificOrbitalEnergy(state) > initialEnergy   
                                );
                            var forwardsResult = sim.runSimulation(initialPosition, initialVelocity,
                                (position, velocity) => velocity.unit() * i,
                                1,
                                (time, state) =>
                                util.specificOrbitalEnergy(state.position, state.velocity) < util.specificOrbitalEnergyOfCircularOrbit(1000000)
                                );

                            foreach (var item in backwardsResult)
                            {
                                writer.WriteLine(
                                    util.semiMajorAxis(item.position, item.velocity) + ","
                                    + util.eccentricity(item.position, item.velocity) + ","
                                    + item.position.magnitude() + ","
                                    + item.velocity.magnitude()
                                    );
                                positionWriter.WriteLine(item.position.x + "," + item.position.y);
                            }

                            writer.WriteLine("");

                            foreach (var item in forwardsResult)
                            {
                                writer.WriteLine(
                                    util.semiMajorAxis(item.position, item.velocity) + ","
                                    + util.eccentricity(item.position, item.velocity) + ","
                                    + item.position.magnitude() + ","
                                    + item.velocity.magnitude()
                                    );
                                otherPositionWriter.WriteLine(item.position.x + "," + item.position.y);
                            }

                            writer.WriteLine("");
                            positionWriter.WriteLine("");
                            otherPositionWriter.WriteLine("");
                        }
                    }
                }
            }

            Assert.Pass();
        }
    }
}