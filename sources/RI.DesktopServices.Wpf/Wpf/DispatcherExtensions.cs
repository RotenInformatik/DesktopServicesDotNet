using System;
using System.Threading.Tasks;
using System.Windows.Threading;




namespace RI.DesktopServices.Wpf
{
    /// <summary>
    ///     Provides utility/extension methods for the <see cref="Dispatcher" /> type.
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    public static class DispatcherExtensions
    {
        #region Static Methods

        /// <summary>
        ///     Forces the dispatcher to process all its queued operations.
        /// </summary>
        /// <param name="dispatcher"> The dispatcher. </param>
        /// <remarks>
        ///     <para>
        ///         The method does not return until all operations are processed.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> is null. </exception>
        public static void DoAllEvents (this Dispatcher dispatcher)
        {
            dispatcher.DoEvents(DispatcherPriority.SystemIdle);
        }

        /// <summary>
        ///     Forces the dispatcher to process all its queued operations.
        /// </summary>
        /// <param name="dispatcher"> The dispatcher. </param>
        /// <returns>
        ///     The task which can be used to await the end of processing all ist queued operations.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The method does not return until all operations are processed.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> is null. </exception>
        public static Task DoAllEventsAsync (this Dispatcher dispatcher)
        {
            return dispatcher.DoEventsAsync(DispatcherPriority.SystemIdle);
        }

        /// <summary>
        ///     Forces the dispatcher to process all its queued operations up to and including the specified priority.
        /// </summary>
        /// <param name="dispatcher"> The dispatcher. </param>
        /// <param name="priority"> The priority up to and including all operations are to be processed. </param>
        /// <remarks>
        ///     <para>
        ///         The method does not return until all operations as specified are processed.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> is null. </exception>
        public static void DoEvents (this Dispatcher dispatcher, DispatcherPriority priority)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            dispatcher.Invoke(() => { }, priority);
        }

        /// <summary>
        ///     Forces the dispatcher to process all its queued operations up to and including the specified priority.
        /// </summary>
        /// <param name="dispatcher"> The dispatcher. </param>
        /// <param name="priority"> The priority up to and including all operations are to be processed. </param>
        /// <returns>
        ///     The task which can be used to await the end of processing all ist queued operations up to and including the
        ///     specified priority.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The method does not return until all operations as specified are processed.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> is null. </exception>
        public static Task DoEventsAsync (this Dispatcher dispatcher, DispatcherPriority priority)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            DispatcherOperation operation = dispatcher.InvokeAsync(() => { }, priority);
            return operation.Task;
        }

        /// <summary>
        ///     Moves the execution of the code which follows the <c> await </c> to a <see cref="Dispatcher" />.
        /// </summary>
        /// <param name="dispatcher"> The dispatcher. </param>
        /// <param name="priority">
        ///     The optional priority after which the execution is moved to the dispatcher. Default value is
        ///     <see cref="DispatcherPriority.Normal" />
        /// </param>
        /// <returns>
        ///     The <see cref="DispatcherAwaiter" /> which can be awaited to move the execution of the code which follows the
        ///     <c> await </c> to <paramref name="dispatcher" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> is null. </exception>
        /// <example>
        ///     <para>
        ///         The following example shows how <see cref="MoveExecutionTo" /> is used:
        ///     </para>
        ///     <code language="cs">
        ///   <![CDATA[
        /// 
        ///   // Code which is executed on any thread
        ///  
        ///   await dispatcher.MoveExecutionTo();
        /// 
        ///   // Code which is executed by the dispatcher ("is moved to the thread the dispatcher is running in")
        /// 
        ///   ]]>
        ///   </code>
        /// </example>
        public static DispatcherAwaiter MoveExecutionTo (this Dispatcher dispatcher,
                                                         DispatcherPriority priority = DispatcherPriority.Normal)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            return new DispatcherAwaiter(dispatcher, priority);
        }

        #endregion
    }
}
