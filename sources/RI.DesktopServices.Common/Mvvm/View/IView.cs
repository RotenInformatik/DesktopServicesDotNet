using RI.DesktopServices.Mvvm.ViewModel;




namespace RI.DesktopServices.Mvvm.View
{
    /// <summary>
    ///     Defines an interface for views.
    /// </summary>
    public interface IView
    {
        /// <summary>
        ///     Gets whether the view is initialized or not.
        /// </summary>
        /// <value>
        ///     true if the view is initialized, false otherwise.
        /// </value>
        bool IsInitialized { get; }

        /// <summary>
        ///     Gets or sets the view model used by the view.
        /// </summary>
        /// <value>
        ///     The view model used by the view.
        /// </value>
        IViewModel ViewModel { get; set; }

        /// <summary>
        ///     Initializes the view.
        /// </summary>
        void Initialize ();
    }
}
