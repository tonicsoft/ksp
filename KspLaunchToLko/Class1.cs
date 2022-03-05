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
        private ToOrbitFlightController mod = null;
        public void Update()
        {
            bool turnModOn = Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Alpha1);

            bool turnModOff = Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Alpha2);



            if (turnModOn)
            {
                mod = new ToOrbitFlightController(80000);
                FlightGlobals.ActiveVessel.OnFlyByWire += mod.onFlyByWire;
            } else if (turnModOff && mod != null)
            {
                FlightGlobals.ActiveVessel.OnFlyByWire -= mod.onFlyByWire;
                mod = null;
            }
        }

    }
}
