namespace Solax.SolaxBase
{
    public class BatteryModesDict : Dictionary<int, string>
    {
        public static BatteryModesDict Modes = new() 
        { 
            { 0, "Self Use Mode" },
            { 1, "Force Time Use" }, 
            { 2, "Back Up Mode" }, 
            { 3, "Feed-in Priority" }, 
        };

        public static string GetModeName(int mode)
        {
            if (Modes.TryGetValue(mode, out var name))
                return name;

            return "?";
        }
    }
}
