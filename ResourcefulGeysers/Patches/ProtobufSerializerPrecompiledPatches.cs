using HarmonyLib;
using ProtoBuf;
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
            if (num == SerializerUtils.DispatchingTypeNumber)
            {
                if (!(obj is ISaveSupported saver))
                {
                    Plugin.Log.LogDebug($"An object with type {obj.GetType().FullName} is is using the ISaveSupported identifier.");
                    return true;
                }

                saver.OnProtoSerializeObject(writer);
                return false;
            }
            return true;
        }

        [HarmonyPatch(nameof(ProtobufSerializerPrecompiled.Deserialize))]
        [HarmonyPrefix]
        public static bool Deserialize_Prefix(ProtobufSerializerPrecompiled __instance, int num, object obj, ProtoReader reader)
        {
            if (num == SerializerUtils.DispatchingTypeNumber)
            {
                if (!(obj is ISaveSupported saver))
                {
                    Plugin.Log.LogDebug($"An object with type {obj.GetType().FullName} is is using the ISaveSupported identifier.");
                    return true;
                }

                saver.OnProtoDesrializeObject(reader);
                return false;
            }
            return true;
        }
    }
}