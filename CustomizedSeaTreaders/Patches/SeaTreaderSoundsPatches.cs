using HarmonyLib;
using UnityEngine;

namespace CustomizedSeaTreaders.Patches
{
    [HarmonyPatch(typeof(SeaTreaderSounds))]
    internal class SeaTreaderSoundsPatches
    {
        [HarmonyPatch(nameof(SeaTreaderSounds.SpawnChunks))]
        [HarmonyPrefix]
        public static bool SpawnChunks_Prefix(SeaTreaderSounds __instance, Transform legTr)
        {
            // Get our random spawn.
            var spawnPrefab = Plugin.GetRandomSpawn();
            if (spawnPrefab == null)
            {
                return false;
            }

            // Try to find a place to spawn.
            if (!Physics.Raycast(legTr.position + legTr.up * 2f, -legTr.up, out RaycastHit raycastHit, 4f, Voxeland.GetTerrainLayerMask()))
            {
                return false;
            }

            // Instantiate our spawn.
            var instance = Object.Instantiate(spawnPrefab);

            // Move it into place.
            Transform transform = instance.transform;
            transform.position = raycastHit.point;
            transform.rotation = Random.rotation;
            transform.rotation = Quaternion.FromToRotation(transform.up, raycastHit.normal) * transform.rotation;
            Vector2 vector = Random.insideUnitCircle.normalized * __instance.chunkSpawnOffset;
            Vector3 vector2 = new Vector3(vector.x, 0f, vector.y);
            transform.position = transform.TransformPoint(vector2);

            // Prevent normal logic.
            return false;
        }
    }
}