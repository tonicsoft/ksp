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
        public void Test1()
        {

            const double PlanetMass = 5.29152E+22;

            const int targetOrbitalRadius = 750000;

            var util = new OrbitalUtilities(GravitationalConstant, PlanetMass);

            var sim = new PathToOrbitSimulator(targetOrbitalRadius, util);

            var initialPosition = new Vector2(-638947, 36691);
            var initialVelocity = new Vector2(-593, -500);

            using (var writer = new StreamWriter("plot.txt"))
            {
                using (var positionWriter = new StreamWriter("position-plot.txt"))
                {
                    using (var otherPositionWriter = new StreamWriter("other-position-plot.txt"))
                    {
                        for (double i = 5; i < 30; i += 2)
                        {
                            var result = sim.runBackwardsSimulation((position, velocity) => velocity.unit() * i);
                            var forwardsResult = sim.runForwardsSimulation(initialPosition, initialVelocity, (position, velocity) => velocity.unit() * i);

                            foreach (var item in result)
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