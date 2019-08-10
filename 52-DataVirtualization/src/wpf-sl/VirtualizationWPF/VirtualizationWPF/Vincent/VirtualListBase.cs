using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Threading;

namespace VirtualizationWPF
{
    public abstract class VirtualListBase<T> : IList<DataRefBase<T>>, IList, IItemProperties, INotifyPropertyChanged, INotifyCollectionChanged where T : class
    {
        private static readonly PropertyChangedEventArgs m_IndexerChanged = new PropertyChangedEventArgs("Item[]");
        private static readonly PropertyChangedEventArgs m_CountChanged = new PropertyChangedEventArgs("Count");
        private static readonly NotifyCollectionChangedEventArgs m_CollectionReset = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
        private const int UndefinedCount = -1;
        private const int UninitializedCount = -1;
        private int m_Count;

        // for IList //
        private object m_SyncRoot;

        private static readonly ReadOnlyCollection<ItemPropertyInfo> m_ItemPropertyInfo;

        // statistics
        private int m_CacheRequest;
        private int m_CacheMisses;

        private class CachedDataRef : DataRefBase<T>
        {
            public readonly VirtualListBase<T> List;
            public readonly int Index;

            public CachedDataRef(int index, VirtualListBase<T> list)
            {
                Index = index;
                List = list;
            }

            public override T Data
            {
                get { return List.LoadData(Index); }
            }

            public override int GetHashCode()
            {
                return Index ^ List.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                CachedDataRef other = obj as CachedDataRef;
                return other != null && Index == other.Index && List == other.List;
            }
        }

        private class CacheBlock
        {
            public int StartIndex;
            public int EndIndex;
            public readonly T[] Data;

            public CacheBlock(int length)
            {
                Data = new T[length];
            }

            public bool Contains(int recordNo, out int index)
            {
                if (StartIndex <= recordNo && recordNo < EndIndex)
                {
                    index = recordNo - StartIndex;
                    return true;
                }
                else
                {
                    index = -1;
                    return false;
                }
            }

            public void Clear()
            {
                StartIndex = EndIndex = 0;
                for (int i = 0; i < Data.Length; ++i)
                    Data[i] = null;
            }
        }

        private readonly LinkedList<CacheBlock> m_CacheBlock;
        internal readonly int NumCacheBlocks;
        internal readonly int NumItemsPerCacheBlock;

        public VirtualListBase(int numCacheBlocks, int cacheBlockLength)
        {
            m_Count = UninitializedCount;
            NumCacheBlocks = numCacheBlocks;
            NumItemsPerCacheBlock = cacheBlockLength;
            m_CacheBlock = new LinkedList<CacheBlock>();
        }

        static VirtualListBase()
        {
            IList<ItemPropertyInfo> itemPropertyInfo = new List<ItemPropertyInfo>(CachedDataRef.PropertyDescriptorCollection.Count);
            foreach (PropertyDescriptor propertyDescriptor in CachedDataRef.PropertyDescriptorCollection)
                itemPropertyInfo.Add(new ItemPropertyInfo(propertyDescriptor.Name, propertyDescriptor.PropertyType, propertyDescriptor));
            m_ItemPropertyInfo = new ReadOnlyCollection<ItemPropertyInfo>(itemPropertyInfo);
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
                CollectionChanged(this, e);
        }

        protected void OnCollectionReset()
        {
            OnCollectionChanged(m_CollectionReset);
        }

        private void NotifyCollectionChanged()
        {
            OnCollectionReset();
            OnPropertyChanged(m_IndexerChanged);
            OnPropertyChanged(m_CountChanged);
        }

        protected abstract int InternalLoad(T[] data, int startIndex);

