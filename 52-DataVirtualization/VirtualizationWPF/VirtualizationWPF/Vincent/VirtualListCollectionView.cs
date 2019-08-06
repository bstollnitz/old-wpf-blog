using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Collections;

namespace VirtualizationWPF
{
    class VirtualListCollectionView<T> : VirtualListBase<T>, ICollectionView where T : class
    {
        private static readonly PropertyChangedEventArgs m_CulturePropertyChanged = new PropertyChangedEventArgs("Culture");
        private static readonly PropertyChangedEventArgs m_IsCurrentBeforeFirstChanged = new PropertyChangedEventArgs("IsCurrentBeforeFirst");
        private static readonly PropertyChangedEventArgs m_IsCurrentAfterLastChanged = new PropertyChangedEventArgs("IsCurrentAfterLast");
        private static readonly PropertyChangedEventArgs m_CurrentPositionChanged = new PropertyChangedEventArgs("CurrentPosition");
        private static readonly PropertyChangedEventArgs m_CurrentItemChanged = new PropertyChangedEventArgs("CurrentItem");
        private readonly VirtualList<T> m_SourceCollection;
        private readonly Func<SortDescriptionCollection, Predicate<object>, T[], int, int> Load;
        private int m_DeferRefreshCount;
        private bool m_NeedsRefresh;
        private CultureInfo m_CultureInfo;
        private int m_CurrentPosition;
        private DataRefBase<T> m_CurrentItem;
        private bool m_IsCurrentAfterLast;
        private bool m_IsCurrentBeforeFirst;
        private Predicate<object> m_Filter;
        private SortDescriptionCollection m_SortDescriptionCollection;

        private class RefreshDeferrer : IDisposable
        {
            private VirtualListCollectionView<T> m_List;

            public RefreshDeferrer(VirtualListCollectionView<T> list)
            {
                m_List = list;
            }

            #region IDisposable Members

            public void Dispose()
            {
                if (m_List != null)
                {
                    m_List.EndDeferRefresh();
                    m_List = null;
                }
            }

            #endregion
        }


        public VirtualListCollectionView(VirtualList<T> list)
            : base(list.NumCacheBlocks, list.NumItemsPerCacheBlock)
        {
            Load = list.Load;
            m_SourceCollection = list;
            // initialize current item and markers
            if (list.Count == 0)
                m_IsCurrentAfterLast = m_IsCurrentBeforeFirst = true;
            else
            {
                m_CurrentPosition = 0;
                m_CurrentItem = list[0];
            }
            m_NeedsRefresh = true;
        }


        protected override int InternalLoad(T[] data, int startIndex)
        {
            return Load(m_SortDescriptionCollection, m_Filter, data, startIndex);
        }

        private bool IsRefreshDeferred
        {
            get
            {
                return m_DeferRefreshCount > 0;
            }
        }

        private void ThrowIfDeferred()
        {
            if (IsRefreshDeferred)
                throw new Exception("Can't do this while I'm deferred");
        }

        private void RefreshOrDefer()
        {
            if (IsRefreshDeferred)
                m_NeedsRefresh = true;
            else
                Refresh();
        }

        private void EndDeferRefresh()
        {
            if (0 == --m_DeferRefreshCount && m_NeedsRefresh)
                Refresh();
        }

        private bool IsCurrentInView
        {
            get
            {
                ThrowIfDeferred();
                return CurrentPosition >= 0 && CurrentPosition < Count;
            }
        }


        private void OnCurrentChanged()
        {
            if (CurrentChanged != null)
                CurrentChanged(this, EventArgs.Empty);
        }

        private void SortDescriptionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshOrDefer();
        }

        private void SetCurrent(DataRefBase<T> newItem, int newPosition, int count)
        {
            if (newItem != null)
                m_IsCurrentBeforeFirst = m_IsCurrentAfterLast = false;
            else if (count == 0)
            {
                m_IsCurrentBeforeFirst = m_IsCurrentAfterLast = true;
                newPosition = -1;
            }
            else
            {
                m_IsCurrentBeforeFirst = newPosition < 0;
                m_IsCurrentAfterLast = newPosition >= count;
            }
            m_CurrentItem = newItem;
            m_CurrentPosition = newPosition;
        }

        private void SetCurrent(DataRefBase<T> newItem, int newPosition)
        {
            int count = newItem != null ? 0 : Count;
            SetCurrent(newItem, newPosition, count);
        }

        private bool PassesFilter(object item)
        {
            return (CanFilter && Filter != null) ? Filter(item) : true;
        }

        private bool OnCurrentChanging()
        {
            if (CurrentChanging == null)
                return true;
            else
            {
                CurrentChangingEventArgs e = new CurrentChangingEventArgs();
                CurrentChanging(this, e);
                return !e.Cancel;
            }
        }

        #region ICollectionView Members

        public bool CanFilter
        {
            get { return true; }
        }

        public bool CanGroup
        {
            get { return false; }
        }

        public bool CanSort
        {
            get { return true; }
        }

        public CultureInfo Culture
        {
            get
            {
                return m_CultureInfo;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                if (m_CultureInfo != value)
                {
                    m_CultureInfo = value;
                    OnPropertyChanged(m_CulturePropertyChanged);
                }
            }
        }

        public event EventHandler CurrentChanged;

        public event CurrentChangingEventHandler CurrentChanging;

