using System;
using ProtoBuf;
using UnityEngine;
using ResourcefulGeysers;
using ResourcefulGeysers.Utils;

// MonoBehaviour serialization don't seem to be supported outside the global namespace.
public class GeyserSpawn : MonoBehaviour, ISaveSupported
{
    public int Version = 1;

    public string GeyserId = null;

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
        //Plugin.Log.LogDebug($"Restored geyser spawn at {gameObject.transform.position}. It will expire in {ExpireTime - DayNightCycle.main.timePassed} seconds");
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

    public void OnProtoSerializeObject(ProtoWriter writer)
    {
        ProtoWriter.WriteFieldHeader(1, WireType.Variant, writer);
        ProtoWriter.WriteInt32(Version, writer);

        GeyserId = Spawner?.gameObject.GetComponent<UniqueIdentifier>()?.Id;
        if (GeyserId != null)
        {
            ProtoWriter.WriteFieldHeader(2, WireType.String, writer);
            ProtoWriter.WriteString(GeyserId, writer);
        }

        ProtoWriter.WriteFieldHeader(3, WireType.Fixed64, writer);
        ProtoWriter.WriteDouble(ExpireTime, writer);

        //Plugin.Log.LogDebug($"Serialized object at {gameObject.transform.position} with GeyserId {GeyserId}");
    }

    public void OnProtoDesrializeObject(ProtoReader reader)
    {
        for (int i = reader.ReadFieldHeader(); i > 0; i = reader.ReadFieldHeader())
        {
            switch (i)
            {
                case 1:
                    Version = reader.ReadInt32();
                    break;
                case 2:
                    GeyserId = reader.ReadString();
                    break;
                case 3:
                    ExpireTime = reader.ReadDouble();
                    break;
            }
        }
        //Plugin.Log.LogDebug($"Deserialized object at {gameObject.transform.position} with GeyserId {GeyserId}");
    }
}