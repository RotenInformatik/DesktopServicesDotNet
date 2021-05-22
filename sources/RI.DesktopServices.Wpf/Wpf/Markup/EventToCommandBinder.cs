using System;
using System.Windows;




namespace RI.Framework.Windows.Wpf.Markup
{
	/// <summary>
	///     Provides attached properties to bind events to commands.
	/// </summary>
	public static class EventToCommandBinder
	{
		#region Constants

		/// <summary>
		///     The attached property which contains all the event-to-command bindings (<see cref="EventBinding" />) in a collection (<see cref="EventBindings" />).
		/// </summary>
		public static readonly DependencyProperty EventBindingsProperty = DependencyProperty.RegisterAttached("EventBindingsInternal", typeof(EventBindings), typeof(EventToCommandBinder), new UIPropertyMetadata(null, EventToCommandBinder.OnEventBindingsChanged));

		#endregion




		#region Static Methods

		/// <summary>
		///     Gets the collection of event-to-command bindings (<see cref="EventBinding" />, <see cref="EventBindings" />) for a dependency object.
		/// </summary>
		/// <param name="item"> The dependency object. </param>
		/// <returns>
		///     The collection of event-to-command bindings.
		/// </returns>
		/// <remarks>
		///     <para>
		///         If the dependency object does not already have an instance of <see cref="EventBindings" /> attached, a new one is created and attached.
		///     </para>
		/// </remarks>
		public static EventBindings GetEventBindings (DependencyObject item)
		{
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			EventBindings collection = item.GetValue(EventToCommandBinder.EventBindingsProperty) as EventBindings;

			if (collection == null)
			{
				collection = new EventBindings(item);
				item.SetValue(EventToCommandBinder.EventBindingsProperty, collection);
			}

			return collection;
		}

		private static void OnEventBindingsChanged (DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
		}

		#endregion
	}
}
