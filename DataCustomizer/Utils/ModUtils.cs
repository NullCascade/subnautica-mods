using SMLHelper.V2.Handlers;
using System;

namespace DataCustomizer.Utils
{
    /// <summary>
    /// Static utilities class for common functions and properties to be used within your mod code
    /// </summary>
    internal static class ModUtils
    {
        public static TechType ParseTechType(string name)
        {
            if (Enum.TryParse(name, out TechType result))
            {
                return result;
            }
            else if (TechTypeHandler.TryGetModdedTechType(name, out TechType moddedResult))
            {
                return moddedResult;
            }
            throw new Exception($"Could not determine underlying TechType for key '{name}'");
        }

        public static CraftData.BackgroundType ParseBackgroundType(string name)
        {
            if (Enum.TryParse(name, out CraftData.BackgroundType result))
            {
                return result;
            }
            throw new Exception($"Could not determine underlying CraftData.BackgroundType for key '{name}'");
        }

        public static HarvestType ParseHarvestType(string name)
        {
            if (Enum.TryParse(name, out HarvestType result))
            {
                return result;
            }
            throw new Exception($"Could not determine underlying HarvestType for key '{name}'");
        }
    }
}
