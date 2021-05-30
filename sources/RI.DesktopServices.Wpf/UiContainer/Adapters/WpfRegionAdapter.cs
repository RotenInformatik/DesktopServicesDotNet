using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;




namespace RI.DesktopServices.UiContainer.Adapters
{
    /// <summary>
    ///     Region adapter for common WPF controls.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The common WPF controls which are supported as containers by this region adapter are:
    ///         <see cref="ContentControl" />, <see cref="ItemsControl" />, <see cref="Panel" />.
    ///         All types derived from those are also supported.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public sealed class WpfRegionAdapter : WpfRegionAdapterBase
    {
        #region Overrides

        /// <inheritdoc />
        public override void Add (object container, object element)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (container is ContentControl contentControl)
            {
                if (contentControl.Content != null)
                {
                    throw new NotSupportedException("The container of type" + contentControl.GetType()
                                                        .Name +
                                                    " already has an element and does not support multiple elements.");
                }

                contentControl.Content = element;
            }
            else if (container is ItemsControl itemsControl)
            {
                itemsControl.Items.Remove(element);
                itemsControl.Items.Add(element);
            }
            else if ((container is Panel panel) && (element is UIElement uiElement))
            {
                panel.Children.Remove(uiElement);
                panel.Children.Add(uiElement);
            }
        }

        /// <inheritdoc />
        public override void Clear (object container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (container is ContentControl contentControl)
            {
                contentControl.Content = null;
            }
            else if (container is ItemsControl itemsControl)
            {
                itemsControl.Items.Clear();
            }
            else if (container is Panel panel)
            {
                panel.Children.Clear();
            }
        }

        /// <inheritdoc />
        public override bool Contains (object container, object element)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            bool contains = false;

            if (container is ContentControl contentControl)
            {
                contains = ReferenceEquals(contentControl.Content, element);
            }
            else if (container is ItemsControl itemsControl)
            {
                contains = itemsControl.Items.Contains(element);
            }
            else if ((container is Panel panel) && (element is UIElement uiElement))
            {
                contains = panel.Children.Contains(uiElement);
            }

            return contains;
        }

        /// <inheritdoc />
        public override List<object> Get (object container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            List<object> elements = new List<object>();

            if (container is ContentControl contentControl)
            {
                elements.Add(contentControl.Content);
            }
            else if (container is ItemsControl itemsControl)
            {
                foreach (object element in itemsControl.Items)
                {
                    elements.Add(element);
                }
            }
            else if (container is Panel panel)
            {
                foreach (object element in panel.Children)
                {
                    elements.Add(element);
                }
            }

            return elements;
        }

        /// <inheritdoc />
        public override void Remove (object container, object element)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (container is ContentControl contentControl)
            {
                if (ReferenceEquals(contentControl.Content, element))
                {
                    contentControl.Content = null;
                }
            }
            else if (container is ItemsControl itemsControl)
            {
                itemsControl.Items.Remove(element);
            }
            else if ((container is Panel panel) && (element is UIElement uiElement))
            {
                panel.Children.Remove(uiElement);
            }
        }

        /// <inheritdoc />
        protected override void GetSupportedTypes (List<Type> types)
        {
            types.Add(typeof(ContentControl));
            types.Add(typeof(ItemsControl));
            types.Add(typeof(Panel));
        }

        #endregion
    }
}
