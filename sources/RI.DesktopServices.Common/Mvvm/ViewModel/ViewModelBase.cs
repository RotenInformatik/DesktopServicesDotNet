namespace RI.DesktopServices.Mvvm.ViewModel
{
    /// <summary>
    ///     Implements a base class for view models.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public abstract class ViewModelBase : NotificationObject, IViewModel
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="ViewModelBase" />.
        /// </summary>
        protected ViewModelBase ()
        {
            this.IsInitialized = false;
            this.SortIndex = null;
        }

        #endregion




        #region Instance Fields

        private bool _isInitialized;

        private int? _sortIndex;

        #endregion




        #region Virtuals

        /// <inheritdoc cref="IViewModel.Initialize" />
        protected virtual void Initialize () { }

        #endregion




        #region Interface: IRegionElement

        /// <inheritdoc />
        public int? SortIndex
        {
            get => this._sortIndex;
            private set
            {
                this.OnPropertyChanging();
                this._sortIndex = value;
                this.OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public virtual void Activated () { }

        /// <inheritdoc />
        public virtual bool CanNavigateFrom () => true;

        /// <inheritdoc />
        public virtual bool CanNavigateTo () => true;

        /// <inheritdoc />
        public virtual void Deactivated () { }

        /// <inheritdoc />
        public virtual void NavigatedFrom () { }

        /// <inheritdoc />
        public virtual void NavigatedTo () { }

        #endregion




        #region Interface: IViewModel

        /// <inheritdoc />
        public bool IsInitialized
        {
            get => this._isInitialized;
            private set
            {
                this.OnPropertyChanging();
                this._isInitialized = value;
                this.OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        void IViewModel.Initialize ()
        {
            if (!this.IsInitialized)
            {
                this.Initialize();
                this.IsInitialized = true;
            }
        }

        #endregion
    }
}
