using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;




namespace RI.DesktopServices.Wpf.Controls
{
    /// <summary>
    ///     A specialized <see cref="CheckBox" /> which is connected to a <see cref="ComboBox" /> in order to set its
    ///     <see cref="Selector.SelectedItem" /> property to null.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         When the <see cref="ComboBoxChecker" /> is checked and the <see cref="ComboBox" />.
    ///         <see cref="Selector.SelectedItem" /> property is null, <see cref="Selector.SelectedItem" /> is set to the first
    ///         item in <see cref="ComboBox" />.<see cref="ItemsControl.ItemsSource" /> or <see cref="ComboBox" />.
    ///         <see cref="ItemsControl.Items" />.
    ///         When the <see cref="ComboBoxChecker" /> is unchecked, the <see cref="ComboBox" />.
    ///         <see cref="Selector.SelectedItem" /> property is set to null.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public class ComboBoxChecker : CheckBox
    {
        #region Static Fields

        /// <summary>
        ///     The dependency property for the <see cref="ComboBox" /> property.
        /// </summary>
        /// <value>
        ///     The dependency property for the <see cref="ComboBox" /> property.
        /// </value>
        public static readonly DependencyProperty ComboBoxProperty =
            DependencyProperty.Register(nameof(ComboBoxChecker.ComboBox), typeof(ComboBox), typeof(ComboBoxChecker),
                                        new FrameworkPropertyMetadata(ComboBoxChecker.OnComboBoxChanged));

        #endregion




        #region Static Methods

        private static void OnComboBoxChanged (DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((ComboBoxChecker)obj).UnbindEvents((ComboBox)args.OldValue);
            ((ComboBoxChecker)obj).BindEvents((ComboBox)args.NewValue);
            ((ComboBoxChecker)obj).UpdateChecked();
        }

        #endregion




        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="ComboBoxChecker" />.
        /// </summary>
        public ComboBoxChecker ()
        {
            this.SelectionChangedHandler = this.SelectionChangedMethod;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets or sets the <see cref="ComboBox" /> connected to this <see cref="ComboBoxChecker" />.
        /// </summary>
        /// <value>
        ///     The <see cref="ComboBox" /> connected to this <see cref="ComboBoxChecker" />.
        /// </value>
        public ComboBox ComboBox
        {
            get
            {
                return (ComboBox)this.GetValue(ComboBoxChecker.ComboBoxProperty);
            }
            set
            {
                this.SetValue(ComboBoxChecker.ComboBoxProperty, value);
            }
        }

        private SelectionChangedEventHandler SelectionChangedHandler { get; set; }

        #endregion




        #region Instance Methods

        private void BindEvents (ComboBox newValue)
        {
            if (newValue != null)
            {
                newValue.SelectionChanged += this.SelectionChangedHandler;
            }
        }

        private void SelectionChangedMethod (object sender, SelectionChangedEventArgs e)
        {
            this.UpdateChecked();
        }

        private void UnbindEvents (ComboBox oldValue)
        {
            if (oldValue != null)
            {
                oldValue.SelectionChanged -= this.SelectionChangedHandler;
            }
        }

        private void UpdateChecked ()
        {
            this.IsChecked = this.ComboBox?.SelectedItem != null;
        }

        #endregion




        #region Overrides

        /// <inheritdoc />
        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
        protected override void OnChecked (RoutedEventArgs e)
        {
            base.OnChecked(e);

            if (this.ComboBox == null)
            {
                return;
            }

            if (this.ComboBox.SelectedItem == null)
            {
                if (this.ComboBox.ItemsSource != null)
                {
                    this.ComboBox.SelectedItem = this.ComboBox.ItemsSource.Cast<object>()
                                                     .FirstOrDefault();
                }
                else if (this.ComboBox.Items != null)
                {
                    this.ComboBox.SelectedItem = this.ComboBox.Items.Cast<object>()
                                                     .FirstOrDefault();
                }
            }
        }

        /// <inheritdoc />
        protected override void OnUnchecked (RoutedEventArgs e)
        {
            base.OnUnchecked(e);

            if (this.ComboBox == null)
            {
                return;
            }

            if (this.ComboBox.SelectedItem != null)
            {
                this.ComboBox.SelectedItem = null;
            }
        }

        #endregion
    }
}
