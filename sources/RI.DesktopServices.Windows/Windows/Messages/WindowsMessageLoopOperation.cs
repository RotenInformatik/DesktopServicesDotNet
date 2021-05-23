using System;
using System.Threading;




namespace RI.Framework.Utilities.Windows
{
    public sealed class WindowsMessageLoopOperation
    {
        #region Instance Constructor/Destructor

        internal WindowsMessageLoopOperation (WindowsMessageLoop messageLoop, int handle, Delegate action,
                                              object[] parameters)
        {
            this.SyncRoot = new object();

            this.MessageLoop = messageLoop;
            this.Handle = handle;
            this.Action = action;
            this.Parameters = parameters;

            this.State = WindowsMessageLoopOperationState.Waiting;
            this.Result = null;
            this.Exception = null;
            this.OperationDone = new ManualResetEvent(false);
        }

        #endregion




        #region Instance Fields

        private Exception _exception;

        private object _result;

        private WindowsMessageLoopOperationState _state;

        #endregion




        #region Instance Properties/Indexer

        public Exception Exception
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._exception;
                }
            }
            private set
            {
                lock (this.SyncRoot)
                {
                    this._exception = value;
                }
            }
        }

        public bool IsDone
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return (this.State != WindowsMessageLoopOperationState.Waiting) &&
                           (this.State != WindowsMessageLoopOperationState.Executing);
                }
            }
        }

        public object Result
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._result;
                }
            }
            private set
            {
                lock (this.SyncRoot)
                {
                    this._result = value;
                }
            }
        }

        public WindowsMessageLoopOperationState State
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._state;
                }
            }
            private set
            {
                lock (this.SyncRoot)
                {
                    this._state = value;
                }
            }
        }

        private Delegate Action { get; set; }

        private int Handle { get; set; }

        private WindowsMessageLoop MessageLoop { get; set; }

        private ManualResetEvent OperationDone { get; set; }

        private object[] Parameters { get; set; }

        private object SyncRoot { get; set; }

        #endregion




        #region Instance Methods

        public bool Cancel ()
        {
            lock (this.SyncRoot)
            {
                if (this.State != WindowsMessageLoopOperationState.Waiting)
                {
                    return false;
                }

                this.State = WindowsMessageLoopOperationState.Canceled;
                this.Result = null;
                this.OperationDone.Set();

                return true;
            }
        }

        public void Wait ()
        {
            this.Wait(Timeout.Infinite);
        }

        public bool Wait (TimeSpan timeout)
        {
            return this.Wait((int)timeout.TotalMilliseconds);
        }

        public bool Wait (int milliseconds)
        {
            if ((milliseconds < 0) && (milliseconds != Timeout.Infinite))
            {
                throw new ArgumentOutOfRangeException(nameof(milliseconds));
            }

            return this.OperationDone.WaitOne(milliseconds);
        }

        internal void Execute ()
        {
            lock (this.SyncRoot)
            {
                if (this.State != WindowsMessageLoopOperationState.Waiting)
                {
                    return;
                }

                this.State = WindowsMessageLoopOperationState.Executing;
            }

            object result = null;

            try
            {
                result = this.Action.DynamicInvoke(this.Parameters);
            }
            catch (Exception exception)
            {
                this.Exception = exception;
            }

            lock (this.SyncRoot)
            {
                this.State = this.Exception == null ? WindowsMessageLoopOperationState.Finished
                                 : WindowsMessageLoopOperationState.Exception;

                this.Result = result;
                this.OperationDone.Set();
            }
        }

        #endregion
    }
}
