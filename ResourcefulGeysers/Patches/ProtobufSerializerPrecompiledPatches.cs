﻿using HarmonyLib;
using ProtoBuf;
using ProtoBuf.Serializers;
using ResourcefulGeysers.Utils;

namespace ResourcefulGeysers.Patches
{
    [HarmonyPatch(typeof(ProtobufSerializerPrecompiled))]
    internal class ProtobufSerializerPrecompiledPatches
    {
        [HarmonyPatch(nameof(ProtobufSerializerPrecompiled.Serialize))]
        [HarmonyPrefix]
        public static bool Serialize_Prefix(ProtobufSerializerPrecompiled __instance, int num, object obj, ProtoWriter writer)
        {
            if (num != SerializerUtils.DispatchingTypeNumber)
            {
                return true;
            }

            var type = obj.GetType();
            if (!SerializerUtils.Serializers.TryGetValue(type, out IProtoSerializer serializer))
            {
                Plugin.Log.LogDebug($"An object with type {type.FullName} is trying to serialize without being registered.");
                return true;
            }

            serializer.Write(obj, writer);
            return false;
        }

        [HarmonyPatch(nameof(ProtobufSerializerPrecompiled.Deserialize))]
        [HarmonyPrefix]
        public static bool Deserialize_Prefix(ProtobufSerializerPrecompiled __instance, int num, object obj, ProtoReader reader)
        {
            if (num != SerializerUtils.DispatchingTypeNumber)
            {
                return true;
            }

            var type = obj.GetType();
            if (!SerializerUtils.Serializers.TryGetValue(type, out IProtoSerializer serializer))
            {
                Plugin.Log.LogDebug($"An object with type {type.FullName} is trying to serialize without being registered.");
                return true;
            }

            serializer.Read(obj, reader);
            return false;
        }
    }
}