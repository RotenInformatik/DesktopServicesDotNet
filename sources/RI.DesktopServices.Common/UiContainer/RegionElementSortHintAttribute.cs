using System;




namespace RI.DesktopServices.UiContainer
{
    /// <summary>
    ///     Used as a hint for region adapters to assign a sort index to an element.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class RegionElementSortHintAttribute : Attribute
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="RegionService" />.
        /// </summary>
        /// <param name="index"> The sort index to be assigned to the element. </param>
        public RegionElementSortHintAttribute (int index)
        {
            this.Index = index;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the sort index of the element.
        /// </summary>
        /// <value>
        ///     The sort index of the element.
        /// </value>
        public int Index { get; }

        #endregion
    }
}
