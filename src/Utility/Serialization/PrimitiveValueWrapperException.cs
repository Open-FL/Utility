using Utility.Exceptions;

namespace Utility.Serialization
{
    public class PrimitiveValueWrapperException : Byt3Exception
    {

        public PrimitiveValueWrapperException(string message) : base(message)
        {
        }

    }
}