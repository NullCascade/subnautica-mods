using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace ImmersiveTimeCapsules
{
    [BepInPlugin(MyGUID, PluginName, VersionString)]
    public class Plugin : BaseUnityPlugin
    {
        private const string MyGUID = "com.NullCascade.ImmersiveTimeCapsules";
        private const string PluginName = "Immersive Time Capsules";
        private const string VersionString = "1.0.0";

        private static readonly Harmony Harmony = new Harmony(MyGUID);
        public static ManualLogSource Log = new ManualLogSource(PluginName);

        private void Awake()
        {
            Log = Logger;
            Harmony.PatchAll();
            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loaded.");
        }
    }
}
