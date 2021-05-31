using System.ComponentModel;
using System.Runtime.CompilerServices;




namespace RI.DesktopServices.Mvvm
{
    /// <summary>
    ///     Boilerplate implementation of types which can raise notifications about changes of its properties.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public abstract class NotificationObject : INotifyPropertyChanging, INotifyPropertyChanged
    {
        #region Virtuals

        /// <summary>
        ///     Raises the <see cref="INotifyPropertyChanged.PropertyChanged" /> event.
        /// </summary>
        /// <param name="propertyName"> The name of the property which was changed. </param>
        protected virtual void OnPropertyChanged ([CallerMemberName] string propertyName = null) =>
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        ///     Raises the <see cref="INotifyPropertyChanging.PropertyChanging" /> event.
        /// </summary>
        /// <param name="propertyName"> The name of the property which is going to be changed. </param>
        protected virtual void OnPropertyChanging ([CallerMemberName] string propertyName = null) =>
            this.PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));

        #endregion




        #region Interface: INotifyPropertyChanged

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion




        #region Interface: INotifyPropertyChanging

        /// <inheritdoc />
        public event PropertyChangingEventHandler PropertyChanging;

        #endregion
    }
}
