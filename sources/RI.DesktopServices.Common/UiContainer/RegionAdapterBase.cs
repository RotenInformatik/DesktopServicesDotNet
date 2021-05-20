using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using RI.DesktopServices.Utilities;




namespace RI.DesktopServices.UiContainer
{
    /// <summary>
    ///     Boilerplate implementation of <see cref="IRegionAdapter" />.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public abstract class RegionAdapterBase : IRegionAdapter
    {
        #region Abstracts

        /// <summary>
        ///     Fills a list of types which are supported by this region adapter.
        /// </summary>
        /// <param name="types"> The list which is filled with the supported types by this region adapter. </param>
        protected abstract void GetSupportedTypes (List<Type> types);

        #endregion




        #region Virtuals

        /// <summary>
        ///     Checks whether the specified element can be navigated away from its current container.
        /// </summary>
        /// <param name="container"> The current container. </param>
        /// <param name="element"> The current element. </param>
        /// <returns>
        ///     true if the navigation is allowed, false otherwise.
        /// </returns>
        protected virtual bool CanNavigateFrom (object container, object element)
        {
            if (element is IRegionElement)
            {
                IRegionElement regionElement = (IRegionElement)element;
                return regionElement.CanNavigateFrom();
            }
            return true;
        }

        /// <summary>
        ///     Checks whether the specified element can be navigated to its new container.
        /// </summary>
        /// <param name="container"> The new container. </param>
        /// <param name="element"> The new element. </param>
        /// <returns>
        ///     true if the navigation is allowed, false otherwise.
        /// </returns>
        protected virtual bool CanNavigateTo (object container, object element)
        {
            if (element is IRegionElement)
            {
                IRegionElement regionElement = (IRegionElement)element;
                return regionElement.CanNavigateTo();
            }
            return true;
        }

        /// <summary>
        ///     Creates a list of elements where the elements are sorted according to their sort index.
        /// </summary>
        /// <param name="elements"> The sequence of elements to sort. </param>
        /// <returns>
        ///     The list of sorted elements.
        ///     An empty list is returned if the sequence contains no elements.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         <see cref="GetSortIndex" /> is used to retrieve the sort indices used to sort the elements.
        ///     </para>
        ///     <para>
        ///         <paramref name="elements"/> is only enumerated once.
        ///     </para>
        /// </remarks>
        protected virtual List<object> GetSortedElements (IEnumerable elements)
        {
            List<object> sorted = new List<object>();
            foreach (object element in elements)
            {
                sorted.Add(element);
            }
            sorted.Sort((x, y) =>
            {
                int xIndex = sorted.IndexOf(x);
                int yIndex = sorted.IndexOf(y);
                return this.GetSortIndex(x, xIndex == -1 ? null : xIndex).CompareTo(this.GetSortIndex(y, yIndex == -1 ? null : yIndex));
            });
            return sorted;
        }

        /// <summary>
        ///     Gets the sort index of an element.
        /// </summary>
        /// <param name="element"> The element. </param>
        /// <param name="indexInContainer"> The index of the element in its container or null if the index is not available. </param>
        /// <returns>
        ///     The sort index of the element.
        ///     <see cref="int.MaxValue" /> is returned if the element does not provide a sort index in any way.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The sort index of an element is retrieved using <see cref="IRegionElement.SortIndex" /> (high priority), <see cref="RegionElementSortHintAttribute" /> (medium priority), and the index of the element in the container (if available, low priority).
        ///     </para>
        /// </remarks>
        protected virtual int GetSortIndex (object element, int? indexInContainer)
        {
            if (element is IRegionElement)
            {
                IRegionElement regionElement = (IRegionElement)element;
                if (regionElement.SortIndex.HasValue)
                {
                    return regionElement.SortIndex.Value;
                }
            }

            object[] attributes = element.GetType().GetCustomAttributes(typeof(RegionElementSortHintAttribute), true);
            if (attributes.Length != 0)
            {
                return ((RegionElementSortHintAttribute)attributes[0]).Index;
            }

            if (indexInContainer.HasValue)
            {
                return indexInContainer.Value;
            }

            return int.MaxValue;
        }

        /// <summary>
        ///     Navigates the specified element away from its current container.
        /// </summary>
        /// <param name="container"> The current container. </param>
        /// <param name="element"> The current element. </param>
        protected virtual void NavigatedFrom (object container, object element)
        {
            if (element is IRegionElement)
            {
                IRegionElement regionElement = (IRegionElement)element;
                regionElement.NavigatedFrom();
            }
        }

        /// <summary>
        ///     Navigates the specified element to its new container.
        /// </summary>
        /// <param name="container"> The new container. </param>
        /// <param name="element"> The new element. </param>
        protected virtual void NavigatedTo (object container, object element)
        {
            if (element is IRegionElement)
            {
                IRegionElement regionElement = (IRegionElement)element;
                regionElement.NavigatedTo();
            }
        }

        #endregion




        #region Interface: IRegionAdapter

        /// <inheritdoc />
        public abstract void Add(object container, object element);

        /// <inheritdoc />
        public abstract void Remove(object container, object element);

        /// <inheritdoc />
        public abstract void Clear(object container);

        /// <inheritdoc />
        public abstract bool Contains(object container, object element);

        /// <inheritdoc />
        public abstract List<object> Get (object container);

        /// <inheritdoc />
        public virtual void Sort(object container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            List<object> existingElements = this.Get(container);
            List<object> sortedElements = this.GetSortedElements(existingElements);

            if (!sortedElements.SequenceEqual(existingElements))
            {
                this.Clear(container);
                foreach (object sortedElement in sortedElements)
                {
                    this.Add(container, sortedElement);
                }
            }
        }

        /// <inheritdoc />
        public virtual void Activate (object container, object element)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (element is IRegionElement)
            {
                IRegionElement regionElement = (IRegionElement)element;
                regionElement.Activated();
            }
        }

        /// <inheritdoc />
        public virtual void Deactivate (object container, object element)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (element is IRegionElement)
            {
                IRegionElement regionElement = (IRegionElement)element;
                regionElement.Deactivated();
            }
        }

        /// <inheritdoc />
        public virtual bool CanNavigate (object container, object element)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            List<object> currentElements = this.Get(container);
            foreach (object currentElement in currentElements)
            {
                if (!this.CanNavigateFrom(container, currentElement))
                {
                    return false;
                }
            }

            return this.CanNavigateTo(container, element);
        }

        /// <inheritdoc />
        public virtual bool Navigate (object container, object element)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            List<object> currentElements = this.Get(container);

            if (currentElements.Any(x => object.ReferenceEquals(x, element)))
            {
                return true;
            }

            if (!this.CanNavigate(container, element))
            {
                return false;
            }

            foreach (object currentElement in currentElements)
            {
                this.NavigatedFrom(container, currentElement);
            }

            this.Clear(container);

            if (element != null)
            {
                this.Add(container, element);
            }

            currentElements = this.Get(container);

            foreach (object currentElement in currentElements)
            {
                this.NavigatedTo(container, currentElement);
            }

            return true;
        }

        /// <inheritdoc />
        public virtual bool IsCompatibleContainer (Type type, out int inheritanceDepth)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            List<Type> supportedTypes = new List<Type>();
            this.GetSupportedTypes(supportedTypes);

            return type.GetBestMatchingType(out _, out inheritanceDepth, supportedTypes.ToArray());
        }

        #endregion
    }
}
