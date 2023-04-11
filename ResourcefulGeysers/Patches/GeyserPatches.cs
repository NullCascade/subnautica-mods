using HarmonyLib;
using ResourcefulGeysers.MonoBehaviours;

namespace ResourcefulGeysers.Patches
{
    [HarmonyPatch(typeof(Geyser))]
    internal class GeyserPatches
    {
        [HarmonyPatch(nameof(Geyser.Erupt))]
        [HarmonyPostfix]
        public static void Awake_Postfix(Geyser __instance)
        {
            var spawner = GeyserSpawner.EnsureOn(__instance.gameObject);
            spawner.SpawnResources(__instance);
        }
    }
}