using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Markup;

namespace ExpandTreeViewWPF
{
    /// <summary>
    /// ViewModel for the Taxonomy data source and its derived classes.
    /// This ViewModel adds the notion of node selection and expansion.
    /// </summary>
    [ContentProperty("Subclasses")]
    public abstract class TaxonomyViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Constructs a new TaxonomyViewModel from the given Taxonomy.
        /// </summary>
        /// <param name="taxonomy">The original Taxonomy object to wrap.</param>
        protected TaxonomyViewModel(Taxonomy taxonomy)
        {
            Taxonomy = taxonomy;
            subclasses = new TaxonomyViewModelCollection(taxonomy.Subclasses);
        }

        /// <summary>
        /// Original Taxonomy object being wrapped.
        /// </summary>
        public Taxonomy Taxonomy { get; private set; }

        /// <summary>
        /// Gets and sets the Classification of the original Taxonomy.
        /// </summary>
        public string Classification
        {
            get { return Taxonomy.Classification; }
            set 
            { 
                Taxonomy.Classification = value;
                OnPropertyChanged("Classification");
            }
        }

        /// <summary>
        /// Gets the Rank of the original Taxonomy.
        /// </summary>
        public string Rank
        {
            get { return Taxonomy.Rank; }
        }

        /// <summary>
        /// The TaxonomyViewModelCollection ensures that any object added to it is also added
        /// to the original Taxonomy. 
        /// </summary>
        private TaxonomyViewModelCollection subclasses;
        public Collection<TaxonomyViewModel> Subclasses { get { return subclasses; } }

