using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;




namespace RI.DesktopServices.Mvvm.ViewModel
{
    /// <summary>
    ///     The dependency property to auto-wire the view model for a view.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public static class ViewModelLocator
    {
        #region Static Fields

        /// <summary>
        ///     The dependency property.
        /// </summary>
        /// <value>
        ///     Gets dependency property
        /// </value>
        public static readonly DependencyProperty AutoWireViewModelProperty = DependencyProperty.RegisterAttached("AutoWireViewModel", typeof(bool?), typeof(ViewModelLocator), new PropertyMetadata(defaultValue: null, propertyChangedCallback: ViewModelLocator.AutoWireViewModelChanged));

        #endregion




        #region Static Properties/Indexer

        /// <summary>
        ///     Gets or sets the service provider used to resolve the region service.
        /// </summary>
        /// <value>
        ///     The service provider used to resolve the region service.
        /// </value>
        public static IServiceProvider ServiceProvider { get; set; }

        #endregion




        #region Static Methods

        /// <summary>
        ///     Gets the value of the dependency property.
        /// </summary>
        /// <param name="obj"> The object the dependency property is attached to. </param>
        /// <returns> The current value of the dependency property. </returns>
        public static bool? GetAutoWireViewModel(DependencyObject obj)
        {
            return (bool?)obj.GetValue(ViewModelLocator.AutoWireViewModelProperty);
        }

        /// <summary>
        ///     Gets the value of the dependency property.
        /// </summary>
        /// <param name="obj"> The object the dependency property is attached to. </param>
        /// <param name="value"> The new value of the dependency property. </param>
        public static void SetAutoWireViewModel(DependencyObject obj, bool? value)
        {
            obj.SetValue(ViewModelLocator.AutoWireViewModelProperty, value);
        }

        private static void AutoWireViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool? value = (bool?)e.NewValue;

            if (value.HasValue && value.Value && d is FrameworkElement view)
            {
                object viewModel = ViewModelLocator.ResolveViewModel(view.GetType(), DesignerProperties.GetIsInDesignMode(d));

                if (viewModel == null)
                {
                    Trace.TraceWarning($"No view model resolved for {view.GetType().Name}");
                }
                else
                {
                    Trace.TraceInformation($"View model {viewModel.GetType().Name} resolved for {view.GetType().Name}");
                }

                view.DataContext = viewModel;
            }
        }

        private static object ResolveViewModel(Type viewType, bool designTime)
        {
            if (designTime)
            {
                return null;
            }

            string viewName = viewType.FullName;
            string viewAssemblyName = viewType.Assembly.FullName;
            string viewModelLocation = viewName?.Replace(".Views.", ".ViewModels.");
            string suffix = (viewModelLocation?.EndsWith("View")).GetValueOrDefault() ? "Model" : "ViewModel";

            if (viewModelLocation == null)
            {
                return null;
            }

            string viewModelName = string.Format(CultureInfo.InvariantCulture, "{0}{1}, {2}", viewModelLocation, suffix, viewAssemblyName);
            Type viewModelType = Type.GetType(viewModelName);

            if (viewModelType == null)
            {
                return null;
            }

            object viewModel = ViewModelLocator.ServiceProvider.GetService(viewModelType);

            if (viewModel is IViewModel model)
            {
                if (!model.IsInitialized)
                {
                    model.Initialize();
                }
            }

            return viewModel;
        }

        #endregion
    }
}
