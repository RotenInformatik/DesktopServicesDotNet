using System;
using System.Windows;




namespace RI.DesktopServices.UiContainer.Adapters
{
    /// <summary>
    ///     Boilerplate implementation of <see cref="IRegionAdapter" /> for WPF.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public abstract class WpfRegionAdapterBase : RegionAdapterBase
    {
        #region Overrides

        /// <inheritdoc />
        public override void Activate (object container, object element)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            base.Activate(container, element);

            if (element is FrameworkElement)
            {
                FrameworkElement frameworkElement = (FrameworkElement)element;
                if (frameworkElement.DataContext is IRegionElement)
                {
                    IRegionElement regionElement = (IRegionElement)frameworkElement.DataContext;
                    regionElement.Activated();
                }
            }
        }

        /// <inheritdoc />
        public override void Deactivate (object container, object element)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            base.Deactivate(container, element);

            if (element is FrameworkElement)
            {
                FrameworkElement frameworkElement = (FrameworkElement)element;
                if (frameworkElement.DataContext is IRegionElement)
                {
                    IRegionElement regionElement = (IRegionElement)frameworkElement.DataContext;
                    regionElement.Deactivated();
                }
            }
        }

        /// <inheritdoc />
        protected override bool CanNavigateFrom (object container, object element)
        {
            bool fromBase = base.CanNavigateFrom(container, element);
            bool fromDataContext = true;

            if (element is FrameworkElement)
            {
                FrameworkElement frameworkElement = (FrameworkElement)element;
                if (frameworkElement.DataContext is IRegionElement)
                {
                    IRegionElement regionElement = (IRegionElement)frameworkElement.DataContext;
                    fromDataContext = regionElement.CanNavigateFrom();
                }
            }

            return fromBase && fromDataContext;
        }

        /// <inheritdoc />
        protected override bool CanNavigateTo (object container, object element)
        {
            bool fromBase = base.CanNavigateTo(container, element);
            bool fromDataContext = true;

            if (element is FrameworkElement)
            {
                FrameworkElement frameworkElement = (FrameworkElement)element;
                if (frameworkElement.DataContext is IRegionElement)
                {
                    IRegionElement regionElement = (IRegionElement)frameworkElement.DataContext;
                    fromDataContext = regionElement.CanNavigateTo();
                }
            }

            return fromBase && fromDataContext;
        }

        /// <inheritdoc />
        protected override void NavigatedFrom (object container, object element)
        {
            base.NavigatedFrom(container, element);

            if (element is FrameworkElement)
            {
                FrameworkElement frameworkElement = (FrameworkElement)element;
                if (frameworkElement.DataContext is IRegionElement)
                {
                    IRegionElement regionElement = (IRegionElement)frameworkElement.DataContext;
                    regionElement.NavigatedFrom();
                }
            }
        }

        /// <inheritdoc />
        protected override void NavigatedTo (object container, object element)
        {
            base.NavigatedTo(container, element);

            if (element is FrameworkElement)
            {
                FrameworkElement frameworkElement = (FrameworkElement)element;
                if (frameworkElement.DataContext is IRegionElement)
                {
                    IRegionElement regionElement = (IRegionElement)frameworkElement.DataContext;
                    regionElement.NavigatedTo();
                }
            }
        }

        #endregion
    }
}
