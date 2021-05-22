using System.Diagnostics;
using System.Windows;

using RI.Framework.Services.Regions;




namespace RI.Framework.Windows.Wpf.Markup
{
	/// <summary>
	///     Provides attached properties to associate a WPF control with a region of a <see cref="IRegionService" />.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="RegionBinder" /> is a convenience utility to work with regions (<see cref="IRegionService" />) in MVVM scenarios.
	///         It defines an attached property (<see cref="RegionNameProperty" />) which can be used to associate a container with a region (using the region services <see cref="IRegionService.AddRegion" /> method).
	///         It also defines an attached property (<see cref="RegionServiceProperty" />) to specify the used <see cref="IRegionService" />.
	///     </para>
	///     <para>
	///         The used <see cref="IRegionService" /> is determined in the following order:
	///         If <see cref="RegionServiceProperty" /> is not null, that instance is used.
	///         If <see cref="DefaultRegionService" /> is not null, that instance is used.
	///         <see cref="RegionLocator" /> is used if neither <see cref="RegionServiceProperty" /> nor <see cref="DefaultRegionService" /> is set.
	///     </para>
	/// </remarks>
	public static class RegionBinder
	{
		#region Constants

		/// <summary>
		///     Associates a container with a region by specifying its region name.
		/// </summary>
		public static readonly DependencyProperty RegionNameProperty = DependencyProperty.RegisterAttached("RegionName", typeof(string), typeof(RegionBinder), new UIPropertyMetadata(null, RegionBinder.OnRegionNameChange));

		/// <summary>
		///     Associates a container with a region service.
		/// </summary>
		public static readonly DependencyProperty RegionServiceProperty = DependencyProperty.RegisterAttached("RegionService", typeof(IRegionService), typeof(RegionBinder), new UIPropertyMetadata(null, RegionBinder.OnRegionServiceChange));

		#endregion




		#region Static Properties/Indexer

		/// <summary>
		///     Gets or sets the default region service to use.
		/// </summary>
		/// <value>
		///     The default region service to use.
		/// </value>
		/// <remarks>
		///     <para>
		///         The default value is null.
		///     </para>
		/// </remarks>
		public static IRegionService DefaultRegionService { get; set; }

		#endregion




		#region Static Methods

		/// <summary>
		///     Gets the region name of the specified container.
		/// </summary>
		/// <param name="obj"> The container. </param>
		/// <returns>
		///     The region name associated with the container or null if the container has no region name associated.
		/// </returns>
		/// <remarks>
		///     <note type="note">
		///         This method is for supporting the XAML designer and not intended to be used by your code.
		///     </note>
		/// </remarks>
		public static string GetRegionName (DependencyObject obj)
		{
			return obj?.GetValue(RegionBinder.RegionNameProperty) as string;
		}

		/// <summary>
		///     Gets the region service of the specified container.
		/// </summary>
		/// <param name="obj"> The container. </param>
		/// <returns>
		///     The region service associated with the container or null if <see cref="DefaultRegionService" /> or <see cref="RegionLocator" /> is used.
		/// </returns>
		/// <remarks>
		///     <note type="note">
		///         This method is for supporting the XAML designer and not intended to be used by your code.
		///     </note>
		/// </remarks>
		public static IRegionService GetRegionService (DependencyObject obj)
		{
			return obj?.GetValue(RegionBinder.RegionServiceProperty) as IRegionService;
		}

		/// <summary>
		///     Sets the region name of the specified container.
		/// </summary>
		/// <param name="obj"> The container. </param>
		/// <param name="value"> The region name to associate with the container. Can be null to unassociate the container from a region. </param>
		/// <remarks>
		///     <note type="note">
		///         This method is for supporting the XAML designer and not intended to be used by your code.
		///     </note>
		/// </remarks>
		public static void SetRegionName (DependencyObject obj, string value)
		{
			obj?.SetValue(RegionBinder.RegionNameProperty, value);
		}

		/// <summary>
		///     Sets the region service of the specified container.
		/// </summary>
		/// <param name="obj"> The container. </param>
		/// <param name="value"> The region service to associate with the container. Can be null to use <see cref="DefaultRegionService" /> or <see cref="RegionLocator" />. </param>
		/// <remarks>
		///     <note type="note">
		///         This method is for supporting the XAML designer and not intended to be used by your code.
		///     </note>
		/// </remarks>
		public static void SetRegionService (DependencyObject obj, IRegionService value)
		{
			obj?.SetValue(RegionBinder.RegionServiceProperty, value);
		}

		private static void OnRegionNameChange (DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			string oldRegion = e.OldValue as string;
			string newRegion = e.NewValue as string;

			if (RegionService.RegionNameComparer.Equals(oldRegion, newRegion))
			{
				return;
			}

			IRegionService regionService = RegionBinder.GetRegionService(obj) ?? RegionBinder.DefaultRegionService ?? RegionLocator.Service;
			if (regionService == null)
			{
				Trace.TraceWarning("No region service available while trying to assign region name: Object={0}, OldRegion={1}, NewRegion={2}", obj?.GetType().Name ?? "[null]", oldRegion ?? "[null]", newRegion ?? "[null]");
				return;
			}

			Trace.TraceInformation("Assigning region name: Object={0}, OldRegion={1}, NewRegion={2}, RegionService={3}", obj?.GetType().Name ?? "[null]", oldRegion ?? "[null]", newRegion ?? "[null]", regionService.GetType().Name);

			if (oldRegion != null)
			{
				regionService.RemoveRegion(oldRegion);
			}

			if (newRegion != null)
			{
				regionService.AddRegion(newRegion, obj);
			}
		}

		private static void OnRegionServiceChange (DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			IRegionService oldService = e.OldValue as IRegionService;
			IRegionService newService = e.NewValue as IRegionService;

			IRegionService oldServiceResolved = oldService ?? RegionBinder.DefaultRegionService ?? RegionLocator.Service;
			IRegionService newServiceResolved = newService ?? RegionBinder.DefaultRegionService ?? RegionLocator.Service;

			if (object.ReferenceEquals(oldServiceResolved, newServiceResolved))
			{
				return;
			}

			string regionName = RegionBinder.GetRegionName(obj);

			Trace.TraceInformation("Assigning region service: Object={0}, OldService={1}, NewService={2}, RegionName={3}", obj?.GetType().Name ?? "[null]", oldServiceResolved?.GetType().Name ?? "[null]", newServiceResolved?.GetType().Name ?? "[null]", regionName ?? "[null]");

			if (regionName != null)
			{
				oldServiceResolved?.RemoveRegion(regionName);
				newServiceResolved?.AddRegion(regionName, obj);
			}
		}

		#endregion
	}
}
