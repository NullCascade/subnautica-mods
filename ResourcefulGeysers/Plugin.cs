using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using ResourcefulGeysers.Utils;
using UnityEngine;
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

        private void Awake()
        {
            Log = Logger;
            Harmony.PatchAll();
            CoroutineHost.StartCoroutine(PrefabUtils.GetPrefabs());
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
