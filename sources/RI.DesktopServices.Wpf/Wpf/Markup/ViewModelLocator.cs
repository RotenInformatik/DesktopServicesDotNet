using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;

using RI.DesktopServices.Mvvm.View;
using RI.DesktopServices.Mvvm.ViewModel;




namespace RI.DesktopServices.Wpf.Markup
{
    /// <summary>
    /// The dependency property to auto-wire the view model for a view.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The resolve itself is done by the specified <see cref="ServiceProvider"/>.
    /// This is a static singleton which must be set prior to the first use of <see cref="ViewModelLocator"/>.
    /// </para>
    /// <para>
    /// The view model name/type to wire (with the view the <see cref="AutoWireViewModelProperty"/> is attached to) is determined by conventions.
    /// <see cref="ViewModelLocator"/> searches for the name of the view (e.g. "Views/MainView") and then converts it to the name of the view model, which is expected (according to the preceding example) to be "ViewModels/MainViewModel".
    /// </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public static class ViewModelLocator
    {
        /// <summary>
        /// Gets the dependency property.
        /// </summary>
        /// <value>
        /// The dependency property.
        /// </value>
        public static readonly DependencyProperty AutoWireViewModelProperty = DependencyProperty.RegisterAttached("AutoWireViewModel", typeof(bool?), typeof(ViewModelLocator), new FrameworkPropertyMetadata(defaultValue: null, propertyChangedCallback: ViewModelLocator.AutoWireViewModelChanged,
        flags: FrameworkPropertyMetadataOptions.AffectsArrange |
        FrameworkPropertyMetadataOptions.AffectsMeasure |
        FrameworkPropertyMetadataOptions.AffectsRender |
        FrameworkPropertyMetadataOptions.AffectsParentArrange |
        FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        /// <summary>
        /// Gets the value of the dependency property.
        /// </summary>
        /// <param name="obj">The object the dependency property is attached to.</param>
        /// <returns>The current value of the dependency property.</returns>
        public static bool? GetAutoWireViewModel (DependencyObject obj)
        {
            return (bool?)obj.GetValue(ViewModelLocator.AutoWireViewModelProperty);
        }

        /// <summary>
        /// Gets the value of the dependency property.
        /// </summary>
        /// <param name="obj">The object the dependency property is attached to.</param>
        /// <param name="value">The new value of the dependency property.</param>
        public static void SetAutoWireViewModel (DependencyObject obj, bool? value)
        {
            obj.SetValue(ViewModelLocator.AutoWireViewModelProperty, value);
        }

        /// <summary>
        /// Gets or sets the service provider used to resolve view models.
        /// </summary>
        /// <value>
        /// The service provider used to resolve view models.
        /// </value>
        public static IServiceProvider ServiceProvider { get; set; }

        private static void AutoWireViewModelChanged (DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool? value = (bool?)e.NewValue;

            if (value.HasValue && value.Value && !DesignerProperties.GetIsInDesignMode(d) && d is FrameworkElement view)
            {
                object viewModel = ResolveViewModel(view.GetType());

                ProcessValue(viewModel);

                if (d is IView iview)
                {
                    if (viewModel is IViewModel iviewModel)
                    {
                        iview.ViewModel = iviewModel;
                    }
                    else
                    {
                        iview.ViewModel = null;
                    }
                }
                else
                {
                    view.DataContext = viewModel;
                }

                ProcessValue(d);
            }
        }

        private static object ResolveViewModel (Type viewType)
        {
            string viewName = viewType.FullName;
            string viewAssemblyName = viewType.Assembly.FullName;
            string viewModelLocation = viewName?.Replace(".Views.", ".ViewModels.");
            string suffix = (viewModelLocation?.EndsWith("View")).GetValueOrDefault() ? "Model" : "ViewModel";

            Trace.TraceInformation($"Resolving view model location for view: {viewType.Name} at {viewModelLocation ?? "[null]"} with suffix {suffix ?? "[null]"}");

            if (viewModelLocation == null)
            {
                Trace.TraceWarning($"No view model location found for view: {viewType.Name}");
                return null;
            }

            string viewModelName = string.Format(CultureInfo.InvariantCulture, "{0}{1}, {2}", viewModelLocation, suffix, viewAssemblyName);

            Trace.TraceInformation($"Resolving view model type for view: {viewType.Name} in {viewModelName ?? "[null]"}");

            Type viewModelType = Type.GetType(viewModelName);

            if (viewModelType == null)
            {
                Trace.TraceWarning($"No view model type found for view: {viewType.Name}");
                return null;
            }

            Trace.TraceInformation($"Resolving view model instance for view: {viewType.Name} as {viewModelType.GetType().Name}");

            object viewModel = ViewModelLocator.ServiceProvider.GetService(viewModelType);

            if (viewModel == null)
            {
                Trace.TraceWarning($"No view model instance found for view: {viewType.Name}");
            }
            else
            {
                Trace.TraceInformation($"Resolved view model for view: {viewType.Name} <-> {viewModel.GetType().Name}");
            }

            return viewModel;
        }

        private static void ProcessValue (object value)
        {
            if (value == null)
            {
                return;
            }

            if (value is IViewModel { IsInitialized: false, } model)
            {
                Trace.TraceInformation($"Initializing view model {model.GetType().Name} while locating view model");
                model.Initialize();
                Trace.TraceInformation($"Initialized view model {model.GetType().Name} while locating view model");
            }

            if (value is IView { IsInitialized: false, } view)
            {
                Trace.TraceInformation($"Initializing view {view.GetType().Name} while locating view model");
                view.Initialize();
                Trace.TraceInformation($"Initialized view {view.GetType().Name} while locating view model");
            }

            if (value is FrameworkElement { IsInitialized: false, } frameworkElement)
            {
                if (frameworkElement.DataContext is IViewModel {IsInitialized: false,} dataContext)
                {
                    Trace.TraceInformation($"Initializing data context {dataContext.GetType().Name} while locating view model");
                    dataContext.Initialize();
                    Trace.TraceInformation($"Initialized data context {dataContext.GetType().Name} while locating view model");
                }
            }
        }
    }
}
