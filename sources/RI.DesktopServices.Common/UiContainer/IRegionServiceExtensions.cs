using System;




namespace RI.DesktopServices.UiContainer
{
    /// <summary>
    ///     Provides utility/extension methods for the <see cref="IRegionService" /> type.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public static class IRegionServiceExtensions
    {
        #region Static Methods

        /// <summary>
        ///     Activates an element in a region, if the region exists.
        /// </summary>
        /// <param name="service"> The region service. </param>
        /// <param name="region"> The name of the region. </param>
        /// <param name="element"> The element which is to be activated in the region. </param>
        /// <returns> true if the region exists and the element was activated, false otherwise. </returns>
        /// <remarks>
        ///     <para>
        ///         If the element is not in the region, it will be added before being activated.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="service" />, <paramref name="region" />, or <paramref name="element" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="region" /> is an empty string. </exception>
        /// <exception cref="NotSupportedException"> The container associated to <paramref name="region" /> already has an element and does not support multiple elements. </exception>
        public static bool TryActivateElement(this IRegionService service, string region, object element)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (string.IsNullOrWhiteSpace(region))
            {
                throw new ArgumentException("Parameter is an empty string.", nameof(region));
            }

            if (!service.HasRegion(region))
            {
                return false;
            }

            service.ActivateElement(region, element);

            return true;
        }

        #endregion
    }
}
