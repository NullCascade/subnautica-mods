using System;
using System.Collections.Generic;
using UnityEngine;
using ResourcefulGeysers;
using ResourcefulGeysers.Utils;

public class GeyserSpawner : MonoBehaviour
{
    public HashSet<GeyserSpawn> SpawnedObjects = new HashSet<GeyserSpawn>();

    public static GeyserSpawner EnsureOn(GameObject geyser)
    {
        var spawner = geyser.GetComponent<GeyserSpawner>();
        if (spawner == null)
        {
            Plugin.Log.LogDebug($"Adding GeyserSpawner to geyser '{geyser.GetComponent<UniqueIdentifier>()?.Id}' at {geyser.transform.position}");
            spawner = geyser.AddComponent<GeyserSpawner>();
        }

        return spawner;
    }

    public int GetSpawnCount()
    {
        int maxCount = UnityEngine.Random.Range(Plugin.MinChunkCount, Plugin.MaxChunkCount + 1);
        if (maxCount <= 0)
        {
            return 0;
        }

        int count = 0;
        for (int i = 0; i < maxCount; i++)
        {
            if (UnityEngine.Random.value <= Plugin.ChunkChance)
            {
                count++;
            }
        }

        return count;
    }

    public void SpawnResource()
    {
        var geyserSpawn = GeyserSpawn.Create(this, PrefabUtils.ShaleChunk);
        var instance = geyserSpawn.gameObject;

        Transform transform = instance.transform;
        transform.position = gameObject.transform.position + new Vector3(0f, 10f, 0f);
        transform.rotation = UnityEngine.Random.rotation;
        Vector2 vector = UnityEngine.Random.insideUnitCircle.normalized * Plugin.MaxSpawnOffset;
        Vector3 position = new Vector3(vector.x, 0f, vector.y);
        transform.position = transform.TransformPoint(position);

        var rigidBody = instance.GetComponent<Rigidbody>();
        rigidBody.isKinematic = false;

        var velocity = UnityEngine.Random.onUnitSphere * Plugin.MaxSpawnVelocity;
        velocity.y = System.Math.Abs(velocity.y);
        rigidBody.velocity = velocity;

        instance.SetActive(true);
    }

    public void SpawnResources(Geyser geyser)
    {
        if (!Plugin.Enabled || !geyser.erupting)
        {
            return;
        }

        int count = Math.Min(GetSpawnCount(), Plugin.MaxSpawnedObjects - SpawnedObjects.Count);
        if (count <= 0)
        {
            return;
        }

        //Plugin.Log.LogDebug($"Spawning {count} chunks at {gameObject.transform.position}");
        for (int i = 0; i < count; i++)
        {
            SpawnResource();
        }
    }

    public void DestroyAllSpawns()
    {
        if (SpawnedObjects.Count == 0)
        {
            return;
        }

        foreach (var spawn in SpawnedObjects)
        {
            Destroy(spawn);
        }

        SpawnedObjects.Clear();
    }
}