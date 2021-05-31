using System.ComponentModel;

using RI.DesktopServices.UiContainer;




namespace RI.DesktopServices.Mvvm.ViewModel
{
    /// <summary>
    ///     Defines an interface for view models.
    /// </summary>
    public interface IViewModel : INotifyPropertyChanging, INotifyPropertyChanged, IRegionElement
    {
        /// <summary>
        ///     Gets whether the view model is initialized or not.
        /// </summary>
        /// <value>
        ///     true if the view model is initialized, false otherwise.
        /// </value>
        bool IsInitialized { get; }

        /// <summary>
        ///     Initializes the view model.
        /// </summary>
        void Initialize ();
    }
}
