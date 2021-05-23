namespace RI.DesktopServices.UiContainer
{
    /// <summary>
    ///     Supports region awareness of elements in a region.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="IRegionElement" /> can be implemented by elements which are used with regions to make them aware of
    ///         certain region operations being performed on them.
    ///     </para>
    /// </remarks>
    public interface IRegionElement
    {
        /// <summary>
        ///     Gets, if available, the sort index of the element in its container.
        /// </summary>
        /// <value>
        ///     The sort index of the element in its container or null if no index is provided by the element.
        /// </value>
        int? SortIndex { get; }

        /// <summary>
        ///     The element was activated.
        /// </summary>
        /// <remarks>
        ///     <note type="note">
        ///         Do not call this method directly, it is intended to be called from an <see cref="IRegionAdapter" />
        ///         implementation.
        ///     </note>
        /// </remarks>
        void Activated ();

        /// <summary>
        ///     Determines whether the element allows being navigated away.
        /// </summary>
        /// <returns>
        ///     true if the current element allows navigation away from, false otherwise.
        /// </returns>
        /// <remarks>
        ///     <note type="note">
        ///         Do not call this method directly, it is intended to be called from an <see cref="IRegionAdapter" />
        ///         implementation.
        ///     </note>
        /// </remarks>
        bool CanNavigateFrom ();

        /// <summary>
        ///     Determines whether the element allows being navigated to.
        /// </summary>
        /// <returns>
        ///     true if the current element allows navigation to, false otherwise.
        /// </returns>
        /// <remarks>
        ///     <note type="note">
        ///         Do not call this method directly, it is intended to be called from an <see cref="IRegionAdapter" />
        ///         implementation.
        ///     </note>
        /// </remarks>
        bool CanNavigateTo ();

        /// <summary>
        ///     The element was deactivated.
        /// </summary>
        /// <remarks>
        ///     <note type="note">
        ///         Do not call this method directly, it is intended to be called from an <see cref="IRegionAdapter" />
        ///         implementation.
        ///     </note>
        /// </remarks>
        void Deactivated ();

        /// <summary>
        ///     The element was navigated away from.
        /// </summary>
        /// <remarks>
        ///     <note type="note">
        ///         Do not call this method directly, it is intended to be called from an <see cref="IRegionAdapter" />
        ///         implementation.
        ///     </note>
        /// </remarks>
        void NavigatedFrom ();

        /// <summary>
        ///     The element was navigated to.
        /// </summary>
        /// <remarks>
        ///     <note type="note">
        ///         Do not call this method directly, it is intended to be called from an <see cref="IRegionAdapter" />
        ///         implementation.
        ///     </note>
        /// </remarks>
        void NavigatedTo ();
    }
}
