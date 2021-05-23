using System;
using System.Collections.Generic;




namespace RI.DesktopServices.UiContainer
{
    /// <summary>
    ///     Defines the interface for a region service.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A region service is used to modularize the user interface of an application.
    ///         The modularization is done by defining regions in the user interface into which modules of the application can
    ///         insert their user interface content by only knowing the name of the region.
    ///         This allows application modules to implement their own user interface without any dependency to the overall
    ///         user interface structure.
    ///     </para>
    ///     <para>
    ///         Each region is represented by a container into which elements are inserted.
    ///         The region service uses <see cref="IRegionAdapter" /> to do the mapping between the containers and elements so
    ///         they do not have a dependency on each other.
    ///         The supported types for containers and elements are defined by the available region adapters.
    ///     </para>
    ///     <para>
    ///         A region is associated with exactly one container and always identified by its name.
    ///         A region (or its container respectively) can have zero, one, or multiple elements, depending on the kind of
    ///         region operations which are performed.
    ///     </para>
    ///     <para>
    ///         Names of regions are considered case-insensitive.
    ///     </para>
    ///     <para>
    ///         There are three basic kind of operations which can be performed with regions: Navigation, Activation,
    ///         Adding/Removing.
    ///     </para>
    ///     <para>
    ///         Navigation is used when a region only contains zero or one element and where the elements need to be aware of
    ///         the navigation (e.g. allowing or disallowing the change of the current element).
    ///         An example for navigation are contextual views.
    ///     </para>
    ///     <para>
    ///         Activation is used when a region contains zero, one, or more elements and where zero, one, or more elements can
    ///         be active elements.
    ///         An example for activation are tabs where multiple tabs can exist but only one can be the active tab.
    ///     </para>
    ///     <para>
    ///         Adding/Removing are the most basic operations where simply zero, one, or more elements are added to or removed
    ///         from a region.
    ///         An example for adding/removing are toolbars.
    ///     </para>
    ///     <para>
    ///         For regions and elements where the order of elements matter, the elements can either implement
    ///         <see cref="IRegionElement" /> or apply <see cref="RegionElementSortHintAttribute" />.
    ///     </para>
    /// </remarks>
    public interface IRegionService
    {
        /// <summary>
        ///     Activates all elements in a region.
        /// </summary>
        /// <param name="region"> The name of the region. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="region" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="region" /> is an empty string. </exception>
        /// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
        void ActivateAllElements (string region);

        /// <summary>
        ///     Activates an element in a region.
        /// </summary>
        /// <param name="region"> The name of the region. </param>
        /// <param name="element"> The element which is to be activated in the region. </param>
        /// <remarks>
        ///     <para>
        ///         If the element is not in the region, it will be added before being activated.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="region" /> is an empty string. </exception>
        /// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
        void ActivateElement (string region, object element);

        /// <summary>
        ///     Adds a region adapter.
        /// </summary>
        /// <param name="regionAdapter"> The region adapter to add. </param>
        /// <remarks>
        ///     <note type="implement">
        ///         Specifying an already added region adapter should have no effect.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="regionAdapter" /> is null. </exception>
        void AddAdapter (IRegionAdapter regionAdapter);

        /// <summary>
        ///     Adds an element to a region.
        /// </summary>
        /// <param name="region"> The name of the region. </param>
        /// <param name="element"> The element to add to the region. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="region" /> is an empty string. </exception>
        /// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
        /// <exception cref="NotSupportedException">
        ///     The container associated to <paramref name="region" /> already has an element
        ///     and does not support multiple elements.
        /// </exception>
        void AddElement (string region, object element);

        /// <summary>
        ///     Adds a region and associates it with a container.
        /// </summary>
        /// <param name="region"> The name of the region. </param>
        /// <param name="container"> The container which represents the region. </param>
        /// <remarks>
        ///     <note type="implement">
        ///         An already existing region with the same name should be overwritten or removed first respectively.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="container" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="region" /> is an empty string. </exception>
        /// <exception cref="NotSupportedException">
        ///     The type of <paramref region="container" /> is not supported by any region
        ///     adapter.
        /// </exception>
        void AddRegion (string region, object container);

        /// <summary>
        ///     Determines whether the specified region can be navigated away from its current element to a specified new element.
        /// </summary>
        /// <param name="region"> The name of the region. </param>
        /// <param name="element"> The element to navigate to. </param>
        /// <returns>
        ///     true if the current element allows navigation away from (no current element is an implicit &quot;allow&quot;) and
        ///     the new element allows navigation to (no new element is an implicit &quot;allow&quot;), false otherwise.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         <paramref name="element" /> can be null to only navigate away from the current element but not to a new one,
        ///         leaving the region without an navigated to element.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="region" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="region" /> is an empty string. </exception>
        /// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
        bool CanNavigate (string region, object element);

