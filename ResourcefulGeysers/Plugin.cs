using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using ResourcefulGeysers.Utils;
using UWE;

namespace ResourcefulGeysers
{
    [BepInPlugin(MyGUID, PluginName, VersionString)]
    public class Plugin : BaseUnityPlugin
    {
        private const string MyGUID = "com.NullCascade.ResourcefulGeysers";
        private const string PluginName = "ResourcefulGeysers";
        private const string VersionString = "1.0.0";

        private static readonly Harmony Harmony = new Harmony(MyGUID);
        public static ManualLogSource Log = new ManualLogSource(PluginName);

        public static bool Enabled = true;
        public static float ChunkChance { get; set; } = 0.1f;
        public static int MinChunkCount { get; set; } = 10;
        public static int MaxChunkCount { get; set; } = 10;
        public static float MaxSpawnOffset { get; set; } = 2.0f;
        public static float MaxSpawnVelocity { get; set; } = 100.0f;
        public static int MaxSpawnedObjects = 10;
        public static double SpawnLifetime = 60.0;

        private void Awake()
        {
            Log = Logger;
            Harmony.PatchAll();
            CoroutineHost.StartCoroutine(PrefabUtils.GetPrefabs());

            SerializerUtils.RegisterSerializer<MonoBehaviours.GeyserSpawn>();
        }

        /// <summary>
        /// Method to handle changes to configuration made by the player
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfigSettingChanged(object sender, System.EventArgs e)
        {

        }
    }
}
