using Utility.Exceptions;

namespace Utility.ExtPP.Plugins
{
    public class ErrorException : Byt3Exception
    {

        public ErrorException(string message) : base(message)
        {
        }

    }
}