        private T LoadData(int index)
        {
            ++m_CacheRequest;
            LinkedListNode<CacheBlock> cacheBlockNode = m_CacheBlock.First;
            int indexInCacheBlock = -1;
            while (cacheBlockNode != null)
            {
                if (cacheBlockNode.Value.Contains(index, out indexInCacheBlock))
                    break;
                cacheBlockNode = cacheBlockNode.Next;
            }
            if (cacheBlockNode == null)
            {
                ++m_CacheMisses;
                CacheBlock cacheBlock;
                if (m_CacheBlock.Count < NumCacheBlocks)
                    cacheBlockNode = new LinkedListNode<CacheBlock>(cacheBlock = new CacheBlock(NumItemsPerCacheBlock));
                else
                {
                    cacheBlockNode = m_CacheBlock.Last;
                    m_CacheBlock.RemoveLast();
                    cacheBlock = cacheBlockNode.Value;
                }
                indexInCacheBlock = index % cacheBlock.Data.Length;
                int count = InternalLoad(cacheBlock.Data, cacheBlock.StartIndex = index - indexInCacheBlock);
                cacheBlock.EndIndex = Math.Min(count, cacheBlock.StartIndex + cacheBlock.Data.Length);
                if (count != m_Count)
                {
                    bool firstTime = m_Count == UninitializedCount;
                    // collection has changed in the meantime, update the count unless it was undefined
                    m_Count = count;
                    // signal that our collection has changed, if this is not the first time aroud
                    // failure to check for this will give a nullreferenceexception on collectionchanged
                    if (!firstTime)
                        NotifyCollectionChanged();
                    // clear the cache: the only block left is the one we're holding
                    m_CacheBlock.Clear();
                }
                m_CacheBlock.AddFirst(cacheBlockNode);
                // if the index is outside the bounds of the new count, return nothing
                if (indexInCacheBlock >= cacheBlock.EndIndex)
                    return null;
                else
                    return cacheBlock.Data[indexInCacheBlock];
            }
            else
            {
                // move the block to the front of the cache if it's not already there
                if (cacheBlockNode != m_CacheBlock.First)
                {
                    m_CacheBlock.Remove(cacheBlockNode);
                    m_CacheBlock.AddFirst(cacheBlockNode);
                }
                return cacheBlockNode.Value.Data[indexInCacheBlock];
            }
        }

        protected void ClearCache()
        {
            m_CacheBlock.Clear();
            m_Count = UndefinedCount;
            NotifyCollectionChanged();
        }


        #region IList<DataRefBase<T>> Members

        public int IndexOf(DataRefBase<T> item)
        {
            CachedDataRef dataRef = item as CachedDataRef;
            return dataRef != null && dataRef.List == this ? dataRef.Index : -1;
        }

        public void Insert(int index, DataRefBase<T> item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public DataRefBase<T> this[int index]
        {
            get
            {
                return new CachedDataRef(index, this);
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region ICollection<DataRefBase<T>> Members

        public void Add(DataRefBase<T> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(DataRefBase<T> item)
        {
            return item != null && ((CachedDataRef)item).List == this;
        }

        public void CopyTo(DataRefBase<T>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("arrayIndex");
            if (arrayIndex >= array.Length)
                throw new ArgumentException("arrayIndex is greater or equal than the array length");
            if (arrayIndex + Count > array.Length)
                throw new ArgumentException("Number of elements in list is greater than available space");
            foreach (var item in this)
                array[arrayIndex++] = item;
        }

        public int Count
        {
            get
            {
                // if the count hasn't been determind yet, we need to access at least one remote item to get it
                if (m_Count == UndefinedCount || m_Count == UninitializedCount)
                    LoadData(0);
                return m_Count;
            }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(DataRefBase<T> item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<DataRefBase<T>> Members

        public IEnumerator<DataRefBase<T>> GetEnumerator()
        {
            for (int index = 0; index < Count; ++index)
                yield return this[index];
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IList Members

        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            CachedDataRef dataRef = value as CachedDataRef;
            return dataRef != null && Contains(dataRef);
        }

        public int IndexOf(object value)
        {
            CachedDataRef dataRef = value as CachedDataRef;
            return dataRef != null ? IndexOf(dataRef) : -1;
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public bool IsFixedSize
        {
            get { return true; }
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (array.Rank != 1)
                throw new ArgumentException("Array rank must be 1");
            if (index < array.GetLowerBound(0))
                throw new ArgumentOutOfRangeException("index");
            if (index > array.GetUpperBound(0))
                throw new ArgumentException("arrayIndex is greater or equal than the array upper bound");
            if (index + Count - 1 > array.GetUpperBound(0))
                throw new ArgumentException("Number of elements in list is greater than available space");
            if (array is DataRefBase<T>[])
                CopyTo((DataRefBase<T>[])array, index);
            else
                try
                {
                    foreach (var t in this)
                        array.SetValue(t, index++);
                }
                catch (Exception e)
                {
                    throw new InvalidCastException("Type of datalist cannot be converted to the type of the destination array", e);
                }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get
            {
                if (m_SyncRoot == null)
                    Interlocked.CompareExchange(ref m_SyncRoot, new object(), null);
                return m_SyncRoot;
            }
        }

        #endregion

        #region IItemProperties Members

        public ReadOnlyCollection<ItemPropertyInfo> ItemProperties
        {
            get
            {
                return m_ItemPropertyInfo;
            }
        }

        #endregion

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