        public object CurrentItem
        {
            get
            {
                ThrowIfDeferred();
                return m_CurrentItem;
            }
        }

        public int CurrentPosition
        {
            get
            {
                ThrowIfDeferred();
                return m_CurrentPosition;
            }
        }

        public IDisposable DeferRefresh()
        {
            ++m_DeferRefreshCount;
            return new RefreshDeferrer(this);
        }

        public Predicate<object> Filter
        {
            get
            {
                return m_Filter;
            }
            set
            {
                if (!CanFilter)
                    throw new NotSupportedException("Filter not supported");
                m_Filter = value;
                RefreshOrDefer();
            }
        }

        public ObservableCollection<GroupDescription> GroupDescriptions
        {
            get { return null; }
        }

        public ReadOnlyObservableCollection<object> Groups
        {
            get { return null; }
        }

        public bool IsCurrentAfterLast
        {
            get
            {
                ThrowIfDeferred();
                return m_IsCurrentAfterLast;
            }
        }

        public bool IsCurrentBeforeFirst
        {
            get
            {
                ThrowIfDeferred();
                return m_IsCurrentBeforeFirst;
            }
        }

        public bool IsEmpty
        {
            get { return Count == 0; }
        }

        public bool MoveCurrentTo(object item)
        {
            ThrowIfDeferred();
            if (Object.Equals(CurrentItem, item) && (item != null || IsCurrentInView))
                return IsCurrentInView;
            int position = -1;
            if (PassesFilter(item))
                position = IndexOf(item);
            return this.MoveCurrentToPosition(position);
        }

        public bool MoveCurrentToFirst()
        {
            ThrowIfDeferred();
            return MoveCurrentToPosition(0);
        }

        public bool MoveCurrentToLast()
        {
            ThrowIfDeferred();
            return MoveCurrentToPosition(Count - 1);
        }

        public bool MoveCurrentToNext()
        {
            ThrowIfDeferred();
            int position = CurrentPosition + 1;
            return position <= Count && MoveCurrentToPosition(position);
        }

        public bool MoveCurrentToPosition(int position)
        {
            ThrowIfDeferred();
            if (position < -1 || position > Count)
                throw new ArgumentOutOfRangeException("position");
            if (position != CurrentPosition && OnCurrentChanging())
            {
                bool isCurrentBeforeFirst = m_IsCurrentBeforeFirst;
                bool isCurrentAfterLast = m_IsCurrentAfterLast;
                if (position < 0)
                {
                    m_IsCurrentBeforeFirst = true;
                    SetCurrent(null, -1);
                }
                else if (position >= Count)
                {
                    m_IsCurrentAfterLast = true;
                    SetCurrent(null, Count);
                }
                else
                {
                    m_IsCurrentBeforeFirst = m_IsCurrentAfterLast = false;
                    SetCurrent(this[position], position);
                }
                OnCurrentChanged();
                if (isCurrentBeforeFirst != m_IsCurrentBeforeFirst)
                    OnPropertyChanged(m_IsCurrentBeforeFirstChanged);
                if (isCurrentAfterLast != m_IsCurrentAfterLast)
                    OnPropertyChanged(m_IsCurrentAfterLastChanged);
                OnPropertyChanged(m_CurrentPositionChanged);
                OnPropertyChanged(m_CurrentItemChanged);
            }
            return IsCurrentInView;
        }

        public bool MoveCurrentToPrevious()
        {
            ThrowIfDeferred();
            int position = CurrentPosition - 1;
            return position >= -1 && MoveCurrentToPosition(position);
        }

        public void Refresh()
        {
            DataRefBase<T> currentItem = (DataRefBase<T>)CurrentItem;
            int currentPosition = IsEmpty ? -1 : CurrentPosition;
            bool isCurrentBeforeFirst = m_IsCurrentBeforeFirst;
            bool isCurrentAfterLast = m_IsCurrentAfterLast;
            OnCurrentChanging();
            ClearCache();
            if (isCurrentBeforeFirst || IsEmpty)
                SetCurrent(null, 0);
            else if (isCurrentAfterLast)
                SetCurrent(null, Count);
            else
            {
                int index = IndexOf(currentItem);
                if (index < 0)
                    SetCurrent(null, -1);
                else
                    SetCurrent(currentItem, index);
            }
            m_NeedsRefresh = false;
            OnCollectionReset();
            OnCurrentChanged();
            if (isCurrentBeforeFirst != m_IsCurrentBeforeFirst)
                OnPropertyChanged(m_IsCurrentBeforeFirstChanged);
            if (isCurrentAfterLast != m_IsCurrentAfterLast)
                OnPropertyChanged(m_IsCurrentAfterLastChanged);
            if (currentPosition != CurrentPosition)
                OnPropertyChanged(m_CurrentPositionChanged);
            if (currentItem != CurrentItem)
                OnPropertyChanged(m_CurrentItemChanged);
        }

        public SortDescriptionCollection SortDescriptions
        {
            get
            {
                if (m_SortDescriptionCollection == null)
                {
                    m_SortDescriptionCollection = new SortDescriptionCollection();
                    ((INotifyCollectionChanged)m_SortDescriptionCollection).CollectionChanged += SortDescriptionsChanged;
                }
                return m_SortDescriptionCollection;
            }
        }


        public IEnumerable SourceCollection
        {
            get { return m_SourceCollection; }
        }

        #endregion
    }
}
