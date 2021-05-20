using System;
using System.Collections.Generic;

using RI.DesktopServices.UiContainer;
using RI.Framework.ComponentModel;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Services.Regions
{
    /// <summary>
    ///     Provides a centralized and global region provider.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="RegionLocator" /> is merely a convenience utility as it uses <see cref="ServiceLocator" /> to retrieve and use a <see cref="IRegionService" />.
    ///     </para>
    ///     <para>
    ///         <see cref="RegionLocator" /> has additional overloads to supply an elements or containers name or type, which are resolved using <see cref="ServiceLocator" />.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    /// TODO: Do not use locator types, move things into extension methods
    public static class RegionLocator
    {
        #region Static Properties/Indexer

        /// <inheritdoc cref="IRegionService.Adapters" />
        public static IEnumerable<IRegionAdapter> Adapters => RegionLocator.Service?.Adapters ?? new IRegionAdapter[0];

        /// <summary>
        ///     Gets whether a region service is available and can be used by <see cref="RegionLocator" />.
        /// </summary>
        /// <value>
        ///     true if a region service is available and can be used by <see cref="RegionLocator" />, false otherwise.
        /// </value>
        public static bool IsAvailable => RegionLocator.Service != null;

        /// <summary>
        ///     Gets the available region service.
        /// </summary>
        /// <value>
        ///     The available region service or null if no region service is available.
        /// </value>
        public static IRegionService Service => ServiceLocator.GetInstance<IRegionService>();

        #endregion




        #region Static Methods

        /// <inheritdoc cref="IRegionService.ActivateElement" />
        public static void ActivateElement (string region, string element)
        {
            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            if (region.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(region));
            }

            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (element.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(element));
            }

            object value = RegionLocator.GetValue(element);
            if (value == null)
            {
                throw new InvalidOperationException("An element of name \"" + element + "\" could not be found.");
            }

            RegionLocator.ActivateElement(region, value);
        }

        /// <inheritdoc cref="IRegionService.ActivateElement" />
        public static void ActivateElement (string region, Type element)
        {
            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            if (region.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(region));
            }

            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            object value = RegionLocator.GetValue(element);
            if (value == null)
            {
                throw new InvalidOperationException("An element of type " + element.Name + " could not be found.");
            }

            RegionLocator.ActivateElement(region, value);
        }

        /// <inheritdoc cref="IRegionService.ActivateElement" />
        public static void ActivateElement (string region, object element) => RegionLocator.Service?.ActivateElement(region, element);

        /// <inheritdoc cref="IRegionService.AddElement" />
        public static void AddElement (string region, string element)
        {
            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            if (region.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(region));
            }

            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (element.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(element));
            }

            object value = RegionLocator.GetValue(element);
            if (value == null)
            {
                throw new InvalidOperationException("An element of name \"" + element + "\" could not be found.");
            }

            RegionLocator.AddElement(region, value);
        }

        /// <inheritdoc cref="IRegionService.AddElement" />
        public static void AddElement (string region, Type element)
        {
            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            if (region.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(region));
            }

            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            object value = RegionLocator.GetValue(element);
            if (value == null)
            {
                throw new InvalidOperationException("An element of type " + element.Name + " could not be found.");
            }

            RegionLocator.AddElement(region, value);
        }

        /// <inheritdoc cref="IRegionService.AddElement" />
        public static void AddElement (string region, object element) => RegionLocator.Service?.AddElement(region, element);

        /// <inheritdoc cref="IRegionService.AddRegion" />
        public static void AddRegion (string region, string container)
        {
            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            if (region.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(region));
            }

            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (container.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(container));
            }

            object value = RegionLocator.GetValue(container);
            if (value == null)
            {
                throw new InvalidOperationException("A container of name \"" + container + "\" could not be found.");
            }

            RegionLocator.AddRegion(region, value);
        }

        /// <inheritdoc cref="IRegionService.AddRegion" />
        public static void AddRegion (string region, Type container)
        {
            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            if (region.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(region));
            }

            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            object value = RegionLocator.GetValue(container);
            if (value == null)
            {
                throw new InvalidOperationException("A container of type " + container.Name + " could not be found.");
            }

            RegionLocator.AddRegion(region, value);
        }

        /// <inheritdoc cref="IRegionService.AddRegion" />
        public static void AddRegion (string region, object container) => RegionLocator.Service?.AddRegion(region, container);

        /// <inheritdoc cref="IRegionService.CanNavigate" />
        public static bool CanNavigate (string region, string element)
        {
            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            if (region.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(region));
            }

            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (element.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(element));
            }

            object value = RegionLocator.GetValue(element);
            if (value == null)
            {
                throw new InvalidOperationException("An element of name \"" + element + "\" could not be found.");
            }

            return RegionLocator.CanNavigate(region, value);
        }

        /// <inheritdoc cref="IRegionService.CanNavigate" />
        public static bool CanNavigate (string region, Type element)
        {
            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            if (region.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(region));
            }

            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            object value = RegionLocator.GetValue(element);
            if (value == null)
            {
                throw new InvalidOperationException("An element of type " + element.Name + " could not be found.");
            }

            return RegionLocator.CanNavigate(region, value);
        }

        /// <inheritdoc cref="IRegionService.CanNavigate" />
        public static bool CanNavigate (string region, object element) => RegionLocator.Service?.CanNavigate(region, element) ?? false;

        /// <inheritdoc cref="IRegionService.ClearElements" />
        public static void ClearElements (string region) => RegionLocator.Service?.ClearElements(region);

        /// <inheritdoc cref="IRegionService.DeactivateAllElements" />
        public static void DeactivateAllElements (string region) => RegionLocator.Service?.DeactivateAllElements(region);

        /// <inheritdoc cref="IRegionService.DeactivateElement" />
        public static void DeactivateElement (string region, string element)
        {
            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            if (region.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(region));
            }

            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (element.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(element));
            }

            object value = RegionLocator.GetValue(element);
            if (value == null)
            {
                throw new InvalidOperationException("An element of name \"" + element + "\" could not be found.");
            }

            RegionLocator.DeactivateElement(region, value);
        }

        /// <inheritdoc cref="IRegionService.DeactivateElement" />
        public static void DeactivateElement (string region, Type element)
        {
            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            if (region.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(region));
            }

            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            object value = RegionLocator.GetValue(element);
            if (value == null)
            {
                throw new InvalidOperationException("An element of type " + element.Name + " could not be found.");
            }

            RegionLocator.DeactivateElement(region, value);
        }

        /// <inheritdoc cref="IRegionService.DeactivateElement" />
        public static void DeactivateElement (string region, object element) => RegionLocator.Service?.DeactivateElement(region, element);

        /// <inheritdoc cref="IRegionService.GetElements" />
        public static List<object> GetElements (string region) => RegionLocator.Service?.GetElements(region) ?? new List<object>();

        /// <inheritdoc cref="IRegionService.GetRegionContainer" />
        public static object GetRegionContainer (string region) => RegionLocator.Service?.GetRegionContainer(region);

        /// <inheritdoc cref="IRegionService.GetRegionName" />
        public static string GetRegionName (string container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (container.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(container));
            }

            object value = RegionLocator.GetValue(container);
            if (value == null)
            {
                throw new InvalidOperationException("A container of name \"" + container + "\" could not be found.");
            }

            return RegionLocator.GetRegionName(value);
        }

        /// <inheritdoc cref="IRegionService.GetRegionName" />
        public static string GetRegionName (Type container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            object value = RegionLocator.GetValue(container);
            if (value == null)
            {
                throw new InvalidOperationException("A container of type " + container.Name + " could not be found.");
            }

            return RegionLocator.GetRegionName(value);
        }

        /// <inheritdoc cref="IRegionService.GetRegionName" />
        public static string GetRegionName (object container) => RegionLocator.Service?.GetRegionName(container);

        /// <inheritdoc cref="IRegionService.GetRegionNames(object)" />
        public static HashSet<string> GetRegionNames (string container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (container.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(container));
            }

            object value = RegionLocator.GetValue(container);
            if (value == null)
            {
                throw new InvalidOperationException("A container of name \"" + container + "\" could not be found.");
            }

            return RegionLocator.GetRegionNames(value);
        }

        /// <inheritdoc cref="IRegionService.GetRegionNames(object)" />
        public static HashSet<string> GetRegionNames (Type container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            object value = RegionLocator.GetValue(container);
            if (value == null)
            {
                throw new InvalidOperationException("A container of type " + container.Name + " could not be found.");
            }

            return RegionLocator.GetRegionNames(value);
        }

        /// <inheritdoc cref="IRegionService.GetRegionNames(object)" />
        public static HashSet<string> GetRegionNames (object container) => RegionLocator.Service?.GetRegionNames(container);

        /// <inheritdoc cref="IRegionService.GetRegionNames()" />
        public static HashSet<string> GetRegionNames () => RegionLocator.Service?.GetRegionNames() ?? new HashSet<string>();

        /// <inheritdoc cref="IRegionService.HasElement" />
        public static bool HasElement (string region, string element)
        {
            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            if (region.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(region));
            }

            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (element.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(element));
            }

            object value = RegionLocator.GetValue(element);
            if (value == null)
            {
                throw new InvalidOperationException("An element of name \"" + element + "\" could not be found.");
            }

            return RegionLocator.HasElement(region, value);
        }

        /// <inheritdoc cref="IRegionService.HasElement" />
        public static bool HasElement (string region, Type element)
        {
            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            if (region.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(region));
            }

            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            object value = RegionLocator.GetValue(element);
            if (value == null)
            {
                throw new InvalidOperationException("An element of type " + element.Name + " could not be found.");
            }

            return RegionLocator.HasElement(region, value);
        }

        /// <inheritdoc cref="IRegionService.HasElement" />
        public static bool HasElement (string region, object element) => RegionLocator.Service?.HasElement(region, element) ?? false;

        /// <inheritdoc cref="IRegionService.HasRegion" />
        public static bool HasRegion (string region) => RegionLocator.Service?.HasRegion(region) ?? false;

        /// <inheritdoc cref="IRegionService.Navigate" />
        public static bool Navigate (string region, string element)
        {
            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            if (region.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(region));
            }

            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (element.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(element));
            }

            object value = RegionLocator.GetValue(element);
            if (value == null)
            {
                throw new InvalidOperationException("An element of name \"" + element + "\" could not be found.");
            }

            return RegionLocator.Navigate(region, value);
        }

        /// <inheritdoc cref="IRegionService.Navigate" />
        public static bool Navigate (string region, Type element)
        {
            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            if (region.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(region));
            }

            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            object value = RegionLocator.GetValue(element);
            if (value == null)
            {
                throw new InvalidOperationException("An element of type " + element.Name + " could not be found.");
            }

            return RegionLocator.Navigate(region, value);
        }

        /// <inheritdoc cref="IRegionService.Navigate" />
        public static bool Navigate (string region, object element) => RegionLocator.Service?.Navigate(region, element) ?? false;

        /// <inheritdoc cref="IRegionService.RemoveElement" />
        public static void RemoveElement (string region, string element)
        {
            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            if (region.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(region));
            }

            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (element.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(element));
            }

            object value = RegionLocator.GetValue(element);
            if (value == null)
            {
                throw new InvalidOperationException("An element of name \"" + element + "\" could not be found.");
            }

            RegionLocator.RemoveElement(region, value);
        }

        /// <inheritdoc cref="IRegionService.RemoveElement" />
        public static void RemoveElement (string region, Type element)
        {
            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            if (region.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(region));
            }

            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            object value = RegionLocator.GetValue(element);
            if (value == null)
            {
                throw new InvalidOperationException("An element of type " + element.Name + " could not be found.");
            }

            RegionLocator.RemoveElement(region, value);
        }

        /// <inheritdoc cref="IRegionService.RemoveElement" />
        public static void RemoveElement (string region, object element) => RegionLocator.Service?.RemoveElement(region, element);

        /// <inheritdoc cref="IRegionService.RemoveRegion" />
        public static void RemoveRegion (string region) => RegionLocator.Service?.RemoveRegion(region);

        private static object GetValue (string name)
        {
            if (name.IsNullOrEmptyOrWhitespace())
            {
                return null;
            }

            return ServiceLocator.GetInstance(name);
        }

        private static object GetValue (Type type)
        {
            if (type == null)
            {
                return null;
            }

            return ServiceLocator.GetInstance(type);
        }

        #endregion
    }
}
