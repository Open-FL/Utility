using Utility.Exceptions;

namespace Utility.IO.VirtualFS
{
    public class ManifestReaderException : Byt3Exception
    {

        public ManifestReaderException(string message) : base(message)
        {
        }

    }
}