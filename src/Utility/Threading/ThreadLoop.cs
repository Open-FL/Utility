using System.Threading;

namespace Utility.Threading
{
    public abstract class ThreadLoop
    {

        private bool stopServer;

        protected int Tick;

        public bool IsRunning { get; private set; }

        public CancellationToken Token { get; private set; }

        public virtual void StartServer(int tick, CancellationToken token)
        {
            IsRunning = true;
            Tick = tick;
            Thread loopThread = new Thread(() => Loop(token));
            loopThread.Start();
        }

        public virtual void StopServer()
        {
            stopServer = true;
        }

        public virtual void StartSynchronous(int tick, CancellationToken token)
        {
            IsRunning = true;
            Tick = tick;
            Loop(token);
        }

        private void Loop(CancellationToken token)
        {
            OnLoopEnter();

            while (!stopServer)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                Update();
                Thread.Sleep(Tick);
            }

            stopServer = false;
            OnLoopExit();
        }

        protected abstract void Update();

        protected virtual void OnLoopExit()
        {
        }

        protected virtual void OnLoopEnter()
        {
        }

    }
}