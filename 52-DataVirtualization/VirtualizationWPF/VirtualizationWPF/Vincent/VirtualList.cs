using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace VirtualizationWPF
{
    public class VirtualList<T> : VirtualListBase<T>, ICollectionViewFactory where T : class
    {
        internal readonly Func<SortDescriptionCollection, Predicate<object>, T[], int, int> Load;

        public VirtualList(Func<SortDescriptionCollection, Predicate<object>, T[], int, int> load, int numCacheBlocks, int cacheBlockLength)
            : base(numCacheBlocks, cacheBlockLength)
        {
            Load = load;
        }

        public VirtualList(Func<SortDescriptionCollection, Predicate<object>, T[], int, int> load)
            : base(5, 200)
        {
            Load = load;
        }

        protected override int InternalLoad(T[] data, int startIndex)
        {
            return Load(null, null, data, startIndex);
        }

        #region ICollectionViewFactory Members

        public ICollectionView CreateView()
        {
            return new VirtualListCollectionView<T>(this);
        }

        #endregion
    }
}
