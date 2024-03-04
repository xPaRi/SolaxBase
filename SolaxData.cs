using System.Text.Json.Serialization;

namespace Solax.SolaxBase
{
    // 
    public class SolaxData
    {
        [JsonPropertyName("sn")]
        public string SerialNumber { get; set; } = string.Empty;

        [JsonPropertyName("ver")]
        public string Version { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("Data")]
        public List<int> Data { get; set; } = new List<int>();

        [JsonPropertyName("Information")]
        public List<object> Information { get; set; } = new List<object>();

        /// <summary>
        /// Seznam hodnot jako slovník.
        /// </summary>
        public Dictionary<int, SolaxValue> ValueDict
        {
            get 
            {
                _ValueDict ??= Decode();

                return _ValueDict;
            }
        }

        private Dictionary<int, SolaxValue>? _ValueDict = null;

        private Dictionary<int, SolaxValue> Decode()
        {
            var result = new Dictionary<int, SolaxValue>(200);

            Add(result, 0, "Network Voltage Phase 1", UnitsEnum.V, OperationsEnum.div10);
            Add(result, 1, "Network Voltage Phase 2", UnitsEnum.V, OperationsEnum.div10);
            Add(result, 2, "Network Voltage Phase 3", UnitsEnum.V, OperationsEnum.div10);
            
            Add(result, 3, "Output Current Phase 1", UnitsEnum.A, OperationsEnum.twoway_div10);
            Add(result, 4, "Output Current Phase 2", UnitsEnum.A, OperationsEnum.twoway_div10);
            Add(result, 5, "Output Current Phase 3", UnitsEnum.A, OperationsEnum.twoway_div10);

            Add(result, 6, "Power Now Phase 1", UnitsEnum.W, OperationsEnum.to_signed);
            Add(result, 7, "Power Now Phase 2", UnitsEnum.W, OperationsEnum.to_signed);
            Add(result, 8, "Power Now Phase 3", UnitsEnum.W, OperationsEnum.to_signed);

            Add(result, 9, "AC Power", UnitsEnum.W, OperationsEnum.to_signed);

            Add(result, 10, "PV1 Voltage", UnitsEnum.V, OperationsEnum.div10);
            Add(result, 11, "PV2 Voltage", UnitsEnum.V, OperationsEnum.div10);

            Add(result, 12, "PV1 Current", UnitsEnum.A, OperationsEnum.div10);
            Add(result, 13, "PV2 Current", UnitsEnum.A, OperationsEnum.div10);

            Add(result, 14, "PV1 Power", UnitsEnum.W, OperationsEnum.NONE);
            Add(result, 15, "PV2 Power", UnitsEnum.W, OperationsEnum.NONE);

            Add(result, 16, "Grid Frequency Phase 1", UnitsEnum.Hz, OperationsEnum.div100);
            Add(result, 17, "Grid Frequency Phase 2", UnitsEnum.Hz, OperationsEnum.div100);
            Add(result, 18, "Grid Frequency Phase 3", UnitsEnum.Hz, OperationsEnum.div100);
            
            Add(result, 19, "Inverter Operation mode", UnitsEnum.INVERTER_OPERATION_MODE, OperationsEnum.NONE);

            //# 20 - 32: always 0
            //# 33     : always 1

            Add(result, 34, "Exported Power (value)", UnitsEnum.W, OperationsEnum.to_signed);
            Add(result, 35, "Exported Power (sing)", UnitsEnum.W, OperationsEnum.to_signed);
            //# instead of to_signed this is actually 34 - 35,
            //# because 35 =  if 34>32767: 0 else: 65535
            //# 35: if 34>32767: 0 else: 65535

            //# 36 - 38: always  0
            Add(result, 39, "Battery Voltage", UnitsEnum.V, OperationsEnum.div100);
            Add(result, 40, "Battery Current", UnitsEnum.A, OperationsEnum.twoway_div100);
            Add(result, 41, "Battery Power (41)", UnitsEnum.W, OperationsEnum.to_signed);
            Add(result, 44, "Battery Power (44)", UnitsEnum.W, OperationsEnum.to_signed);

            //# 42: div10, almost identical to [39]
            //# 43: twoway_div10,  almost the same as "40" (battery current)
            //# 44: twoway_div100, almost the same as "41" (battery power),
            //# 45: always 1
            //# 46: follows PV Output, idles around 44, peaks at 52,
            Add(result, 47, "Power Now", UnitsEnum.W, OperationsEnum.to_signed);
            //# 48: always 256
            //# 49,50: [49] + [50] * 15160 some increasing counter
            //# 51: always 5634
            //# 52: always 100
            //# 53: always 0
            //# 54: follows PV Output, idles around 35, peaks at 54,
            //# 55-67: always 0
            //"Total Energy": (pack_u16(68, 69), Total(Units.KWH), div10),
            //# 70: div10, today's energy including battery usage
            //# 71-73: 0
            Add(result, 92, "Today's Consumption", UnitsEnum.KWh, OperationsEnum.div100);
            //# 93-101: always 0
            //# 102: always 1
            Add(result, 103, "Battery Remaining Capacity", UnitsEnum.PERCENT, OperationsEnum.NONE);
            //# 104: always 1
            Add(result, 105, "Battery Temperature", UnitsEnum.C, OperationsEnum.NONE);
            Add(result, 106, "Battery Remaining Energy", UnitsEnum.KWh, OperationsEnum.div10);
            //# 107: always 256 or 0
            //# 108: always 3504
            //# 109: always 2400

            Add(result, 168, "Battery Operation mode", UnitsEnum.BATTERY_OPERATION_MODE, OperationsEnum.NONE);

            return result;
        }

