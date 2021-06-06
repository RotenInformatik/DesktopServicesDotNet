using System;
using System.Collections.Generic;
using System.Diagnostics;




namespace RI.DesktopServices.UiContainer
{
    /// <summary>
    ///     Default implementation of <see cref="IRegionService" /> suitable for most scenarios.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public sealed class RegionService : IRegionService
    {
        #region Static Fields

        /// <summary>
        ///     Gets the used string comparer used to compare region names for equality.
        /// </summary>
        /// <value>
        ///     The used string comparer used to compare region names for equality.
        /// </value>
        public static readonly StringComparer RegionNameComparer = StringComparer.InvariantCultureIgnoreCase;

        #endregion




        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="RegionService" />.
        /// </summary>
        public RegionService ()
        {
            this.Adapters = new List<IRegionAdapter>();

            this.RegionDictionary =
                new Dictionary<string, Tuple<object, IRegionAdapter>>(RegionService.RegionNameComparer);
        }

        #endregion




        #region Instance Properties/Indexer

        private List<IRegionAdapter> Adapters { get; }

        private Dictionary<string, Tuple<object, IRegionAdapter>> RegionDictionary { get; }

        #endregion




        #region Interface: IRegionService

        /// <inheritdoc />
        public void ActivateAllElements (string region)
        {
            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            if (string.IsNullOrWhiteSpace(region))
            {
                throw new ArgumentException("Parameter is an empty string.", nameof(region));
            }

            if (!this.RegionDictionary.ContainsKey(region))
            {
                throw new RegionNotFoundException(region);
            }

            object container = this.RegionDictionary[region]
                                   .Item1;

            IRegionAdapter adapter = this.RegionDictionary[region]
                                         .Item2;

            List<object> elements = adapter.Get(container);

            foreach (object element in elements)
            {
                adapter.Activate(container, element);
            }

            adapter.Sort(container);
        }

        /// <inheritdoc />
        public void ActivateElement (string region, object element)
        {
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

            if (!this.RegionDictionary.ContainsKey(region))
            {
                throw new RegionNotFoundException(region);
            }

            this.AddElement(region, element);

            object container = this.RegionDictionary[region]
                                   .Item1;

            IRegionAdapter adapter = this.RegionDictionary[region]
                                         .Item2;

            adapter.Activate(container, element);
            adapter.Sort(container);
        }

        /// <inheritdoc />
        public void AddAdapter (IRegionAdapter regionAdapter)
        {
            if (regionAdapter == null)
            {
                throw new ArgumentNullException(nameof(regionAdapter));
            }

            if (this.Adapters.Contains(regionAdapter))
            {
                return;
            }

            Trace.TraceInformation($"Adding region adapter: {regionAdapter.GetType().Name}");

            this.Adapters.Add(regionAdapter);
        }

        /// <inheritdoc />
        public void AddElement (string region, object element)
        {
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

            if (!this.RegionDictionary.ContainsKey(region))
            {
                throw new RegionNotFoundException(region);
            }

            if (this.HasElement(region, element))
            {
                return;
            }

            object container = this.RegionDictionary[region]
                                   .Item1;

            IRegionAdapter adapter = this.RegionDictionary[region]
                                         .Item2;

            adapter.Add(container, element);
            adapter.Sort(container);
        }

        /// <inheritdoc />
        public void AddRegion (string region, object container)
        {
            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (string.IsNullOrWhiteSpace(region))
            {
                throw new ArgumentException("Parameter is an empty string.", nameof(region));
            }

            Trace.TraceInformation($"Adding region {region} to: {container.GetType().Name}");

            List<Tuple<int, IRegionAdapter>> adapters = new List<Tuple<int, IRegionAdapter>>();
            Type containerType = container.GetType();

            foreach (IRegionAdapter currentAdapter in this.Adapters)
            {
                if (currentAdapter.IsCompatibleContainer(containerType, out int inheritanceDepth))
                {
                    adapters.Add(new Tuple<int, IRegionAdapter>(inheritanceDepth, currentAdapter));
                }
            }

            adapters.Sort((x, y) => x.Item1.CompareTo(y.Item1));

            if (adapters.Count == 0)
            {
                throw new NotSupportedException($"No region adapter supports the container type {containerType.Name}");
            }

            IRegionAdapter adapter = adapters[0]
                .Item2;

            Trace.TraceInformation($"Used region adapter for region{region} at {container.GetType().Name}: {adapter.GetType().Name}");

            if (this.RegionDictionary.ContainsKey(region))
            {
                if (ReferenceEquals(container, this.RegionDictionary[region]
                                                   .Item1) && adapter.Equals(this.RegionDictionary[region]
                                                                                 .Item2))
                {
                    return;
                }

                this.RegionDictionary.Remove(region);
            }

            this.RegionDictionary.Add(region, new Tuple<object, IRegionAdapter>(container, adapter));
        }

        /// <inheritdoc />
        public bool CanNavigate (string region, object element)
        {
            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            if (string.IsNullOrWhiteSpace(region))
            {
                throw new ArgumentException("Parameter is an empty string.", nameof(region));
            }

            if (!this.RegionDictionary.ContainsKey(region))
            {
                throw new RegionNotFoundException(region);
            }

            object container = this.RegionDictionary[region]
                                   .Item1;

            IRegionAdapter adapter = this.RegionDictionary[region]
                                         .Item2;

            return adapter.CanNavigate(container, element);
        }

        /// <inheritdoc />
        public void ClearElements (string region)
        {
            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            if (string.IsNullOrWhiteSpace(region))
            {
                throw new ArgumentException("Parameter is an empty string.", nameof(region));
            }

            if (!this.RegionDictionary.ContainsKey(region))
            {
                throw new RegionNotFoundException(region);
            }

            object container = this.RegionDictionary[region]
                                   .Item1;

            IRegionAdapter adapter = this.RegionDictionary[region]
                                         .Item2;

            adapter.Clear(container);
            adapter.Sort(container);
        }

        /// <inheritdoc />
        public void DeactivateAllElements (string region)
        {
            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            if (string.IsNullOrWhiteSpace(region))
            {
                throw new ArgumentException("Parameter is an empty string.", nameof(region));
            }

            if (!this.RegionDictionary.ContainsKey(region))
            {
                throw new RegionNotFoundException(region);
            }

            object container = this.RegionDictionary[region]
                                   .Item1;

            IRegionAdapter adapter = this.RegionDictionary[region]
                                         .Item2;

            List<object> elements = adapter.Get(container);

            foreach (object element in elements)
            {
                adapter.Deactivate(container, element);
            }

            adapter.Sort(container);
        }

        /// <inheritdoc />
        public void DeactivateElement (string region, object element)
        {
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

            if (!this.RegionDictionary.ContainsKey(region))
            {
                throw new RegionNotFoundException(region);
            }

            this.AddElement(region, element);

            object container = this.RegionDictionary[region]
                                   .Item1;

            IRegionAdapter adapter = this.RegionDictionary[region]
                                         .Item2;

            adapter.Deactivate(container, element);
            adapter.Sort(container);
        }

        /// <inheritdoc />
        public List<IRegionAdapter> GetAdapters ()
        {
            return new List<IRegionAdapter>(this.Adapters);
        }

        /// <inheritdoc />
        public object GetContainerOfRegion (string region)
        {
            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            if (string.IsNullOrWhiteSpace(region))
            {
                throw new ArgumentException("Parameter is an empty string.", nameof(region));
            }

            if (!this.RegionDictionary.ContainsKey(region))
            {
                return null;
            }

            return this.RegionDictionary[region]
                       .Item1;
        }

        /// <inheritdoc />
        public List<object> GetElementsOfRegion (string region)
        {
            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            if (string.IsNullOrWhiteSpace(region))
            {
                throw new ArgumentException("Parameter is an empty string.", nameof(region));
            }

            if (!this.RegionDictionary.ContainsKey(region))
            {
                throw new RegionNotFoundException(region);
            }

            object container = this.RegionDictionary[region]
                                   .Item1;

            IRegionAdapter adapter = this.RegionDictionary[region]
                                         .Item2;

            return adapter.Get(container);
        }

        /// <inheritdoc />
        public string GetRegionName (object container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            foreach (KeyValuePair<string, Tuple<object, IRegionAdapter>> region in this.RegionDictionary)
            {
                if (ReferenceEquals(region.Value.Item1, container))
                {
                    return region.Key;
                }
            }

            return null;
        }

        /// <inheritdoc />
        public HashSet<string> GetRegionNames (object container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            HashSet<string> names = new HashSet<string>(this.RegionDictionary.Comparer);

            foreach (KeyValuePair<string, Tuple<object, IRegionAdapter>> region in this.RegionDictionary)
            {
                if (ReferenceEquals(region.Value.Item1, container))
                {
                    names.Add(region.Key);
                }
            }

            return names;
        }

        /// <inheritdoc />
        public HashSet<string> GetRegionNames ()
        {
            return new HashSet<string>(this.RegionDictionary.Keys, this.RegionDictionary.Comparer);
        }

        /// <inheritdoc />
        public bool HasElement (string region, object element)
        {
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

            if (!this.RegionDictionary.ContainsKey(region))
            {
                throw new RegionNotFoundException(region);
            }

            object container = this.RegionDictionary[region]
                                   .Item1;

            IRegionAdapter adapter = this.RegionDictionary[region]
                                         .Item2;

            return adapter.Contains(container, element);
        }

        /// <inheritdoc />
        public bool HasRegion (string region)
        {
            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            if (string.IsNullOrWhiteSpace(region))
            {
                throw new ArgumentException("Parameter is an empty string.", nameof(region));
            }

            return this.RegionDictionary.ContainsKey(region);
        }

        /// <inheritdoc />
        public bool Navigate (string region, object element)
        {
            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            if (string.IsNullOrWhiteSpace(region))
            {
                throw new ArgumentException("Parameter is an empty string.", nameof(region));
            }

            if (!this.RegionDictionary.ContainsKey(region))
            {
                throw new RegionNotFoundException(region);
            }

            object container = this.RegionDictionary[region]
                                   .Item1;

            IRegionAdapter adapter = this.RegionDictionary[region]
                                         .Item2;

            bool result = adapter.Navigate(container, element);
            adapter.Sort(container);

            return result;
        }

        /// <inheritdoc />
        public void RemoveAdapter (IRegionAdapter regionAdapter)
        {
            if (regionAdapter == null)
            {
                throw new ArgumentNullException(nameof(regionAdapter));
            }

            Trace.TraceInformation($"Removing region adapter: {regionAdapter.GetType().Name}");

            foreach (KeyValuePair<string, Tuple<object, IRegionAdapter>> region in this.RegionDictionary)
            {
                if (ReferenceEquals(regionAdapter, region.Value.Item2))
                {
                    throw new InvalidOperationException("The specified region adapter is still in use.");
                }
            }

            this.Adapters.Remove(regionAdapter);
        }

        /// <inheritdoc />
        public void RemoveElement (string region, object element)
        {
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

            if (!this.RegionDictionary.ContainsKey(region))
            {
                throw new RegionNotFoundException(region);
            }

            if (!this.HasElement(region, element))
            {
                return;
            }

            object container = this.RegionDictionary[region]
                                   .Item1;

            IRegionAdapter adapter = this.RegionDictionary[region]
                                         .Item2;

            adapter.Remove(container, element);
            adapter.Sort(container);
        }

        /// <inheritdoc />
        public void RemoveRegion (string region)
        {
            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            if (string.IsNullOrWhiteSpace(region))
            {
                throw new ArgumentException("Parameter is an empty string.", nameof(region));
            }

            Trace.TraceInformation($"Removing region: {region}");

            this.RegionDictionary.Remove(region);
        }

        #endregion
    }
}
