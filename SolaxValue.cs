using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solax.SolaxBase
{
    public class SolaxValue
    {
        public static SolaxValue GetValue(SolaxData data, int index, string name, UnitsEnum? unit, OperationsEnum operation )
        {
            switch (operation)
            {
                case OperationsEnum.div10:
                    return new SolaxValue(index, name, unit, data.Data[index] / 10m);

                case OperationsEnum.twoway_div10:
                    {
                        decimal result = data.Data[index] / 10.0m;
                        result = Math.Round(result, 2, MidpointRounding.ToEven);
                        result /= 10.0m;
                        return new SolaxValue(index, name, unit, result);
                    }

                case OperationsEnum.twoway_div100:
                    {
                        decimal result = data.Data[index] / 100.0m;
                        result = Math.Round(result, 2, MidpointRounding.ToEven);
                        result /= 100.0m;
                        return new SolaxValue(index, name, unit, result);
                    }

                case OperationsEnum.div100:
                    return new SolaxValue(index, name, unit, data.Data[index] / 100m);

                case OperationsEnum.to_signed:
                    return new SolaxValue(index, name, unit, data.Data[index]);

                default:
                    return new SolaxValue(index, name, unit, data.Data[index]);
            }
        }


        public SolaxValue(int index, string name, UnitsEnum? unit, decimal value)
        {
            this.Index = index;
            this.Name = name;
            this.Unit = unit;
            this.Value = value;
        }

        public int Index { get; }
        public string Name { get; }
        public UnitsEnum? Unit { get; }
        public decimal Value { get; }

        public override string ToString()
        {
            switch(Unit)
            {
                case UnitsEnum.INVERTER_OPERATION_MODE:
                    return $"{Name} = {InverterModesDict.GetModeName(Convert.ToInt32(Value))}";
                case UnitsEnum.BATTERY_OPERATION_MODE:
                    return $"{Name} = {BatteryModesDict.GetModeName(Convert.ToInt32(Value))}";
                case UnitsEnum.PERCENT:
                    return $"{Name} = {Value} %";
                case UnitsEnum.C:
                    return $"{Name} = {Value} °C";
                default:
                    return $"{Name} = {Value} {Unit}";
            }
        }
    }
}
