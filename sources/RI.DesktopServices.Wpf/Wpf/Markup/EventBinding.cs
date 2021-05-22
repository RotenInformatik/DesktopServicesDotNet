using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

using RI.Framework.Utilities;




namespace RI.Framework.Windows.Wpf.Markup
{
	/// <summary>
	///     Defines a single event-to-command binding used with <see cref="EventToCommandBinder" />.
	/// </summary>
	public sealed class EventBinding : Freezable
	{
		#region Static Fields

		/// <summary>
		///     Defines the command to be executed.
		/// </summary>
		public static DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(EventBinding), new UIPropertyMetadata(null, EventBinding.OnCommandChanged));

		/// <summary>
		///     Defines the event to be handled.
		/// </summary>
		public static DependencyProperty EventNameProperty = DependencyProperty.Register("EventName", typeof(string), typeof(EventBinding), new UIPropertyMetadata(null, EventBinding.OnEventNameChanged));

		#endregion




		#region Static Methods

		private static void OnCommandChanged (DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			(obj as EventBinding)?.ReAttach();
		}

		private static void OnEventNameChanged (DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			(obj as EventBinding)?.ReAttach();
		}

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="EventBinding" />.
		/// </summary>
		public EventBinding ()
		{
			this.AttachedTo = null;
			this.EventInfo = null;
			this.EventHandlerDelegate = null;

			this.EventHandlerMethod = this.GetType().GetMethod(nameof(this.OnEvent), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets or sets the command to be executed.
		/// </summary>
		/// <value>
		///     The command to be executed.
		/// </value>
		public ICommand Command
		{
			get
			{
				return (ICommand)this.GetValue(EventBinding.CommandProperty);
			}
			set
			{
				this.SetValue(EventBinding.CommandProperty, value);
			}
		}

		/// <summary>
		///     Gets or sets the event to be handled.
		/// </summary>
		/// <value>
		///     The event to be handled.
		/// </value>
		public string EventName
		{
			get
			{
				return (string)this.GetValue(EventBinding.EventNameProperty);
			}
			set
			{
				this.SetValue(EventBinding.EventNameProperty, value);
			}
		}

		private DependencyObject AttachedTo { get; set; }
		private Delegate EventHandlerDelegate { get; set; }
		private MethodInfo EventHandlerMethod { get; set; }
		private EventInfo EventInfo { get; set; }

		#endregion




		#region Instance Methods

		internal void Attach (DependencyObject attachedTo)
		{
			this.Detach();

			if (this.Command == null)
			{
				return;
			}

			if (this.EventName.IsNullOrEmptyOrWhitespace())
			{
				return;
			}

			this.AttachedTo = attachedTo;
			if (this.AttachedTo == null)
			{
				return;
			}

			this.EventInfo = this.AttachedTo.GetType().GetEvent(this.EventName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (this.EventInfo == null)
			{
				return;
			}

			this.EventHandlerDelegate = Delegate.CreateDelegate(this.EventInfo.EventHandlerType, this, this.EventHandlerMethod);

			this.EventInfo.AddEventHandler(this.AttachedTo, this.EventHandlerDelegate);
		}

		internal void Detach ()
		{
			this.EventInfo?.RemoveEventHandler(this.AttachedTo, this.EventHandlerDelegate);

			this.EventHandlerDelegate = null;
			this.EventInfo = null;
			this.AttachedTo = null;
		}

		internal void ReAttach ()
		{
			this.Attach(this.AttachedTo);
		}

		[SuppressMessage("ReSharper", "UnusedParameter.Local")]
		private void OnEvent (object sender, object eventArgs)
		{
			if (this.Command.CanExecute(eventArgs))
			{
				this.Command.Execute(eventArgs);
			}
		}

		#endregion




		#region Overrides

		/// <inheritdoc />
		protected override Freezable CreateInstanceCore ()
		{
			return new EventBinding();
		}

		#endregion
	}
}
