namespace Utility.Serialization.Serializers.Base
{
    /// <summary>
    /// Base Packet is the base for the Serialization Library.
    /// All Serialized Objects will be wrapped into this packet.
    /// </summary>
    public class BasePacket
    {

        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="packetType">Custom Packet Key</param>
        /// <param name="payload">Serialized Packet Data</param>
        public BasePacket(object packetType, byte[] payload)
        {
            PacketType = packetType;
            Payload = payload;
        }

        /// <summary>
        /// Custom Key that is used to determine the type of the Packet
        /// </summary>
        public object PacketType { get; }

        /// <summary>
        /// Serialized Packet that this Base Packet is wrapping around
        /// </summary>
        public byte[] Payload { get; }

    }
}