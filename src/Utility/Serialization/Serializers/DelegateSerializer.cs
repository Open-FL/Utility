namespace Utility.Serialization.Serializers
{
    public class DelegateSerializer : ASerializer
    {

        public delegate object DelDeserialize(byte[] bytes);

        public delegate byte[] DelSerialize(object obj);

        private readonly DelDeserialize deserializer;

        private readonly DelSerialize serializer;

        public DelegateSerializer(DelDeserialize deserialize, DelSerialize serialize)
        {
            serializer = serialize;
            deserializer = deserialize;
        }

        public override object Deserialize(PrimitiveValueWrapper s)
        {
            return deserializer(s.ReadBytes());
        }

        public override void Serialize(PrimitiveValueWrapper s, object o)
        {
            s.Write(serializer(o));
        }

    }
}