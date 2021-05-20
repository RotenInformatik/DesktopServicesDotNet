using System;
using System.Collections.Generic;

using RI.DesktopServices.UiContainer;




namespace RI.Framework.Services.Regions
{
    /// <summary>
    ///     Default implementation of <see cref="IRegionService" /> suitable for most scenarios.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public sealed class RegionService : IRegionService
    {
        #region Constants

        /// <summary>
        ///     Gets the used string comparer used to compare region names for equality.
        /// </summary>
        /// <value>
        ///     The used string comparer used to compare region names for equality.
        /// </value>
        public static readonly StringComparerEx RegionNameComparer = StringComparerEx.InvariantCultureIgnoreCase;

        #endregion




        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="RegionService" />.
        /// </summary>
        public RegionService ()
        {
            this.AdaptersUpdated = new List<IRegionAdapter>();
            this.AdaptersManual = new List<IRegionAdapter>();

            this.RegionDictionary = new Dictionary<string, Tuple<object, IRegionAdapter>>(RegionService.RegionNameComparer);
        }

        #endregion




        #region Instance Properties/Indexer

        [Import(typeof(IRegionAdapter), Recomposable = true)]
        private Import AdaptersImported { get; set; }

        private List<IRegionAdapter> AdaptersManual { get; set; }

        private List<IRegionAdapter> AdaptersUpdated { get; set; }

        private Dictionary<string, Tuple<object, IRegionAdapter>> RegionDictionary { get; set; }

        #endregion




        #region Instance Methods

        private void UpdateAdapters ()
        {
            this.Log(LogLevel.Debug, "Updating adapters");

            HashSet<IRegionAdapter> currentAdapters = new HashSet<IRegionAdapter>(this.Adapters);
            HashSet<IRegionAdapter> lastAdapters = new HashSet<IRegionAdapter>(this.AdaptersUpdated);

            HashSet<IRegionAdapter> newAdapters = currentAdapters.Except(lastAdapters);
            HashSet<IRegionAdapter> oldAdapters = lastAdapters.Except(currentAdapters);

            this.AdaptersUpdated.Clear();
            this.AdaptersUpdated.AddRange(currentAdapters);

            foreach (IRegionAdapter adapter in newAdapters)
            {
                this.Log(LogLevel.Debug, "Receiver added: {0}", adapter.GetType().Name);
            }

            foreach (IRegionAdapter adapter in oldAdapters)
            {
                this.Log(LogLevel.Debug, "Receiver removed: {0}", adapter.GetType().Name);
            }
        }

        #endregion




        #region Interface: IImporting

        /// <inheritdoc />
        void IImporting.ImportsResolved (CompositionFlags composition, bool updated)
        {
            if (updated)
            {
                this.UpdateAdapters();
            }
        }

        /// <inheritdoc />
        void IImporting.ImportsResolving (CompositionFlags composition)
        {
        }

        #endregion




        #region Interface: IRegionService

        /// <inheritdoc />
        public IEnumerable<IRegionAdapter> Adapters
        {
            get
            {
                foreach (IRegionAdapter adapter in this.AdaptersManual)
                {
                    yield return adapter;
                }

                foreach (IRegionAdapter adapter in this.AdaptersImported.Values<IRegionAdapter>())
                {
                    yield return adapter;
                }
            }
        }

        /// <inheritdoc />
        public void ActivateAllElements (string region)
        {
            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            if (region.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(region));
            }

            if (!this.RegionDictionary.ContainsKey(region))
            {
                throw new RegionNotFoundException(region);
            }

            object container = this.RegionDictionary[region].Item1;
            IRegionAdapter adapter = this.RegionDictionary[region].Item2;
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

            if (region.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(region));
            }

            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (!this.RegionDictionary.ContainsKey(region))
            {
                throw new RegionNotFoundException(region);
            }

            this.AddElement(region, element);

            object container = this.RegionDictionary[region].Item1;
            IRegionAdapter adapter = this.RegionDictionary[region].Item2;

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

            if (this.AdaptersManual.Contains(regionAdapter))
            {
                return;
            }

            this.AdaptersManual.Add(regionAdapter);