        /// <summary>
        ///     Removes all elements from a region.
        /// </summary>
        /// <param name="region"> The name of the region. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="region" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="region" /> is an empty string. </exception>
        /// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
        void ClearElements (string region);

        /// <summary>
        ///     Deactivates all elements in a region.
        /// </summary>
        /// <param name="region"> The name of the region. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="region" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="region" /> is an empty string. </exception>
        /// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
        void DeactivateAllElements (string region);

        /// <summary>
        ///     Deactivates an element in a region.
        /// </summary>
        /// <param name="region"> The name of the region. </param>
        /// <param name="element"> The element which is to be deactivated in the region. </param>
        /// <remarks>
        ///     <para>
        ///         If the element is not in the region, it will be added before being deactivated.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="region" /> is an empty string. </exception>
        /// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
        void DeactivateElement (string region, object element);

        /// <summary>
        ///     Gets all used region adapters.
        /// </summary>
        /// <returns>
        ///     The list with all used region adapters.
        ///     If no region adapters are used, an empty list is returned.
        /// </returns>
        List<IRegionAdapter> GetAdapters ();

        /// <summary>
        ///     Gets the container associated with a region.
        /// </summary>
        /// <param name="region"> The name of the region. </param>
        /// <returns>
        ///     The container associated with the region or null if the region does not exist.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="region" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="region" /> is an empty string. </exception>
        object GetContainerOfRegion (string region);

        /// <summary>
        ///     Gets all elements of a region.
        /// </summary>
        /// <param name="region"> The name of the region. </param>
        /// <returns>
        ///     The list with all elements of the region.
        ///     An empty list is returned if the region contains no elements.
        ///     null is returned if the region does not exist.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="region" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="region" /> is an empty string. </exception>
        List<object> GetElementsOfRegion (string region);

        /// <summary>
        ///     Gets the first region name which is associated with a container.
        /// </summary>
        /// <param name="container"> The container. </param>
        /// <returns>
        ///     The first name of the region associated with the container or null if no region is associated with the container.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="container" /> is null. </exception>
        string GetRegionName (object container);

        /// <summary>
        ///     Gets all currently available regions.
        /// </summary>
        /// <returns>
        ///     The set with all region names.
        ///     An empty set is returned if no region is available.
        /// </returns>
        HashSet<string> GetRegionNames ();

        /// <summary>
        ///     Gets all region names which are associated with a container.
        /// </summary>
        /// <param name="container"> The container. </param>
        /// <returns>
        ///     The set with all names of regions associated with the container.
        ///     An empty set is returned if no region is associated with the container.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="container" /> is null. </exception>
        HashSet<string> GetRegionNames (object container);

        /// <summary>
        ///     Determines whether a region contains a specified element.
        /// </summary>
        /// <param name="region"> The name of the region. </param>
        /// <param name="element"> The element which is to be checked whether it is in the region. </param>
        /// <returns>
        ///     true if the element is in the specified region, false otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="region" /> is an empty string. </exception>
        /// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
        bool HasElement (string region, object element);

        /// <summary>
        ///     Determines whether a region exists.
        /// </summary>
        /// <param name="region"> The name of the region. </param>
        /// <returns>
        ///     true if the region exists, false otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="region" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="region" /> is an empty string. </exception>
        bool HasRegion (string region);

        /// <summary>
        ///     Navigates a region away from its current element to a specified different element.
        /// </summary>
        /// <param name="region"> The name of the region. </param>
        /// <param name="element"> The element to navigate to. </param>
        /// <returns>
        ///     true if the navigation was successful, false otherwise.
        ///     <see cref="CanNavigate" /> for more details.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         <paramref name="element" /> can be null to only navigate away from the current element but not to a new one,
        ///         leaving the region without an navigated to element.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="region" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="region" /> is an empty string. </exception>
        /// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
        bool Navigate (string region, object element);

        /// <summary>
        ///     Removes a region adapter.
        /// </summary>
        /// <param name="regionAdapter"> The region adapter to remove. </param>
        /// <remarks>
        ///     <note type="implement">
        ///         Specifying an already removed region adapter should have no effect.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="regionAdapter" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> The specified region adapter is still in use. </exception>
        void RemoveAdapter (IRegionAdapter regionAdapter);

        /// <summary>
        ///     Removes an element from a region.
        /// </summary>
        /// <param name="region"> The name of the region. </param>
        /// <param name="element"> The element to remove from the region. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="region" /> is an empty string. </exception>
        /// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
        void RemoveElement (string region, object element);

        /// <summary>
        ///     Removes a region and unassociates its container.
        /// </summary>
        /// <param name="region"> The name of the region. </param>
        /// <remarks>
        ///     <note type="implement">
        ///         Nothing should happen if the region does not exist.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="region" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="region" /> is an empty string. </exception>
        void RemoveRegion (string region);
    }
}