        /// <summary>
        /// Determines whether the TreeViewItem associated with this data item
        /// is expanded.
        /// </summary>
        private bool isExpanded;
        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                isExpanded = value;
                OnPropertyChanged("IsExpanded");
            }
        }

        /// <summary>
        /// Determines whether the TreeViewItem associated with this data item
        /// is selected.
        /// </summary>
        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        /// <summary>
        /// Event raised when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// This method traverses the entire view model hierarchy setting the IsExpanded 
        /// property to true on each item.
        /// </summary>
        public void ExpandAll()
        {
            ApplyActionToAllItems(item => item.IsExpanded = true);
        }

        /// <summary>
        /// This method traverses the entire view model hierarchy setting the IsExpanded 
        /// property to false on each item.
        /// </summary>
        public void CollapseAll()
        {
            // Here, I start collapsing items in the tree starting at the root.
            // It may seem that the tree needs to be collapsed starting at the leaf
            // nodes, but that's not the case because the entire tree will be updated
            // in one single layout pass. So in this case, the order in which items
            // are collapsed makes no difference.
            ApplyActionToAllItems(item => item.IsExpanded = false);
        }

        /// <summary>
        /// This helper method traverses the tree in a depth-first non-recursive way 
        /// and executes the action passed as a parameter on each item.
        /// </summary>
        /// <param name="itemAction">Action to be executed for each item.</param>
        private void ApplyActionToAllItems(Action<TaxonomyViewModel> itemAction)
        {
            Stack<TaxonomyViewModel> dataItemStack = new Stack<TaxonomyViewModel>();
            dataItemStack.Push(this);

            while (dataItemStack.Count != 0)
            {
                TaxonomyViewModel currentItem = dataItemStack.Pop();
                itemAction(currentItem);
                foreach (TaxonomyViewModel childItem in currentItem.Subclasses)
                {
                    dataItemStack.Push(childItem);
                }
            }
        }

        /// <summary>
        /// This method sets IsExpanded to true for each element in the ancestor chain of the item
        /// passed as a parameter.
        /// </summary>
        /// <param name="itemToLookFor">The element this method will look for.</param>
        /// <returns>True if it the itemToLookFor was found, false otherwise.</returns>
        public bool ExpandSuperclasses(TaxonomyViewModel itemToLookFor)
        {
            return ApplyActionToSuperclasses(itemToLookFor, superclass => superclass.IsExpanded = true);
        }

        /// <summary>
        /// This helper method uses recursion to look for the element passed as a parameter in the view model 
        /// hierarchy and executes the action passed as a parameter to its entire ancestor chain (excluding
        /// the item itself).
        /// </summary>
        /// <param name="itemToLookFor">The element this method will look for.</param>
        /// <param name="itemAction">Action to be executed on each superclass in the ancestor chain.</param>
        /// <returns>True if it the itemToLookFor was found, false otherwise.</returns>
        private bool ApplyActionToSuperclasses(TaxonomyViewModel itemToLookFor, Action<TaxonomyViewModel> itemAction)
        {
            if (itemToLookFor == this)
            {
                return true;
            }
            else
            {
                foreach (TaxonomyViewModel subclass in this.Subclasses)
                {
                    bool foundItem = subclass.ApplyActionToSuperclasses(itemToLookFor, itemAction);
                    if (foundItem)
                    {
                        itemAction(this);
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// A collection that keeps the original Taxonomy collection in sync with
        /// the view model collection.
        /// </summary>
        private class TaxonomyViewModelCollection : Collection<TaxonomyViewModel>
        {
            private Collection<Taxonomy> originalCollection;

            public TaxonomyViewModelCollection(Collection<Taxonomy> originalCollection)
            {
                this.originalCollection = originalCollection;
            }

            protected override void InsertItem(int index, TaxonomyViewModel item)
            {
                base.InsertItem(index, item);
                originalCollection.Insert(index, item.Taxonomy);
            }

            protected override void RemoveItem(int index)
            {
                base.RemoveItem(index);
                originalCollection.RemoveAt(index);
            }

            protected override void ClearItems()
            {
                base.ClearItems();
                originalCollection.Clear();
            }

            protected override void SetItem(int index, TaxonomyViewModel item)
            {
                base.SetItem(index, item);
                originalCollection[index] = item.Taxonomy;
            }
        }
    }

    /// <summary>
    /// Represents a Domain in a Linnaean taxonomy.
    /// </summary>
    public sealed class DomainViewModel : TaxonomyViewModel
    {
        public DomainViewModel()
            : base(new Domain())
        {
        }
    }

    /// <summary>
    /// Represents a Kingdom in a Linnaean taxonomy.
    /// </summary>
    public sealed class KingdomViewModel : TaxonomyViewModel
    {
        public KingdomViewModel()
            : base(new Kingdom())
        {
        }
    }

    /// <summary>
    /// Represents a Class in a Linnaean taxonomy.
    /// </summary>
    public sealed class ClassViewModel : TaxonomyViewModel
    {
        public ClassViewModel()
            : base(new Class())
        {
        }
    }

    /// <summary>
    /// Represents a Family in a Linnaean taxonomy.
    /// </summary>
    public sealed class FamilyViewModel : TaxonomyViewModel
    {
        public FamilyViewModel()
            : base(new Family())
        {
        }
    }

    /// <summary>
    /// Represents a Genus in a Linnaean taxonomy.
    /// </summary>
    public sealed class GenusViewModel : TaxonomyViewModel
    {
        public GenusViewModel()
            : base(new Genus())
        {
        }
    }

    /// <summary>
    /// Represents an Order in a Linnaean taxonomy.
    /// </summary>
    public sealed class OrderViewModel : TaxonomyViewModel
    {
        public OrderViewModel()
            : base(new Order())
        {
        }
    }

    /// <summary>
    /// Represents a Phylum in a Linnaean taxonomy.
    /// </summary>
    public sealed class PhylumViewModel : TaxonomyViewModel
    {
        public PhylumViewModel()
            : base(new Phylum())
        {
        }
    }

    /// <summary>
    /// Represents a Species in a Linnaean taxonomy.
    /// </summary>
    public sealed class SpeciesViewModel : TaxonomyViewModel
    {
        public SpeciesViewModel()
            : base(new Species())
        {
        }
    }
}
