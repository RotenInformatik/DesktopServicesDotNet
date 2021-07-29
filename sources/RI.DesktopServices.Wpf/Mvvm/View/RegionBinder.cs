using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

using RI.DesktopServices.UiContainer;




namespace RI.DesktopServices.Mvvm.View
{
    /// <summary>
    ///     The dependency property to assign a region to a control.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public static class RegionBinder
    {
        #region Static Fields

        /// <summary>
        ///     The dependency property.
        /// </summary>
        /// <value>
        ///     Gets dependency property
        /// </value>
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(DependencyObject), new FrameworkPropertyMetadata(defaultValue: null, propertyChangedCallback: RegionBinder.NameChanged, flags: FrameworkPropertyMetadataOptions.AffectsRender));

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
        public static string GetName(DependencyObject obj)
        {
            return (string)obj?.GetValue(RegionBinder.NameProperty);
        }

        /// <summary>
        ///     Gets the value of the dependency property.
        /// </summary>
        /// <param name="obj"> The object the dependency property is attached to. </param>
        /// <param name="value"> The new value of the dependency property. </param>
        public static void SetName(DependencyObject obj, string value)
        {
            obj?.SetValue(RegionBinder.NameProperty, value);
        }

        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
        private static void NameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(d))
            {
                string value = (string)e.NewValue;

                if ((!string.IsNullOrWhiteSpace(value)) && (d != null))
                {
                    IRegionService regionService = (IRegionService)RegionBinder.ServiceProvider?.GetService(typeof(IRegionService));
                    regionService?.AddRegion(value, d);

                    if (regionService == null)
                    {
                        Trace.TraceWarning($"No region service available for region {value} to add to {d.GetType().Name}");
                    }
                    else
                    {
                        Trace.TraceInformation($"Region {value} added to {d.GetType().Name}");
                    }
                }
                else
                {
                    Trace.TraceWarning($"Element or region missing");
                }
            }
        }

        #endregion
    }
}
