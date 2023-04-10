using System;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using Newtonsoft.Json;
using HarmonyLib;

namespace DataCustomizer
{
    // TODO Review this file and update to your own requirements.

    [BepInPlugin(MyGUID, PluginName, VersionString)]
    public class Plugin : BaseUnityPlugin
    {
        private const string MyGUID = "com.NullCascade.DataCustomizer";
        private const string PluginName = "DataCustomizer";
        private const string VersionString = "1.0.0";

        private static readonly Harmony Harmony = new Harmony(MyGUID);
        public static ManualLogSource Log = new ManualLogSource(PluginName);

        /// <summary>
        /// Initialise the configuration settings and patch methods
        /// </summary>
        private void Awake()
        {
            Log = Logger;
            Logger.LogInfo($"Plugin v{VersionString} is loading...");

            Harmony.PatchAll();
            LoadData();

            Logger.LogInfo($"Loading complete.");
        }

        private static void LoadData()
        {
            var customizers = new List<Customization>();

            var dataDir = Path.Combine(Paths.PluginPath, "DataCustomizer", "data");
            var files = Directory.GetFiles(dataDir, "*.json");
            foreach (var file in files)
            {
                try
                {
                    Plugin.Log.LogDebug($"Parsing content file: {file}");
                    customizers.Add(JsonConvert.DeserializeObject<Customization>(File.ReadAllText(file)));
                }
                catch (Exception e)
                {
                    Plugin.Log.LogError($"Could not load customizer '{file}': {e.Message}");
                }
            }

            customizers.Sort(delegate (Customization a, Customization b)
            {
                return a.Priority.CompareTo(b.Priority);
            });

            foreach (var customizer in customizers)
            {
                try
                {
                    customizer.ApplyData();
                }
                catch (Exception e)
                {
                    Plugin.Log.LogError($"Could not apply customization: {e.Message}");
                }
            }
        }

        /// <summary>
        /// Code executed every frame. See below for an example use case
        /// to detect keypress via custom configuration.
        /// </summary>
        // TODO - Add your code here or remove this section if not required.
        private void Update()
        {

        }

        /// <summary>
        /// Method to handle changes to configuration made by the player
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfigSettingChanged(object sender, System.EventArgs e)
        {
            // Check if null and return
            if (!(e is SettingChangedEventArgs))
            {
                return;
            }
        }
    }
}
