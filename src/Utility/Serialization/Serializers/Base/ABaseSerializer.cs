using System;

namespace Utility.Serialization.Serializers.Base
{
    /// <summary>
    ///     Abstract Base Serializer is used to Serialize the Base Packet containing a Key Object that has to be unique for all
    ///     types
    /// </summary>
    public abstract class ABaseSerializer : ASerializer<BasePacket>
    {

        /// <summary>
        ///     Returns the Key that is used to index the serializers.
        ///     Can take longer. This gets only called once per Type
        /// </summary>
        /// <param name="t">Type to get the Key for</param>
        /// <returns>The key for the supplied type.</returns>
        public abstract object GetKey(Type t);

    }
}