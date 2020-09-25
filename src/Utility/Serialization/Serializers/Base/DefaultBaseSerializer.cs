using System;
using System.Collections.Generic;

namespace Utility.Serialization.Serializers.Base
{
    /// <summary>
    /// Default Implementation for the BaseSerializer.
    /// Uses Type.AssemblyQualifiedName as Unique Type Key
    /// </summary>
    public class DefaultBaseSerializer : ABaseSerializer
    {

        private readonly Dictionary<string, byte> toIdMap = new Dictionary<string, byte>();
        private byte nextID;

        /// <summary>
        /// Deserializes a BasePacket from the stream.
        /// </summary>
        /// <param name="s">Input Stream</param>
        /// <returns>Deserialized BasePacket</returns>
        public override BasePacket DeserializePacket(PrimitiveValueWrapper pvw)
        {
            object packetType = pvw.ReadByte();
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
            pvw.Write((byte) obj.PacketType);
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
            if (!toIdMap.ContainsKey(t.AssemblyQualifiedName))
            {
                toIdMap.Add(t.AssemblyQualifiedName, nextID);
                nextID++;
            }

            return toIdMap[t.AssemblyQualifiedName];
        }

    }
}