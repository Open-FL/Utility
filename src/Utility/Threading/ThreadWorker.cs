using System.Collections.Concurrent;

namespace Utility.Threading
{
    public abstract class ThreadWorker : ThreadLoop
    {

        private readonly ConcurrentQueue<ThreadWorkerItem> queue = new ConcurrentQueue<ThreadWorkerItem>();

        public virtual void EnqueueItem(object workItem, OnThreadWorkerItemFinish onFinishEvent = null)
        {
            queue.Enqueue(new ThreadWorkerItem(workItem, onFinishEvent));
        }

        protected abstract object DoWork(object input);

        protected override void Update()
        {
            while (queue.TryDequeue(out ThreadWorkerItem result))
            {
                object ret = DoWork(result.WorkItem);
                result.OnFinishEvent?.Invoke(ret);
            }
        }

    }

    public abstract class ThreadWorker<TIn, TOut> : ThreadWorker
    {

        protected abstract TOut DoWork(TIn input);

        protected override object DoWork(object input)
        {
            return DoWork((TIn) input);
        }

    }
}