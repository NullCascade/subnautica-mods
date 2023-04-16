using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using CustomizedSeaTreaders.Serialization;
using HarmonyLib;
using Newtonsoft.Json;
using SMLHelper.V2.Handlers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UWE;

namespace CustomizedSeaTreaders
{
    public static class IEnumerableExtensions
    {
        public static T RandomElementByWeight<T>(this IEnumerable<T> sequence, Func<T, float> weightSelector)
        {
            var totalWeight = sequence.Sum(weightSelector);
            var itemWeightIndex = UnityEngine.Random.value * totalWeight;
            var currentWeightIndex = 0.0f;

            foreach (var item in from weightedItem in sequence select new { Value = weightedItem, Weight = weightSelector(weightedItem) })
            {
                currentWeightIndex += item.Weight;
                if (currentWeightIndex >= itemWeightIndex)
                {
                    return item.Value;
                }
            }
            return default(T);
        }
    }

    [BepInPlugin(MyGUID, PluginName, VersionString)]
    public class Plugin : BaseUnityPlugin
    {
        private const string MyGUID = "com.NullCascade.CustomizedSeaTreaders";
        private const string PluginName = "CustomizedSeaTreaders";
        private const string VersionString = "1.0.0";

        private static readonly Harmony Harmony = new Harmony(MyGUID);
        public static ManualLogSource Log = new ManualLogSource(PluginName);

        public static float SpawnChance = 1.0f;
        public static Dictionary<GameObject, float> WeightedSpawnChances = new Dictionary<GameObject, float>();

        private void Awake()
        {
            Log = Logger;

            // Apply all of our patches
            Harmony.PatchAll();
            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loaded.");

            StartCoroutine(LoadSpawnSettings());
        }

        public static bool TryParseTechType(string name, out TechType result)
        {
            if (UWE.Utils.TryParseEnum(name, out result))
            {
                return true;
            }
            else if (TechTypeHandler.TryGetModdedTechType(name, out result))
            {
                return true;
            }
            return false;
        }

        public static IEnumerator LoadSpawnSettings()
        {
            var settingsPath = Path.Combine(Paths.PluginPath, "CustomizedSeaTreaders", "SpawnSettings.json");
            var settings = JsonConvert.DeserializeObject<SpawnSettings>(File.ReadAllText(settingsPath));
            SpawnChance = settings.Chance;
            Log.LogDebug($"Settings: {JsonConvert.SerializeObject(settings)}");
            foreach (var spawn in settings.Spawns)
            {
                if (!TryParseTechType(spawn.Key, out TechType techType))
                {
                    Log.LogWarning($"Could not determine TechType for spawn {spawn.Key}.");
                    continue;
                }

                var prefabTask = CraftData.GetPrefabForTechTypeAsync(techType);
                yield return prefabTask;
                var prefab = prefabTask.GetResult();

                Log.LogDebug($"Prefab {prefab.name} will have a weight of {spawn.Value.Weight}");
                WeightedSpawnChances[prefab] = spawn.Value.Weight;
            }
        }

        public static GameObject GetRandomSpawn()
        {
            var roll = UnityEngine.Random.value;
            if (roll > SpawnChance)
            {
                return null;
            }

            if (WeightedSpawnChances.Count == 0)
            {
                return null;
            }

            return WeightedSpawnChances.RandomElementByWeight(e => e.Value).Key;
        }
    }
}
