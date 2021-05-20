using System;
using System.Collections.Generic;




namespace RI.DesktopServices.UiContainer
{
    /// <summary>
    ///     Region adapter which mapps between containers (associated with aregion) and its elements.
    /// </summary>
    public interface IRegionAdapter
    {
        /// <summary>
        ///     Adds an element to a container.
        /// </summary>
        /// <param name="container"> The container. </param>
        /// <param name="element"> The element. </param>
        /// <remarks>
        ///     <note type="note">
        ///         Do not call this method directly, it is intended to be called from an <see cref="IRegionService" /> implementation.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="container" /> or <paramref name="element" /> is null. </exception>
        /// <exception cref="NotSupportedException"> The type of <paramref name="container" /> or <paramref name="element" /> is not supported by this region adapter or <paramref name="container" /> already has an element and does not support multiple elements. </exception>
        void Add (object container, object element);

        /// <summary>
        ///     Removes an element from a container.
        /// </summary>
        /// <param name="container"> The container. </param>
        /// <param name="element"> The element. </param>
        /// <remarks>
        ///     <note type="note">
        ///         Do not call this method directly, it is intended to be called from an <see cref="IRegionService" /> implementation.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="container" /> or <paramref name="element" /> is null. </exception>
        /// <exception cref="NotSupportedException"> The type of <paramref name="container" /> or <paramref name="element" /> is not supported by this region adapter. </exception>
        void Remove(object container, object element);

        /// <summary>
        ///     Removes all elements of a container
        /// </summary>
        /// <param name="container"> The container. </param>
        /// <remarks>
        ///     <note type="note">
        ///         Do not call this method directly, it is intended to be called from an <see cref="IRegionService" /> implementation.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="container" /> is null. </exception>
        /// <exception cref="NotSupportedException"> The type of <paramref name="container" /> is not supported by this region adapter. </exception>
        void Clear(object container);

        /// <summary>
        ///     Sorts all elements in a container according to their sort indices.
        /// </summary>
        /// <param name="container"> The container. </param>
        /// <remarks>
        ///     <note type="note">
        ///         Do not call this method directly, it is intended to be called from an <see cref="IRegionService" /> implementation.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="container" /> is null. </exception>
        /// <exception cref="NotSupportedException"> The type of <paramref name="container" /> is not supported by this region adapter. </exception>
        void Sort(object container);

        /// <summary>
        ///     Checks whether an element is in a container.
        /// </summary>
        /// <param name="container"> The container. </param>
        /// <param name="element"> The element. </param>
        /// <returns>
        ///     true if the container contains the element, false otherwise.
        /// </returns>
        /// <remarks>
        ///     <note type="note">
        ///         Do not call this method directly, it is intended to be called from an <see cref="IRegionService" /> implementation.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="container" /> or <paramref name="element" /> is null. </exception>
        /// <exception cref="NotSupportedException"> The type of <paramref name="container" /> or <paramref name="element" /> is not supported by this region adapter. </exception>
        bool Contains(object container, object element);

        /// <summary>
        ///     Gets all elements of a container
        /// </summary>
        /// <param name="container"> The container. </param>
        /// <returns>
        ///     The list which contains all the elements of the container.
        ///     An empty list is returned if the container contains no elements.
        /// </returns>
        /// <remarks>
        ///     <note type="note">
        ///         Do not call this method directly, it is intended to be called from an <see cref="IRegionService" /> implementation.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="container" /> is null. </exception>
        /// <exception cref="NotSupportedException"> The type of <paramref name="container" /> is not handled by this region adapter. </exception>
        List<object> Get(object container);

        /// <summary>
        ///     Activates an element after it was added to the container.
        /// </summary>
        /// <param name="container"> The container. </param>
        /// <param name="element"> The element. </param>
        /// <remarks>
        ///     <note type="note">
        ///         Do not call this method directly, it is intended to be called from an <see cref="IRegionService" /> implementation.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="container" /> or <paramref name="element" /> is null. </exception>
        /// <exception cref="NotSupportedException"> The type of <paramref name="container" /> or <paramref name="element" /> is not supported by this region adapter or <paramref name="container" /> already has an element and does not support multiple elements. </exception>
        void Activate (object container, object element);

        /// <summary>
        ///     Deactivates an element after it was added to the container.
        /// </summary>
        /// <param name="container"> The container. </param>
        /// <param name="element"> The element. </param>
        /// <remarks>
        ///     <note type="note">
        ///         Do not call this method directly, it is intended to be called from an <see cref="IRegionService" /> implementation.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="container" /> or <paramref name="element" /> is null. </exception>
        /// <exception cref="NotSupportedException"> The type of <paramref name="container" /> or <paramref name="element" /> is not supported by this region adapter or <paramref name="container" /> already has an element and does not support multiple elements. </exception>
        void Deactivate(object container, object element);

        /// <summary>
        ///     Determines whether the current and new element of a container allows navigation.
        /// </summary>
        /// <param name="container"> The container. </param>
        /// <param name="element"> The element. </param>
        /// <returns>
        ///     true if the current element allows navigation away from (no current element is an implicit &quot;allow&quot;) and the new element allows navigation to (no new element is an implicit &quot;allow&quot;), false otherwise.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         <paramref name="element" /> can be null to only navigate away from the current element but not to a new one, leaving the container without an element.
        ///     </para>
        ///     <note type="note">
        ///         Do not call this method directly, it is intended to be called from an <see cref="IRegionService" /> implementation.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="container" /> is null. </exception>
        /// <exception cref="NotSupportedException"> The type of <paramref name="container" /> or <paramref name="element" /> is not supported by this region adapter or <paramref name="container" /> already has an element and does not support multiple elements. </exception>
        bool CanNavigate (object container, object element);

        /// <summary>
        ///     Navigates from the current element to a new element in a container.
        /// </summary>
        /// <param name="container"> The container. </param>
        /// <param name="element"> The element. </param>
        /// <returns>
        ///     true if the navigation is was successful, false otherwise.
        ///     <see cref="CanNavigate" /> for more details.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         <paramref name="element" /> can be null to only navigate away from the current element but not to a new one, leaving the container without an element.
        ///     </para>
        ///     <note type="note">
        ///         Do not call this method directly, it is intended to be called from an <see cref="IRegionService" /> implementation.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="container" /> is null. </exception>
        bool Navigate (object container, object element);

        /// <summary>
        ///     Checks whether a specified type is supported by this region adapter for containers.
        /// </summary>
        /// <param name="type"> The type of the container. </param>
        /// <param name="inheritanceDepth"> Returns the depth of the inheritance this region adapter supports the specified type. </param>
        /// <returns>
        ///     true if this region adapter can use the specified type as container, false otherwise.
        /// </returns>
        /// <remarks>
        ///     <note type="note">
        ///         Do not call this method directly, it is intended to be called from an <see cref="IRegionService" /> implementation.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="type" /> is null. </exception>
        bool IsCompatibleContainer(Type type, out int inheritanceDepth);
    }
}
