using System;
using System.Collections;
using System.Threading;




namespace RI.DesktopServices.Windows.Messages
{
    //TODO: IThreadDispatcher

    //TODO: ISynchronizable
    //TODO: Destructor
    //TODO: Use priority queue
    //TODO: Add timeout functionality
    public sealed class WindowsMessageLoop
    {
        #region Instance Constructor/Destructor

        public WindowsMessageLoop ()
        {
            this.SyncRoot = new object();

            this.Thread = null;
            this.ThreadId = 0;
            this.Queue = null;
            this.LastInvokeHandle = 0;

            this.ShutdownMode = WindowsMessageLoopShutdownMode.None;
        }

        #endregion




        #region Instance Fields

        private WindowsMessageLoopShutdownMode _shutdownMode;

        #endregion




        #region Instance Properties/Indexer

        public bool IsInThread
        {
            get
            {
                lock (this.SyncRoot)
                {
                    this.VerifyRunning();

                    return this.Thread.ManagedThreadId == Thread.CurrentThread.ManagedThreadId;
                }
            }
        }

        public bool IsRunning
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this.Thread != null;
                }
            }
        }

        public WindowsMessageLoopShutdownMode ShutdownMode
        {
            get
            {
                lock (this.SyncRoot)
                {
                    this.VerifyRunning();

                    return this._shutdownMode;
                }
            }
            private set
            {
                lock (this.SyncRoot)
                {
                    this._shutdownMode = value;
                }
            }
        }

        internal uint NativeThreadId
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this.ThreadId;
                }
            }
        }

        private int LastInvokeHandle { get; set; }

        private Hashtable Queue { get; set; }

        private object SyncRoot { get; set; }

        private Thread Thread { get; set; }

        private uint ThreadId { get; set; }

        #endregion




        #region Instance Methods

        public WindowsMessageLoopOperation Post (Delegate action, params object[] parameters)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            parameters = parameters ?? new object[0];

            lock (this.SyncRoot)
            {
                this.VerifyRunning();
                this.VerifyNotShuttingDown();

                this.LastInvokeHandle++;
                int handle = this.LastInvokeHandle;

                WindowsMessageLoopOperation operation =
                    new WindowsMessageLoopOperation(this, handle, action, parameters);

                this.Queue.Add(handle, operation);
                return operation;
            }
        }

        public void Run ()
        {
            try
            {
                lock (this.SyncRoot)
                {
                    this.VerifyNotRunning();

                    this.Thread = Thread.CurrentThread;
                    this.ThreadId = WindowsMessage.GetCurrentThreadId();
                    this.Queue = new Hashtable();
                    this.LastInvokeHandle = 0;
                    this.ShutdownMode = WindowsMessageLoopShutdownMode.None;
                }

                while (true)
                {
                    WindowsMessage message = WindowsMessage.GetMessage();

                    message = message.Translate();
                    message.Dispatch();

                    //TODO: Events etc.
                    //TODO: Observe shutdown mode
                }
            }
            finally
            {
                lock (this.SyncRoot)
                {
                    this.Queue?.Clear();

                    this.Thread = null;
                    this.ThreadId = 0;
                    this.Queue = null;
                    this.LastInvokeHandle = 0;
                }
            }
        }

        public object Send (Delegate action, params object[] parameters)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            parameters = parameters ?? new object[0];

            bool isInThread;
            WindowsMessageLoopOperation operation;

            lock (this.SyncRoot)
            {
                this.VerifyRunning();
                this.VerifyNotShuttingDown();

                isInThread = this.IsInThread;
                operation = this.Post(action, parameters);
            }

            if (isInThread)
            {
                //TODO
                //this.ExecuteFrame(operation);
            }
            else
            {
                operation.Wait();
            }

            return operation.Result;
        }

        public void Shutdown (bool finishPendingMessages)
        {
            lock (this.SyncRoot)
            {
                this.VerifyRunning();
                this.VerifyNotShuttingDown();

                this.ShutdownMode = finishPendingMessages ? WindowsMessageLoopShutdownMode.FinishPending
                                        : WindowsMessageLoopShutdownMode.DiscardPending;

                //TODO
            }
        }

        private void VerifyNotRunning ()
        {
            if (this.IsRunning)
            {
                throw new InvalidOperationException(nameof(WindowsMessageLoop) + " is already running.");
            }
        }

        private void VerifyNotShuttingDown ()
        {
            if (this.ShutdownMode != WindowsMessageLoopShutdownMode.None)
            {
                throw new InvalidOperationException(nameof(WindowsMessageLoop) + " is already shutting down.");
            }
        }

        private void VerifyRunning ()
        {
            if (!this.IsRunning)
            {
                throw new InvalidOperationException(nameof(WindowsMessageLoop) + " is not running.");
            }
        }

        private void VerifyShuttingDown ()
        {
            if (this.ShutdownMode == WindowsMessageLoopShutdownMode.None)
            {
                throw new InvalidOperationException(nameof(WindowsMessageLoop) + " is not shutting down.");
            }
        }

        #endregion
    }

    //TODO: ISynchronizable
    //TODO: Destructor
    //TODO: Add priority
    //TODO: Add timeout
}
