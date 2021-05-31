using System;
using System.Windows;

using RI.DesktopServices.Mvvm.ViewModel;




namespace RI.DesktopServices.Mvvm.View
{
    /// <summary>
    ///     Implements a base class for <see cref="Window" />-based views.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public class WindowViewBase : Window, IView
    {
        #region Instance Properties/Indexer

        /// <inheritdoc cref="IView.IsInitialized" />
        public new bool IsInitialized { get; private set; }

        #endregion




        #region Instance Methods

        /// <summary>
        ///     Initializes this view if it was not already initialized before.
        /// </summary>
        protected void PerformInitializationIfNotAlreadyDone ()
        {
            if (!this.IsInitialized)
            {
                this.Initialize();
                this.IsInitialized = true;
            }
        }

        #endregion




        #region Overrides

        /// <inheritdoc />
        protected override void OnInitialized (EventArgs e)
        {
            base.OnInitialized(e);

            this.PerformInitializationIfNotAlreadyDone();
        }

        #endregion




        #region Virtuals

        /// <inheritdoc cref="IView.Initialize" />
        protected virtual void Initialize () { }

        #endregion




        #region Interface: IView

        /// <inheritdoc />
        bool IView.IsInitialized => this.IsInitialized;

        /// <inheritdoc />
        IViewModel IView.ViewModel
        {
            get => this.DataContext as IViewModel;
            set => this.DataContext = value;
        }

        /// <inheritdoc />
        void IView.Initialize ()
        {
            this.PerformInitializationIfNotAlreadyDone();
        }

        #endregion
    }
}
