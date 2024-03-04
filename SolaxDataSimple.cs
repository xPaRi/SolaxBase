using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solax.SolaxBase
{
    public class SolaxDataSimple
    {
        public SolaxDataSimple(SolaxData? result)
        {
            if (result == null)
                return;

            PVAllPower = result.PVAllPower.Value;
            PV1Power = result.PV1Power.Value;
            PV2Power = result.PV2Power.Value;
            BatteryPower = result.BatteryPower.Value;
            BatteryRemainingCapacity = result.BatteryRemainingCapacity.Value;
            PowerNow = result.PowerNow.Value;
            ExportedPower = result.ExportedPower.Value;
        }

        public decimal PVAllPower { get; set; }
        public decimal PV1Power { get; set; }
        public decimal PV2Power { get; set; }
        public decimal BatteryPower { get; set; }
        public decimal BatteryRemainingCapacity { get; set; }
        public decimal PowerNow { get; set; }
        public decimal ExportedPower { get; set; }

    }
}
