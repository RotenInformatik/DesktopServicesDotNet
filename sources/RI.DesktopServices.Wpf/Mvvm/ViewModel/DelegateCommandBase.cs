using System;
using System.Windows.Input;




namespace RI.DesktopServices.Mvvm.ViewModel
{
    /// <summary>
    ///     Base class for delegate command implementations.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Delegate commands are command targets which route the command to a specified delegate.
    ///     </para>
    ///     <para>
    ///         Delegate commands can be used to conveniently implement command handling in view models by providing delegate
    ///         command properties which route the commands to methods in the view model.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public abstract class DelegateCommandBase : ICommand
    {
        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets or sets the delegate which is executed when <see cref="ICommand" />.<see cref="ICommand.CanExecute" /> is
        ///     called.
        /// </summary>
        /// <value>
        ///     The delegate which is executed when <see cref="ICommand" />.<see cref="ICommand.CanExecute" /> is called.
        /// </value>
        protected Func<object, bool> CanExecuteFunction { get; set; }

        /// <summary>
        ///     Gets or sets the delegate which is executed when <see cref="ICommand" />.<see cref="ICommand.Execute" /> is called.
        /// </summary>
        /// <value>
        ///     The delegate which is executed when <see cref="ICommand" />.<see cref="ICommand.Execute" /> is called.
        /// </value>
        protected Action<object> CommandAction { get; set; }

        #endregion




        #region Instance Methods

        /// <summary>
        ///     Informs the command source that the value of <see cref="CanExecute" /> might have changed.
        /// </summary>
        public void RaiseCanExecuteChanged ()
        {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion




        #region Interface: ICommand

        /// <inheritdoc cref="ICommand.CanExecuteChanged" />
        public event EventHandler CanExecuteChanged;

        /// <inheritdoc cref="ICommand.CanExecute" />
        public bool CanExecute (object parameter)
        {
            return this.CanExecuteFunction?.Invoke(parameter) ?? true;
        }

        /// <inheritdoc cref="ICommand.Execute" />
        public void Execute (object parameter)
        {
            this.CommandAction?.Invoke(parameter);
        }

        #endregion
    }
}
