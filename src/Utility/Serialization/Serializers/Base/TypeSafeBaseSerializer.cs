using System;

namespace Utility.Serialization.Serializers.Base
{
    /// <summary>
    /// Default Implementation for the BaseSerializer.
    /// Uses Type.AssemblyQualifiedName as Unique Type Key
    /// </summary>
    public class TypeSafeBaseSerializer : ABaseSerializer
    {

        /// <summary>
        /// Deserializes a BasePacket from the stream.
        /// </summary>
        /// <param name="s">Input Stream</param>
        /// <returns>Deserialized BasePacket</returns>
        public override BasePacket DeserializePacket(PrimitiveValueWrapper pvw)
        {
            object packetType = pvw.ReadString();
            byte[] payload = pvw.ReadBytes();
            return new BasePacket(packetType, payload);
        }

        /// <summary>
        /// Serializes a BasePacket to the Stream
        /// </summary>
        /// <param name="s">Target Stream</param>
        /// <param name="obj">BasePacket to Serialize</param>
        public override void SerializePacket(PrimitiveValueWrapper pvw, BasePacket obj)
        {
            pvw.Write((string) obj.PacketType);
            pvw.Write(obj.Payload);
        }

        /// <summary>
        /// Returns the Unique Key for each Type
        /// using t.AssemblyQualifiedName
        /// </summary>
        /// <param name="t">Type to generate key for</param>
        /// <returns>The Unique Key per type</returns>
        public override object GetKey(Type t)
        {
            return t.AssemblyQualifiedName;
        }

    }
}