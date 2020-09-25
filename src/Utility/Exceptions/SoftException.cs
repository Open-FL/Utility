using System;

namespace Utility.Exceptions
{
    public class SoftException : Exception
    {

        public SoftException(Exception ex) : base(ex.Message, ex)
        {
        }

    }
    
}