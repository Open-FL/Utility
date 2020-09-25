namespace Utility.Threading
{
    internal class ThreadWorkerItem
    {

        public OnThreadWorkerItemFinish OnFinishEvent;
        public object WorkItem;

        public ThreadWorkerItem(object workItem, OnThreadWorkerItemFinish onFinishEvent = null)
        {
            WorkItem = workItem;
            OnFinishEvent = onFinishEvent;
        }

    }

    internal class ThreadWorkerItem<TIn, TOut>
    {

        public OnThreadWorkerItemFinish<TOut> OnFinishEvent;
        public TIn WorkItem;

        public ThreadWorkerItem(TIn workItem, OnThreadWorkerItemFinish<TOut> onFinishEvent = null)
        {
            WorkItem = workItem;
            OnFinishEvent = onFinishEvent;
        }

    }
}