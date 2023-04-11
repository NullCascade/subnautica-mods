using System;
using System.Linq;
using ProtoBuf;

namespace ResourcefulGeysers.Utils
{
    public interface ISaveSupported
    {
        void OnProtoSerializeObject(ProtoWriter writer);

        void OnProtoDesrializeObject(ProtoReader reader);
    }

    public static class SerializerUtils
    {
        internal const int DispatchingTypeNumber = 69;

        public static void RegisterSerializer<T>()
        {
            RegisterSerializer(typeof(T));
        }

        public static void RegisterSerializer(Type type)
        {
            if (!type.GetInterfaces().Contains(typeof(ISaveSupported)))
            {
                throw new Exception($"Type {type.FullName} does not implement ISaveSupported.");
            }

            if (ProtobufSerializerPrecompiled.knownTypes.ContainsKey(type))
            {
                throw new Exception($"Type {type.FullName} is already registered!");
            }

            ProtobufSerializerPrecompiled.knownTypes[type] = DispatchingTypeNumber;
        }
    }
}
