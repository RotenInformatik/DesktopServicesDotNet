using System;
using System.Runtime.Serialization;




namespace RI.DesktopServices.UiContainer
{
    /// <summary>
    ///     The <see cref="RegionNotFoundException" /> is thrown when a region cannot be found and operations are not possible without a region.
    /// </summary>
    [Serializable,]
    public class RegionNotFoundException : Exception
    {
        #region Constants

        private const string ExceptionMessage = "The region does not exist: {0}.";

        #endregion




        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="RegionNotFoundException" />.
        /// </summary>
        /// <param name="region"> The name of the region which could not be found. </param>
        public RegionNotFoundException (string region)
            : base(string.Format(RegionNotFoundException.ExceptionMessage, region))
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="RegionNotFoundException" />.
        /// </summary>
        /// <param name="message"> The message which describes the exception. </param>
        /// <param name="innerException"> The exception which triggered this exception. </param>
        public RegionNotFoundException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        ///     Creates a new instance of <see cref="RegionNotFoundException" />.
        /// </summary>
        /// <param name="info"> The serialization data. </param>
        /// <param name="context"> The type of the source of the serialization data. </param>
        protected RegionNotFoundException (SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}
