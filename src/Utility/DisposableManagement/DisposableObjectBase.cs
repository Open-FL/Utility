using System;

namespace Utility.DisposableManagement
{
    public abstract class DisposableObjectBase : IDisposable
    {

        protected DisposableObjectBase(object handleIdentifier)
        {
            HandleIdentifier = handleIdentifier;
        }

        public object HandleIdentifier { get; }


        public virtual void Dispose()
        {
        }

    }
}