using System;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;
using ProtoBuf.Meta;
using ProtoBuf.Serializers;

namespace ResourcefulGeysers.Utils
{
    public static class SerializerUtils
    {
        internal const int DispatchingTypeNumber = 69;

        internal static Dictionary<Type, IProtoSerializer> Serializers = new Dictionary<Type, IProtoSerializer>();

        public static void RegisterSerializer<T>()
        {
            RegisterSerializer(typeof(T));
        }

        public static void RegisterSerializer(Type type)
        {
            // Make sure that we don't already have the type registered.
            if (ProtobufSerializerPrecompiled.knownTypes.ContainsKey(type))
            {
                throw new Exception($"Type {type.FullName} is already registered in Subnautica!");
            }

            // Also make sure we don't already have a serializer for this type.
            if (Serializers.ContainsKey(type))
            {
                throw new Exception($"Type {type.FullName} is already registered in library!");
            }

            // We will use the RuntimeTypeModel to compile the type, and store the resultant builder for later use.
            var model = RuntimeTypeModel.Default;
            var metaType = model.Add(type, true) ?? throw new Exception($"Could not create MetaType for type {type.FullName}");
            var serializer = metaType.BuildSerializer() ?? throw new Exception($"Could not create build serializer for type {type.FullName}");

            // 
            Serializers[type] = serializer;

            // Store our temporary key so we know that this type is handled by our utility.
            ProtobufSerializerPrecompiled.knownTypes[type] = DispatchingTypeNumber;

            // We also need to whitelist the component.
            ProtobufSerializer.componentWhitelist.Add(type.FullName);
        }
    }
}
