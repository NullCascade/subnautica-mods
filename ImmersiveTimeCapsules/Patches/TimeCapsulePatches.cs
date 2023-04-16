using HarmonyLib;

namespace ImmersiveTimeCapsules.Patches
{
    [HarmonyPatch(typeof(TimeCapsule))]
    internal class TimeCapsulePatches
    {
        [HarmonyPatch(nameof(TimeCapsule.Start))]
        [HarmonyPrefix]
        public static bool Start_Prefix(TimeCapsule __instance)
        {
            __instance.spawned = false;
            Plugin.Log.LogMessage($"Destroying time capsule at {__instance.gameObject.transform.position}");
            UnityEngine.Object.Destroy(__instance.gameObject);
            return false;
        }
    }
}