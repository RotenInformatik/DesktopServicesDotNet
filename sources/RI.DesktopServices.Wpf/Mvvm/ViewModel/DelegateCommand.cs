using System;




namespace RI.DesktopServices.Mvvm.ViewModel
{
    /// <summary>
    ///     An implementation of a delegate command with one command parameter.
    /// </summary>
    /// <typeparam name="T"> The type of the command parameter. </typeparam>
    /// <threadsafety static="false" instance="false" />
    public sealed class DelegateCommand <T> : DelegateCommandBase
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="DelegateCommand" />.
        /// </summary>
        /// <param name="command"> The command delegate to execute. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="command" /> is null. </exception>
        public DelegateCommand (Action<T> command)
            : this(command, null) { }

        /// <summary>
        ///     Creates a new instance of <see cref="DelegateCommand" />.
        /// </summary>
        /// <param name="command"> The command delegate to execute. </param>
        /// <param name="canExecute">
        ///     The delegate which is executed when it is checked whether the command can be executed. Can be
        ///     null.
        /// </param>
        /// <exception cref="ArgumentNullException"> <paramref name="command" /> is null. </exception>
        public DelegateCommand (Action<T> command, Func<T, bool> canExecute)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            this.CommandPrivate = command;
            this.CanExecutePrivate = canExecute;

            this.CommandAction = x => this.CommandPrivate((T)x);
            this.CanExecuteFunction = y => this.CanExecutePrivate?.Invoke((T)y) ?? true;
        }

        #endregion




        #region Instance Properties/Indexer

        private Func<T, bool> CanExecutePrivate { get; }

        private Action<T> CommandPrivate { get; }

        #endregion
    }

    /// <summary>
    ///     An implementation of a delegate command with no command parameter.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public sealed class DelegateCommand : DelegateCommandBase
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="DelegateCommand" />.
        /// </summary>
        /// <param name="command"> The command delegate to execute. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="command" /> is null. </exception>
        public DelegateCommand (Action command)
            : this(command, null) { }

        /// <summary>
        ///     Creates a new instance of <see cref="DelegateCommand" />.
        /// </summary>
        /// <param name="command"> The command delegate to execute. </param>
        /// <param name="canExecute">
        ///     The delegate which is executed when it is checked whether the command can be executed. Can be
        ///     null.
        /// </param>
        /// <exception cref="ArgumentNullException"> <paramref name="command" /> is null. </exception>
        public DelegateCommand (Action command, Func<bool> canExecute)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            this.CommandPrivate = command;
            this.CanExecutePrivate = canExecute;

            this.CommandAction = _ => this.CommandPrivate();
            this.CanExecuteFunction = _ => this.CanExecutePrivate?.Invoke() ?? true;
        }

        #endregion




        #region Instance Properties/Indexer

        private Func<bool> CanExecutePrivate { get; }

        private Action CommandPrivate { get; }

        #endregion
    }
}
