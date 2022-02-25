using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KspLaunchToLko
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class Class1 : MonoBehaviour 
    {
        public void Update()
        {
            bool turnModOn = Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Alpha1);

            bool turnModOff = Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Alpha2);



            if (turnModOn)
            {
                FlightGlobals.ActiveVessel.OnFlyByWire += FlightController;
            } else if (turnModOff)
            {
                FlightGlobals.ActiveVessel.OnFlyByWire -= FlightController;
            }
        }

        public void FlightController(FlightCtrlState s)
        {
            s.mainThrottle = 1;
        }

    }
}
