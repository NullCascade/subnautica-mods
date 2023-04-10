using HarmonyLib;
using ResourcefulGeysers.Utils;
using System;
using System.Collections;
using UnityEngine;
using UWE;
using static RootMotion.FinalIK.Grounding;
using static VFXParticlesPool;

namespace ResourcefulGeysers.Patches
{
    public static class GeyserExtensions
    {
        public static float ChunkChance { get; set; } = 0.1f;
        public static int MinChunkCount { get; set; } = 10;
        public static int MaxChunkCount { get; set; } = 10;
        public static float MaxSpawnOffset { get; set; } = 2.0f;
        public static float MaxSpawnVelocity { get; set; } = 100.0f;
        public static Queue SpawnedObjects = new Queue();
        public static int MaxSpawnedObjects = 10;

        public static void SpawnResources(this Geyser geyser)
        {
            if (!geyser.erupting)
            {
                return;
            }

            int count = UnityEngine.Random.Range(MinChunkCount, MaxChunkCount + 1);
            if (count <= 0)
            {
                Plugin.Log.LogDebug($"No spawn. Rolled 0 chunks.");
                return;
            }

            for (int i = 0; i < count; i++)
            {
                if (UnityEngine.Random.value > ChunkChance)
                {
                    continue;
                }

                var prefab = PrefabUtils.ShaleChunk;
                var instance = UnityEngine.Object.Instantiate(prefab);
                Transform transform = instance.transform;
                transform.position = geyser.transform.position + new Vector3(0f, 10f, 0f);
                transform.rotation = UnityEngine.Random.rotation;
                Vector2 vector = UnityEngine.Random.insideUnitCircle.normalized * MaxSpawnOffset;
                Vector3 position = new Vector3(vector.x, 0f, vector.y);
                transform.position = transform.TransformPoint(position);

                var rigidBody = instance.GetComponent<Rigidbody>();
                rigidBody.isKinematic = false;

                var velocity = UnityEngine.Random.onUnitSphere * MaxSpawnVelocity;
                velocity.y = System.Math.Abs(velocity.y);
                rigidBody.velocity = velocity;

                instance.SetActive(true);

                SpawnedObjects.Enqueue(instance);
            }

            Plugin.Log.LogDebug($"Spawned {count} chunks at {geyser.transform.position}");

            // Cleanup.
            var countBefore = SpawnedObjects.Count;
            while (SpawnedObjects.Count > MaxSpawnedObjects)
            {
                var instance = (GameObject)SpawnedObjects.Dequeue();
                UnityEngine.Object.Destroy(instance);
            }

            if (SpawnedObjects.Count != countBefore)
            {
                Plugin.Log.LogDebug($"Cleaned up {countBefore - SpawnedObjects.Count} spawns.");
            }
        }
    }

    [HarmonyPatch(typeof(Geyser))]
    internal class GeyserPatches
    {
        [HarmonyPatch(nameof(Geyser.Erupt))]
        [HarmonyPostfix]
        public static void Awake_Postfix(Geyser __instance)
        {
            __instance.SpawnResources();
        }
    }
}