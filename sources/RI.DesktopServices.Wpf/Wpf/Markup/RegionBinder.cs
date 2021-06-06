using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

using RI.DesktopServices.UiContainer;




namespace RI.DesktopServices.Wpf.Markup
{
    /// <summary>
    /// The dependency property to assign a region to a control.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public static class RegionBinder
    {
        /// <summary>
        /// The dependency property.
        /// </summary>
        /// <value>
        /// Gets dependency property
        /// </value>
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string),
            typeof(DependencyObject),
            new FrameworkPropertyMetadata(defaultValue: null, propertyChangedCallback: RegionBinder.NameChanged,
                                          flags: FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets the value of the dependency property.
        /// </summary>
        /// <param name="obj">The object the dependency property is attached to.</param>
        /// <returns>The current value of the dependency property.</returns>
        public static string GetName (DependencyObject obj)
        {
            return (string)obj?.GetValue(RegionBinder.NameProperty);
        }

        /// <summary>
        /// Gets the value of the dependency property.
        /// </summary>
        /// <param name="obj">The object the dependency property is attached to.</param>
        /// <param name="value">The new value of the dependency property.</param>
        public static void SetName (DependencyObject obj, string value)
        {
            obj?.SetValue(RegionBinder.NameProperty, value);
        }

        /// <summary>
        /// Gets or sets the service provider used to resolve the region service.
        /// </summary>
        /// <value>
        /// The service provider used to resolve the region service.
        /// </value>
        public static IServiceProvider ServiceProvider { get; set; }

        private static void NameChanged (DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(d))
            {
                string oldRegion = e.OldValue as string;
                string newRegion = e.NewValue as string;

                if (string.Equals(oldRegion, newRegion, StringComparison.InvariantCulture))
                {
                    return;
                }

                if (!string.IsNullOrWhiteSpace(newRegion))
                {
                    Trace.TraceInformation($"Change region binding for element {d?.GetType().Name ?? "[null]"}: {oldRegion ?? "[null]"} -> {newRegion ?? "[null]"}");

                    IRegionService regionService = (IRegionService)RegionBinder.ServiceProvider?.GetService(typeof(IRegionService));
                    regionService?.RemoveRegion(oldRegion);
                    regionService?.AddRegion(newRegion, d);
                }
            }
        }
    }
}
