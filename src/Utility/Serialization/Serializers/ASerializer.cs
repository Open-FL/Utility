namespace Utility.Serialization.Serializers
{
    /// <summary>
    /// Base Class for all Serializers
    /// </summary>
    public abstract class ASerializer
    {

        /// <summary>
        /// Returns the Deserialized Object
        /// </summary>
        /// <param name="s">Stream to read from</param>
        /// <returns>The Deserialized Object</returns>
        public abstract object Deserialize(PrimitiveValueWrapper s);

        /// <summary>
        /// Serializes an object into a stream
        /// </summary>
        /// <param name="s">Target Stream</param>
        /// <param name="o">Object to Serialize</param>
        public abstract void Serialize(PrimitiveValueWrapper s, object o);

    }

    /// <summary>
    /// Generic Implementation for the ASerializer
    /// Overrides the not generic Functions and replaces them with generic ones.
    /// </summary>
    /// <typeparam name="T">Type that this serializer is able to deserialize</typeparam>
    public abstract class ASerializer<T> : ASerializer
    {

        /// <summary>
        /// Deserializes a Packet from the stream.
        /// </summary>
        /// <param name="s">Input Stream</param>
        /// <returns>Deserialized Packet</returns>
        public abstract T DeserializePacket(PrimitiveValueWrapper s);

        /// <summary>
        /// Serializes a Packet to the Stream
        /// </summary>
        /// <param name="s">Target Stream</param>
        /// <param name="obj">Object to Serialize</param>
        public abstract void SerializePacket(PrimitiveValueWrapper s, T obj);

        /// <summary>
        /// Non Generic Override for the ASerializer.
        /// </summary>
        /// <param name="s">Input Stream</param>
        /// <returns>Non Generic Version of the Deserialized Object</returns>
        public override object Deserialize(PrimitiveValueWrapper s)
        {
            return DeserializePacket(s);
        }

        /// <summary>
        /// Non Generic Override for the ASerializer.
        /// </summary>
        /// <param name="s">Input Stream</param>
        /// <param name="o">Non Generic version of the Object to Serialize</param>
        /// <returns>Non Generic Version of the Serialized Object</returns>
        public override void Serialize(PrimitiveValueWrapper s, object o)
        {
            SerializePacket(s, (T) o);
        }

    }
}