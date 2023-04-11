using System;
using ProtoBuf;
using UnityEngine;
using ResourcefulGeysers;

// MonoBehaviour serialization don't seem to be supported outside the global namespace.
[ProtoContract]
public class GeyserSpawn : MonoBehaviour, IProtoEventListener
{
    [ProtoMember(1)]
    public int Version = 1;

    [ProtoMember(2)]
    [AssertNotNull]
    public string GeyserId = null;

    [ProtoMember(3)]
    public double ExpireTime = 0.0;

    public GeyserSpawner Spawner;

    public static GeyserSpawn Create(GeyserSpawner spawner, GameObject prefab)
    {
        var instance = Instantiate(prefab);

        var spawn = instance.AddComponent<GeyserSpawn>();
        spawn.Spawner = spawner;

        spawner.SpawnedObjects.Add(spawn);

        spawn.ExpireTime = DayNightCycle.main.timePassed + Plugin.SpawnLifetime;

        return spawn;
    }

    public void Awake()
    {
        var rigidBody = gameObject.GetComponent<Rigidbody>();
        if (rigidBody != null)
        {
            rigidBody.isKinematic = false;
        }
    }

    public void Update()
    {
        RestoreIfNeeded();
        ExpireIfNeeded();
    }

    private void RestoreIfNeeded()
    {
        if (string.IsNullOrEmpty(GeyserId))
        {
            return;
        }

        if (Spawner != null)
        {
            GeyserId = null;
            return;
        }

        if (!UniqueIdentifier.TryGetIdentifier(GeyserId, out UniqueIdentifier uniqueIdentifier))
        {
            GeyserId = null;
            throw new Exception($"Could not resolve unique identifier {GeyserId} when trying to restore geyser spawner!");
        }

        var geyserGameObject = uniqueIdentifier.gameObject;

        Spawner = GeyserSpawner.EnsureOn(geyserGameObject);
        if (Spawner == null)
        {
            throw new Exception($"Could not get spawner!");
        }

        Spawner.SpawnedObjects.Add(this);
        Plugin.Log.LogDebug($"Restored geyser spawn at {gameObject.transform.position}. It will expire in {ExpireTime - DayNightCycle.main.timePassed} seconds");
    }

    public bool ExpireIfNeeded()
    {
        if (ExpireTime > DayNightCycle.main.timePassed)
        {
            return false;
        }

        //Plugin.Log.LogDebug($"A spawn has expired at {gameObject.transform.position}. {ExpireTime} vs. {DayNightCycle.main.timePassed}");
        Destroy(gameObject);
        Destroy(this);

        return true;
    }

    public void OnProtoSerialize(ProtobufSerializer serializer)
    {
        GeyserId = Spawner?.gameObject.GetComponent<UniqueIdentifier>()?.Id;
    }

    public void OnProtoDeserialize(ProtobufSerializer serializer)
    {

    }
}