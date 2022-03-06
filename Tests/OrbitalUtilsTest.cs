using KspLaunchToLko;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class OrbitalUtilsTest
    {
        [Test]
        public void flightPathAngle()
        {
            var util = Globals.KerbinOrbitalUtilities;

            double radius = 750000;
            double speed = util.circularOrbitSpeed(radius);

            var position = new Vector2(0, radius);
            var velocity = new Vector2(speed, 0);

            var levelFlight = util.flightPathAngle(new OrbitalState(position, velocity));

            Assert.IsTrue(Math.Abs(levelFlight) < 1E-7);

            var straightUp = util.flightPathAngle(new OrbitalState(position, new Vector2(0, speed)));

            Assert.IsTrue(Math.Abs(straightUp - Math.PI / 2) < 1E-7);

            var pointingUp = util.flightPathAngle(new OrbitalState(position, velocity + new Vector2(0, 1)));

            Assert.IsTrue(pointingUp > 0);

            var pointingDown = util.flightPathAngle(new OrbitalState(position, velocity + new Vector2(0, -10)));

            Assert.IsTrue(pointingDown > 0);
        }

        [Test]
        public void trueAnomaly()
        {
            var util = Globals.KerbinOrbitalUtilities;

            double radius = 750000;
            double speed = util.circularOrbitSpeed(radius);

            var position = new Vector2(0, radius);
            var velocity = new Vector2(speed, 0);

            var levelFlight = util.trueAnomaly(new OrbitalState(position, velocity));

            Assert.AreEqual(levelFlight, Math.PI);

            var straightUp = util.trueAnomaly(new OrbitalState(position, new Vector2(0, speed)));

            Assert.AreEqual(straightUp, Math.PI);

            // after a prograde burn we should be just after periapsis
            var afterProgradeBurn = util.trueAnomaly(new OrbitalState(position + 0.1 * velocity, 1.1 * velocity));

            Assert.IsTrue(afterProgradeBurn > 0);
            Assert.IsTrue(Math.PI / 2 > afterProgradeBurn);

            // after a retrograde burn we shoudl be just after apopsis, but because of the crude numerical errors it will appear just before
            var afterRetrogradeBurn= util.trueAnomaly(new OrbitalState(position + 0.1 * velocity, 0.9 * velocity));

            Assert.IsTrue(afterRetrogradeBurn > 2 * Math.PI / 4);
            Assert.IsTrue(afterRetrogradeBurn < Math.PI);
        }
    }
}
