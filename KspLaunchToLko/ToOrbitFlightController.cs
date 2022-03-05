using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KspLaunchToLko
{
    class ToOrbitFlightController
    {
        private readonly double altitude;
        private IFlightControlState state;

        public ToOrbitFlightController(double altitude)
        {
            this.altitude = altitude;
            state = new VerticalAscent();
        }

        public void onFlyByWire(FlightCtrlState s)
        {
            state = state.onFlyByWire(s);
        }
    }

    public interface IFlightControlState
    {
        IFlightControlState onFlyByWire(FlightCtrlState s);
    }

    public class VerticalAscent : IFlightControlState
    {
        private double maxVelocity = 300;
        public IFlightControlState onFlyByWire(FlightCtrlState s)
        {
            var v = FlightGlobals.ActiveVessel;

            var deltaV = maxVelocity - v.srfSpeed;


            if (deltaV > 0)
            {
                s.mainThrottle = 1;
            } else
            {
                s.mainThrottle = 0;
            }

            if (v.altitude > 35000)
            {
                return new Circularize();
            } else
            {
                return this;
            }
        }
    }

    public class Circularize : IFlightControlState
    {
        public void initialise()
        {
            var util = Globals.KerbinOrbitalUtilities;

            var sim = new RocketSimulator(util);



        }
        public IFlightControlState onFlyByWire(FlightCtrlState s)
        {
            var v = FlightGlobals.ActiveVessel;

            var altitude = v.GetCurrentOrbit().ApA;

            if (altitude < 75000)
            {
                s.mainThrottle = 1;
            } else
            {
                s.mainThrottle = 0;
            }

            return this;
        }
    }
}