        private void Add(Dictionary<int, SolaxValue> dict, int index, string name, UnitsEnum? unit, OperationsEnum operation)
        {
            var value = SolaxValue.GetValue(this, index, name, unit, operation);

            dict.Add(index, value);
        }

        public SolaxValue NetworkVoltagePhase1 => ValueDict[0];
        
        public SolaxValue ExportedPower => new SolaxValue(-1, "Exported Power", UnitsEnum.W, ValueDict[34].Value - ValueDict[35].Value);
        public SolaxValue PVAllPower => new SolaxValue(-1, "All Panel Power", UnitsEnum.W, ValueDict[14].Value + ValueDict[15].Value);
        public SolaxValue BatteryPower
        {
            get
            {
                var value = ValueDict[41].Value;
                var sign = (value < 32767) ? 0 : 65535;

                return new SolaxValue(-1, "BatteryPower", UnitsEnum.W, ValueDict[41].Value - sign);
            }
        }

        public SolaxValue BatteryRemainingCapacity => ValueDict[103];
        public SolaxValue PowerNow => new SolaxValue(-1, "BatteryPower", UnitsEnum.W, -ValueDict[47].Value);

        public SolaxValue PV1Power => new SolaxValue(-1, "PV1 Power", UnitsEnum.W, ValueDict[14].Value);
        public SolaxValue PV2Power => new SolaxValue(-1, "PV2 Power", UnitsEnum.W, ValueDict[15].Value);

        /*

        "Total Battery Discharge Energy": (
            pack_u16(74, 75),
            Total(Units.KWH),
            div10,
        ),
        "Total Battery Charge Energy": (
            pack_u16(76, 77),
            Total(Units.KWH),
            div10,
        ),
        "Today's Battery Discharge Energy": (78, Units.KWH, div10),
        "Today's Battery Charge Energy": (79, Units.KWH, div10),
        "Total PV Energy": (pack_u16(80, 81), Total(Units.KWH), div10),
        "Today's Energy": (82, Units.KWH, div10),
        # 83-85: always 0
        "Total Feed-in Energy": (pack_u16(86, 87), Total(Units.KWH), div100),
        "Total Consumption": (pack_u16(88, 89), Total(Units.KWH), div100),
        "Today's Feed-in Energy": (90, Units.KWH, div100),
        # 91: always 0
        
        # 110: around rise to 300 if battery not full, 0 if battery is full
        # 112, 113: range [250,350]; looks like 113 + offset = 112,
        #   peaks if battery is full
        # 114, 115: something around 33; Some temperature?!
        # 116: increases slowly [2,5]
        # 117-121: 1620	773	12850	12850	12850
        # 122-124: always 0
        # 125,126: some curve, look very similar to "42"(Battery Power),
        # with offset around 15
        # 127,128 resetting counter /1000, around battery charge + discharge
        # 164,165,166 some curves
        # 169: div100 same as [39]
        # 170-199: always 0     



 */

    }


}