using System.Threading;

namespace OperatingSystemsPractices.Source.Threads
{
    public class ThreadWithEvents
    {
        private Thread thread;
        public bool IsRunning { get; private set; } = true;
        public AutoResetEvent StartEvent { get; private set; } = new AutoResetEvent(false);
        public AutoResetEvent FinishEvent { get; private set; } = new AutoResetEvent(false);
        public ThreadWithEvents(Thread thread) => this.thread = thread;
        public Thread GetThread() => thread;
        public void Stop()
        {
            IsRunning = false;
            StartEvent.Set();
        }
    }
}
