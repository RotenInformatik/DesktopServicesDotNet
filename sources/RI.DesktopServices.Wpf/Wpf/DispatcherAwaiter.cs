using System;
using System.Runtime.CompilerServices;
using System.Windows.Threading;




namespace RI.DesktopServices.Wpf
{
    /// <summary>
    ///     Implements an awaiter which moves execution to a specified <see cref="System.Windows.Threading.Dispatcher" />.
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    public sealed class DispatcherAwaiter : ICriticalNotifyCompletion
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="DispatcherAwaiter" />.
        /// </summary>
        /// <param name="dispatcher"> The used <see cref="System.Windows.Threading.Dispatcher" />. </param>
        /// <param name="priority">
        ///     The optional priority after which the execution is moved to the dispatcher. Default value is
        ///     <see cref="DispatcherPriority.Normal" />
        /// </param>
        /// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> is null. </exception>
        public DispatcherAwaiter (Dispatcher dispatcher, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            this.SyncRoot = new object();

            this.Dispatcher = dispatcher;
            this.Priority = priority;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the used <see cref="System.Windows.Threading.Dispatcher" />.
        /// </summary>
        /// <value>
        ///     The used <see cref="System.Windows.Threading.Dispatcher" />.
        /// </value>
        public Dispatcher Dispatcher { get; }

        /// <summary>
        ///     Gets whether the awaiter is already completed.
        /// </summary>
        /// <value>
        ///     true if the awaiter is already completed, false otherwise.
        /// </value>
        public bool IsCompleted => false;

        /// <summary>
        ///     Gets the used <see cref="DispatcherPriority" />.
        /// </summary>
        /// <value>
        ///     The used <see cref="DispatcherPriority" />.
        /// </value>
        public DispatcherPriority Priority { get; }

        private object SyncRoot { get; }

        #endregion




        #region Instance Methods

        /// <summary>
        ///     Gets the awaiter as required by the compiler.
        /// </summary>
        /// <returns>
        ///     The awaiter, which is nothing else than this instance itself.
        /// </returns>
        public DispatcherAwaiter GetAwaiter () => this;

        /// <summary>
        ///     Gets the result of the awaiter when completed.
        /// </summary>
        public void GetResult () { }

        #endregion




        #region Interface: ICriticalNotifyCompletion

        /// <inheritdoc />
        public void UnsafeOnCompleted (Action continuation) => this.OnCompleted(continuation);

        #endregion




        #region Interface: INotifyCompletion

        /// <inheritdoc />
        public void OnCompleted (Action continuation)
        {
            if (continuation == null)
            {
                throw new ArgumentNullException(nameof(continuation));
            }

            this.Dispatcher.BeginInvoke(this.Priority, continuation);
        }

        #endregion
    }
}
