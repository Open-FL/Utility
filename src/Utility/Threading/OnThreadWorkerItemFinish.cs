namespace Utility.Threading
{
    public delegate void OnThreadWorkerItemFinish<in TOut>(TOut result);

    public delegate void OnThreadWorkerItemFinish(object result);
}