            this.UpdateAdapters();
        }

        /// <inheritdoc />
        public void AddElement (string region, object element)
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

            if (!this.RegionDictionary.ContainsKey(region))
            {
                throw new RegionNotFoundException(region);
            }

            if (this.HasElement(region, element))
            {
                return;
            }

            object container = this.RegionDictionary[region].Item1;
            IRegionAdapter adapter = this.RegionDictionary[region].Item2;

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

            if (region.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(region));
            }

            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            List<Tuple<int, IRegionAdapter>> adapters = new List<Tuple<int, IRegionAdapter>>();
            Type containerType = container.GetType();
            foreach (IRegionAdapter currentAdapter in this.Adapters)
            {
                int inheritanceDepth;
                if (currentAdapter.IsCompatibleContainer(containerType, out inheritanceDepth))
                {
                    adapters.Add(new Tuple<int, IRegionAdapter>(inheritanceDepth, currentAdapter));
                }
            }
            adapters.Sort((x, y) => x.Item1.CompareTo(y.Item1));
            if (adapters.Count == 0)
            {
                throw new InvalidTypeArgumentException(nameof(container));
            }
            IRegionAdapter adapter = adapters[0].Item2;

            if (this.RegionDictionary.ContainsKey(region))
            {
                if (object.ReferenceEquals(container, this.RegionDictionary[region].Item1) && adapter.Equals(this.RegionDictionary[region].Item2))
                {
                    return;
                }
                this.RegionDictionary.Remove(region);
            }

            this.Log(LogLevel.Debug, "Region added: {0} -> {1} @ {2}", region, container.GetType().Name, adapter.GetType().Name);

            this.RegionDictionary.Add(region, new Tuple<object, IRegionAdapter>(container, adapter));
        }

        /// <inheritdoc />
        public bool CanNavigate (string region, object element)
        {
            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            if (region.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(region));
            }

            if (!this.RegionDictionary.ContainsKey(region))
            {
                throw new RegionNotFoundException(region);
            }

            object container = this.RegionDictionary[region].Item1;
            IRegionAdapter adapter = this.RegionDictionary[region].Item2;

            return adapter.CanNavigate(container, element);
        }

        /// <inheritdoc />
        public void ClearElements (string region)
        {
            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            if (region.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(region));
            }

            if (!this.RegionDictionary.ContainsKey(region))
            {
                throw new RegionNotFoundException(region);
            }

            object container = this.RegionDictionary[region].Item1;
            IRegionAdapter adapter = this.RegionDictionary[region].Item2;

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

            if (region.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(region));
            }

            if (!this.RegionDictionary.ContainsKey(region))
            {
                throw new RegionNotFoundException(region);
            }

            object container = this.RegionDictionary[region].Item1;
            IRegionAdapter adapter = this.RegionDictionary[region].Item2;
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

            if (region.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(region));
            }

            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (!this.RegionDictionary.ContainsKey(region))
            {
                throw new RegionNotFoundException(region);
            }

            this.AddElement(region, element);

            object container = this.RegionDictionary[region].Item1;
            IRegionAdapter adapter = this.RegionDictionary[region].Item2;

            adapter.Deactivate(container, element);
            adapter.Sort(container);
        }

        /// <inheritdoc />
        public List<object> GetElements (string region)
        {
            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            if (region.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(region));
            }

            if (!this.RegionDictionary.ContainsKey(region))
            {
                throw new RegionNotFoundException(region);
            }

            object container = this.RegionDictionary[region].Item1;
            IRegionAdapter adapter = this.RegionDictionary[region].Item2;

            return adapter.Get(container);
        }

        /// <inheritdoc />
        public object GetRegionContainer (string region)
        {
            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            if (region.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(region));
            }

            if (!this.RegionDictionary.ContainsKey(region))
            {
                return null;
            }

            return this.RegionDictionary[region].Item1;
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
                if (object.ReferenceEquals(region.Value.Item1, container))
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
                if (object.ReferenceEquals(region.Value.Item1, container))
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

            if (region.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(region));
            }

            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (!this.RegionDictionary.ContainsKey(region))
            {
                throw new RegionNotFoundException(region);
            }

            object container = this.RegionDictionary[region].Item1;
            IRegionAdapter adapter = this.RegionDictionary[region].Item2;

            return adapter.Contains(container, element);
        }

        /// <inheritdoc />
        public bool HasRegion (string region)
        {
            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            if (region.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(region));
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

            if (region.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(region));
            }

            if (!this.RegionDictionary.ContainsKey(region))
            {
                throw new RegionNotFoundException(region);
            }

            object container = this.RegionDictionary[region].Item1;
            IRegionAdapter adapter = this.RegionDictionary[region].Item2;

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

            foreach (KeyValuePair<string, Tuple<object, IRegionAdapter>> region in this.RegionDictionary)
            {
                if (regionAdapter.Equals(region.Value.Item2))
                {
                    throw new InvalidOperationException("The specified region adapter is still in use.");
                }
            }

            if (!this.AdaptersManual.Contains(regionAdapter))
            {
                return;
            }

            this.AdaptersManual.RemoveAll(regionAdapter);

            this.UpdateAdapters();
        }

        /// <inheritdoc />
        public void RemoveElement (string region, object element)
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

            if (!this.RegionDictionary.ContainsKey(region))
            {
                throw new RegionNotFoundException(region);
            }

            if (!this.HasElement(region, element))
            {
                return;
            }

            object container = this.RegionDictionary[region].Item1;
            IRegionAdapter adapter = this.RegionDictionary[region].Item2;

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

            if (region.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(region));
            }

            if (!this.RegionDictionary.ContainsKey(region))
            {
                return;
            }

            this.Log(LogLevel.Debug, "Region removed: {0}", region);

            this.RegionDictionary.Remove(region);
        }

        #endregion
    }
}
